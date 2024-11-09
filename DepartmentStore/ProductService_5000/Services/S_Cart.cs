using APIGateway.Response;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProductService_5000.Models;
using ProductService_5000.Response;
using System.Net.Http;

namespace ProductService_5000.Services
{
    public interface IS_Cart
    {
        Task<List<MRes_Product>> GetAll(int idBranch,MRes_InfoUser currentUser);

        Task<string> Add(MRes_Cart request, MRes_InfoUser currentUser);
    }

    public class S_Cart : IS_Cart
    {
        private readonly ProductDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpClientFactory _httpClientFactory;
        private static List<Cart> _cart = new List<Cart>();

        public S_Cart(ProductDbContext context, IMapper mapper, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _mapper = mapper;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> Add(MRes_Cart request,MRes_InfoUser currentUser)
        {
            if (currentUser.AccessToken == null)
            {
                var existingList = _cart.FirstOrDefault(m => m.IdProduct == request.IdProduct);
                if (existingList != null)
                {
                    existingList.Quantity += request.Quantity;
                }
                else
                {
                    var newCartTemp = _mapper.Map<Cart>(request);
                    newCartTemp.Id = 0;
                    newCartTemp.IdUser = 0;
                    _cart.Add(newCartTemp);
                }
            }
            else
            {
                var existingProductInCart = await _context.Carts.FirstOrDefaultAsync(m => m.IdProduct == request.IdProduct && m.IdUser == int.Parse(currentUser.IdUser));
                if (existingProductInCart != null)
                {
                    existingProductInCart.Quantity += request.Quantity;
                    _context.Update(existingProductInCart);
                }
                else
                {
                    var newCart = _mapper.Map<Cart>(request);
                    newCart.IdUser = int.Parse(currentUser.IdUser);
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
				cartEntities = await _context.Carts
											 .Where(m => m.IdUser == int.Parse(currentUser.IdUser) && m.IdBranch == idBranch)
											 .ToListAsync();
			}
			else
			{
				cartEntities = _cart; 
			}

			var productIds = cartEntities.Select(m => m.IdProduct).Distinct().ToList();

			var products = await _context.Products
										 .Where(m => productIds.Contains(m.Id))
										 .ToDictionaryAsync(m => m.Id);

			var cartDTOs = cartEntities
				.Where(cartEntity => products.ContainsKey(cartEntity.IdProduct))
				.Select(cartEntity => new MRes_Product
				{
					Id = products[cartEntity.IdProduct].Id,
					ProductName = products[cartEntity.IdProduct].ProductName,
					Price = products[cartEntity.IdProduct].Price,
					Quantity = cartEntity.Quantity,
					IdBranch = idBranch,
					MainImage = products[cartEntity.IdProduct].MainImage
				})
				.ToList();

			return cartDTOs;
		}
	}
}
