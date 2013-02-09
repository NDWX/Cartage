using System;
namespace Pug.Cartage
{
	public interface ICartSummary
	{
		decimal TotalItems { get; }
		//decimal TotalPrice { get; }
		decimal TotalProducts { get; }
	}
}
