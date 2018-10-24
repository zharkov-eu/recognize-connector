using System;
using System.Collections.Generic;
using ExpertSystem.Common.Models;
using ExpertSystem.Server.DAL.Entities;

namespace ExpertSystem.Server.DAL.Extensions
{
	public class WalEntryExtension<T> : IRecordExtension<WalEntry<T>>
	{
		// Разделитель строки WAL-лога
		public char Delimiter { get; }

		private readonly IRecordExtension<T> _extension;

		public WalEntryExtension(IRecordExtension<T> extension)
		{
			Delimiter = ':';

			_extension = extension;
		}

		public string Serialize(WalEntry<T> record)
		{
			return record.Action + Delimiter + record.HashCode + Delimiter
			       + (record.Record != null ? _extension.Serialize(record.Record) : "");
		}

		public WalEntry<T> Deserialize(string line)
		{
			return Deserialize(line.Split(Delimiter));
		}

		public WalEntry<T> Deserialize(IList<string> parts)
		{
			var action = (CsvDbAction) int.Parse(parts[0]);
			var hashCode = int.Parse(parts[1]);
			var socket = _extension.Deserialize(parts[2]);
			return new WalEntry<T>(action, hashCode, socket);
		}
	}
}