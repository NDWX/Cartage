
namespace Pug.Cartage
{
	public class CartSummary : ICartSummary
	{
		decimal totalProducts, totalItems, totalPrice;

		public CartSummary(decimal totalProducts, decimal totalItems)
		{
			this.totalProducts = totalProducts;
			this.totalItems = totalItems;
		}
		public decimal TotalProducts
		{
			get
			{
				return totalProducts;
			}
		}
		public decimal TotalItems
		{
			get
			{
				return totalItems;
			}
		}
	}
}
