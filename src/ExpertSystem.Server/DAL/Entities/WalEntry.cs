using ExpertSystem.Common.Models;

namespace ExpertSystem.Server.DAL.Entities
{
    /// <summary>Структура записей в WAL</summary>
    /// <typeparam name="T">Тип записи</typeparam>
    public class WalEntry<T>
    {
        // Расширение
        private readonly IRecordExtension<T> _extension;

        internal readonly CsvDbAction Action;
        internal readonly int HashCode;
        internal readonly T Record;

        public WalEntry(CsvDbAction action, int hashCode, T record = default(T))
        {
            Action = action;
            HashCode = hashCode;
            Record = record;
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