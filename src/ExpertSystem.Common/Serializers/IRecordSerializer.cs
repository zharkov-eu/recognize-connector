namespace ExpertSystem.Server.DAL.Serializers
{
    /// <summary>Интерфейс сериализации над CSV-записью</summary>
    /// <typeparam name="T">Тип записи</typeparam>
    public interface IRecordSerializer<T>
    {
        string Serialize(T record);

        T Deserialize(string line);
    }
}