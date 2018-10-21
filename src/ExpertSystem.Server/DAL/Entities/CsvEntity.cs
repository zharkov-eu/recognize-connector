using ExpertSystem.Common.Generated;

namespace ExpertSystem.Server.DAL.Entities
{
    /// <summary>Структура записей в WAL</summary>
    public class CsvEntity
    {
        /// <summary>Парсинг строки WAL файла </summary>
        /// <param name="line">Строка WAL файла</param>
        /// <returns>Запись </returns>
        public static CsvEntity ParseFromString(string line)
        {
            var parts = line.Split(Delimiter);
            var action = (CsvDbAction)int.Parse(parts[0]);
            var hashCode = int.Parse(parts[1]);
            var socket = StorageCustomSocket.Deserialize(parts[2]);
            return new CsvEntity(action, hashCode, socket);
        }

        // Разделитель строки WAL-лога
        private const string Delimiter = ":::";

        internal readonly CsvDbAction Action;
        internal readonly int HashCode;
        internal readonly CustomSocket Socket;

        internal CsvEntity(CsvDbAction action, int hashCode, CustomSocket socket = null)
        {
            Action = action;
            HashCode = hashCode;
            Socket = socket;
        }

        public override string ToString()
        {
            return Action + Delimiter + HashCode + Delimiter + StorageCustomSocket.Serialize(Socket);
        }
    }

    /// <summary>Действия WAL</summary>
    public enum CsvDbAction
    {
        Insert,
        Update,
        Delete
    }
}
