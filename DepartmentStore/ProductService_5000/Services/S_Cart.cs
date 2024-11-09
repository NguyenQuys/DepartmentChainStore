using APIGateway.Response;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProductService_5000.Models;
using ProductService_5000.Response;

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
        private static List<Cart> _cart = new List<Cart>();

        public S_Cart(ProductDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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
            var getAll = await _context.Carts.Where(m=>m.IdUser ==int.Parse(currentUser.IdUser)).ToListAsync();
            return getAll;
        }
    }
}
