namespace ExpertSystem.Common.Models
{
	/// <summary>Абстрактный класс расширений над CSV записью</summary>
	/// <typeparam name="TR">Тип записи</typeparam>
	public abstract class RecordExtension<TR>
	{
		public abstract string Serialize(TR record);

		public abstract TR Deserialize(string line);
	}
}