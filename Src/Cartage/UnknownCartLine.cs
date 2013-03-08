using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pug;
using Pug.Application.Security;

namespace Pug.Cartage
{
	public class UnknownCartLine : Exception
	{
		public UnknownCartLine()
			: base()
		{
		}
	}
}
