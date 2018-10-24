using System.Collections.Generic;
using ExpertSystem.Common.Generated;
using ExpertSystem.Common.Models;

namespace ExpertSystem.Server.DAL.Extensions
{
	public class ServerCategoryExtension : IRecordExtension<SocketGroup>
	{
		public char Delimiter { get; }

		public string Serialize(SocketGroup record)
		{
			throw new System.NotImplementedException();
		}

		public SocketGroup Deserialize(string line)
		{
			throw new System.NotImplementedException();
		}

		public SocketGroup Deserialize(IList<string> parts)
		{
			throw new System.NotImplementedException();
		}
	}
}