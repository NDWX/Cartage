using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pug;
using Pug.Application.Security;

namespace Pug.Cartage
{
	public class CartNotFound : Exception
	{
		//string identifier;

		public CartNotFound(string identifier)
			: base(string.Format("Cart : {0} is not known", identifier))
		{
		}
	}
}