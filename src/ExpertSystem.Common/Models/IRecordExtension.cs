using System.Collections.Generic;

namespace ExpertSystem.Common.Models
{
	/// <summary>Интерфейс расширений над CSV записью</summary>
	/// <typeparam name="T">Тип записи</typeparam>
	public interface IRecordExtension<T>
	{
		char Delimiter { get; }

		string Serialize(T record);

		T Deserialize(string line);

		T Deserialize(IList<string > parts);
	}
}