using APIGateway.Response;
using Microsoft.EntityFrameworkCore;
using ProductService_5000.Models;
using ProductService_5000.Request;

namespace ProductService_5000.Services
{
    public interface IS_Cart
    {
        Task<List<Cart>> GetAll(MRes_InfoUser currentUser);

        Task<string> Add(MRes_Cart request, MRes_InfoUser currentUser);
    }

    public class S_Cart : IS_Cart
    {
        private readonly ProductDbContext _context;

        public S_Cart(ProductDbContext context)
        {
            _context = context;
        }

        public async Task<string> Add(MRes_Cart request,MRes_InfoUser currentUser)
        {
            var existingProductInCart = await _context.Carts.FirstOrDefaultAsync(m=>m.IdProduct == request.IdProduct && m.IdUser == int.Parse(currentUser.IdUser));
            if (existingProductInCart != null) 
            {
                request.Quantity += existingProductInCart.Quantity;
            }

            var newCart = new Cart()
            {
                IdProduct = request.IdProduct,
                IdUser = int.Parse(currentUser.IdUser),
                Quantity = request.Quantity
            };

            await _context.AddAsync(newCart);
            await _context.SaveChangesAsync();
            return "Thêm vào giỏ hàng thành công";
        }

        public async Task<List<Cart>> GetAll(MRes_InfoUser currentUser)
        {
            var getAll = await _context.Carts.Where(m=>m.IdUser ==int.Parse(currentUser.IdUser)).ToListAsync();
            return getAll;
        }
    }
}
