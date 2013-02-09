using System;

namespace Pug.Cartage
{
	public interface ICartInfo
	{
		DateTime Created { get; }
		string CreateUser { get; }
		string Identifier { get; }
		DateTime LastModified { get; }
		string LastModifyUser { get; }
	}
}