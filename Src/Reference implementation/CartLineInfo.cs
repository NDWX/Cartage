using System.Collections.Generic;
using Pug.Extensions;

namespace Pug.Cartage
{
	public class CartLineInfo : Pug.Cartage.ICartLineInfo
	{
		string identifier, productCode;
		decimal quantity;

		public CartLineInfo(string identifier, string productCode, decimal quantity)
		{
			this.identifier = identifier;
			this.productCode = productCode;
			this.quantity = quantity;
		}

		public string Identifier
		{
			get
			{
				return identifier;
			}
		}

		public string ProductCode
		{
			get
			{
				return productCode;
			}
		}

		public decimal Quantity
		{
			get
			{
				return quantity;
			}
		}
	}
}
