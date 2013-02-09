using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pug.Cartage
{
	public class CartExists : Exception
	{
		public CartExists(string identifier)
			: base(string.Format("Cart with identifier : {0} already exists.", identifier))
		{
		}
	}
}
