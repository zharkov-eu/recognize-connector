using System;
using System.Collections.Generic;
using System.Linq;
using ExpertSystem.Common.Generated;

namespace ExpertSystem.Common.Models
{
	/// <summary>Расширение для работы с классмо группы разъёмов</summary>
	public static class SocketGroupExtension
	{
		public static readonly Type GroupType = typeof(SocketGroup);

		public static readonly char Delimiter = ';';

		/// <summary>Преобразование группы разъёмов в CSV строку</summary>
		/// <param name="group">Группа разъёмов</param>
		/// <returns></returns>
		public static string Serialize(SocketGroup group)
		{
			var result = group.GroupName + Delimiter;
			foreach (var socketName in group.SocketNames)
				result += socketName + Delimiter;
			return result;
		}

		/// <summary>Десериализация CSV строки</summary>
		/// <param name="line">Строка</param>
		/// <returns>Группа разъёмов на основании переданной CSV строки</returns>
		public static SocketGroup Deserialize(string line)
		{
			return Deserialize(line.Split(Delimiter));
		}

		/// <summary>Десериализация CSV строки</summary>
		/// <param name="parts">Значения</param>
		/// <returns>Группа разъёмов на основании переданной CSV строки</returns>
		public static SocketGroup Deserialize(IList<string> parts)
		{
			// Проверить отсутствие разделителя в конце строки
			var diff = 1;
			if (parts[parts.Count - 1] == "")
				diff = 2;
			return new SocketGroup
			{
				GroupName = parts[0],
				SocketNames = { parts.Skip(1).Take(parts.Count - diff) }
			};
		}
	}
}