using APIGateway.Response;
using AutoMapper;
using BranchService_5003.Models;
using InvoiceService_5005.InvoiceModels;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProductService_5000.Models;
using ProductService_5000.Response;
using System.Net.Http.Headers;
using System.Text.Json;

namespace ProductService_5000.Services
{
	public interface IS_Cart
	{
		Task<List<MRes_Product>> GetAll(int idBranch, MRes_InfoUser currentUser);
		Task<string> Add(MRes_Cart request, MRes_InfoUser currentUser);
		Task<string> Delete(int id, MRes_InfoUser currentUser);

		// Invoice
		Task<List<Invoice>> HistoryPurchase(MRes_InfoUser currentUser);

		Task<List<MRes_Product>> InvoiceIndex(string stringifyCarts, int idBranch);
		Task<List<MRes_InvoiceEmail>> ListInvoiceToShip(MRes_InfoUser currentUser);
	}

	public class S_Cart : IS_Cart
	{
		private readonly ProductDbContext _context;
		private readonly IMapper _mapper;
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private ISession Session => _httpContextAccessor.HttpContext.Session;

		private static readonly string CartAddSessionKey = "CartAdd";
		private static readonly string CartChosenSessionKey = "CartChosen";

		public S_Cart(ProductDbContext context, IMapper mapper, IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
		{
			_context = context;
			_mapper = mapper;
			_httpClientFactory = httpClientFactory;
			_httpContextAccessor = httpContextAccessor;
		}

		public async Task<string> Add(MRes_Cart request, MRes_InfoUser currentUser)
		{
			if (currentUser.AccessToken == null)
			{
				var cart = GetCartFromSession();

				var existingProduct = cart.FirstOrDefault(m => m.IdProduct == request.IdProduct);
				if (existingProduct != null)
				{
					existingProduct.Quantity += request.Quantity;
				}
				else
				{
					var newCartTemp = _mapper.Map<Cart>(request);
					newCartTemp.Id = 0;
					newCartTemp.IdUser = 0;
					cart.Add(newCartTemp);
				}

				SaveCartToSession(cart);
			}
			else
			{
				var userId = int.Parse(currentUser.IdUser);
				var existingProductInCart = await _context.Carts
					.FirstOrDefaultAsync(m => m.IdProduct == request.IdProduct && m.IdUser == userId);

				if (existingProductInCart != null)
				{
					existingProductInCart.Quantity += request.Quantity;
					_context.Update(existingProductInCart);
				}
				else
				{
					var newCart = _mapper.Map<Cart>(request);
					newCart.IdUser = userId;
					await _context.AddAsync(newCart);
				}

				await _context.SaveChangesAsync();
			}

			return "Thêm vào giỏ hàng thành công";
		}

		public async Task<List<MRes_Product>> GetAll(int idBranch, MRes_InfoUser currentUser)
		{
			List<Cart> cartEntities;

			if (currentUser?.AccessToken != null)
			{
				// Fetch authenticated user's cart from the database
				var userId = int.Parse(currentUser.IdUser);
				cartEntities = await _context.Carts
					.Where(m => m.IdUser == userId && m.IdBranch == idBranch)
					.ToListAsync();
			}
			else
			{
				cartEntities = GetCartFromSession();
			}

			var productIds = cartEntities.Select(m => m.IdProduct).Distinct().ToList();
			var products = await _context.Products
				.Where(m => productIds.Contains(m.Id))
				.ToDictionaryAsync(m => m.Id);

			var cartDTOs = cartEntities
				.Where(cartEntity => products.ContainsKey(cartEntity.IdProduct))
				.Select(cartEntity => new MRes_Product
				{
					IdProduct = products[cartEntity.IdProduct].Id,
					ProductName = products[cartEntity.IdProduct].ProductName,
					Price = products[cartEntity.IdProduct].Price,
					Quantity = cartEntity.Quantity,
					IdBranch = idBranch,
					MainImage = products[cartEntity.IdProduct].MainImage
				})
				.ToList();

			return cartDTOs;
		}

		public async Task<string> Delete(int idProduct, MRes_InfoUser currentUser)
		{
			if (currentUser.AccessToken != null)
			{
				var cartChosen = await _context.Carts.FirstOrDefaultAsync(m => m.IdProduct == idProduct);
				_context.Remove(cartChosen);
				await _context.SaveChangesAsync();
			}
			else
			{
				var cartList = GetCartFromSession();
				var cartToDetele = cartList.FirstOrDefault(m => m.IdProduct == idProduct);
				cartList.Remove(cartToDetele);

				SaveCartToSession(cartList);
			}
			return "Xóa khỏi giỏ hàng thành công";
		}

		//****************************** Others **************************
		private List<Cart> GetCartFromSession()
		{
			var cartData = Session.GetString(CartAddSessionKey);
			return string.IsNullOrEmpty(cartData) ? new List<Cart>() : System.Text.Json.JsonSerializer.Deserialize<List<Cart>>(cartData);
		}

		private void SaveCartToSession(List<Cart> cart)
		{
			Session.SetString(CartAddSessionKey, System.Text.Json.JsonSerializer.Serialize(cart));
		}

		public async Task<List<MRes_Product>> InvoiceIndex(string stringifyCarts, int idBranch)
		{
			// Attempt to deserialize the cartItems from the passed JSON string.
			List<MRes_Product> cartItems;
			try
			{
				cartItems = JsonConvert.DeserializeObject<List<MRes_Product>>(stringifyCarts);
				if (cartItems == null)
				{
					throw new Exception("Invalid cart data.");
				}
			}
			catch (Exception ex)
			{
				// You can log the exception if needed.
				throw new Exception("Failed to parse cart data.", ex);
			}

			using var client = _httpClientFactory.CreateClient("ProductService");

			// Get branch data from the service.
			var branchResponse = await client.GetAsync($"/Branch/GetById?id={idBranch}");
			if (!branchResponse.IsSuccessStatusCode)
			{
				throw new Exception("Failed to retrieve data from the Branch Service.");
			}

			var branch = await branchResponse.Content.ReadFromJsonAsync<Branch>();
			if (branch == null)
			{
				throw new Exception("Branch data is empty.");
			}

			// Assign branch data to each item in the cart.
			foreach (var item in cartItems)
			{
				item.LatitudeBranch = branch.Latitude;
				item.LongitudeBranch = branch.Longtitude;
				item.IdBranch = branch.Id;
			}

			return cartItems;
		}

		public async Task<List<Invoice>> HistoryPurchase(MRes_InfoUser currentUser)
		{
			using var client = _httpClientFactory.CreateClient("ProductService");
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", currentUser.AccessToken);
			var responseInvoiceList = await client.GetAsync($"/Invoice/HistoryPurchaseJson?phoneNumber={currentUser.PhoneNumber}");
			if(!responseInvoiceList.IsSuccessStatusCode)
			{
				throw new Exception("Khong lay duoc thong tin");
			}

			var invoiceList = await responseInvoiceList.Content.ReadFromJsonAsync<List<Invoice>>();
			return invoiceList;
		}

		public async Task<List<MRes_InvoiceEmail>> ListInvoiceToShip(MRes_InfoUser currentUser)
		{
			using var client = _httpClientFactory.CreateClient("ProductService");
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", currentUser.AccessToken);

			var reponseInvoiceList = await client.GetAsync($"Invoice/GetListInvoiceByIdShipper?idShipper={int.Parse(currentUser.IdUser)}");
			if(!reponseInvoiceList.IsSuccessStatusCode)
			{
				throw new Exception("Khong lay dc du lieu");
			}

			var invoiceList = await reponseInvoiceList.Content.ReadFromJsonAsync<List<MRes_InvoiceEmail>>();
			return invoiceList;
		}
	}
}

//var detail = new MRes_InvoiceEmail()
//{
//	IdInvoice = id,
//	Address = invoice.Address,
//	InvoiceNumber = invoice.InvoiceNumber,
//	Time = invoice.CreatedDate,
//	ProductNameAndQuantity = productNameAndQuantity,
//	SinglePrice = listSinglePrice,
//	Discount = discount,
//	Total = totalOriginalPrice - discount,
//	PaymentMethod = invoice.PaymentMethod.Method,
//	Status = invoice.Status.Type,
//	CustomerNote = invoice.CustomerNote,
//	StoreNote = invoice.StoreNote
//};