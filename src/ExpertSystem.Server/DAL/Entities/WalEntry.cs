using System.Collections.Generic;

namespace ExpertSystem.Server.DAL.Entities
{
    /// <summary>Структура записей в WAL</summary>
    /// <typeparam name="T">Тип записи</typeparam>
    public class WalEntry<T>
    {
        internal readonly CsvDbAction Action;
        internal readonly int HashCode;
        internal readonly T Record;

        public WalEntry(CsvDbAction action, int hashCode, T record = default(T))
        {
            Action = action;
            HashCode = hashCode;
            Record = record;
        }

        protected bool Equals(WalEntry<T> other)
        {
            return Action == other.Action && 
                   HashCode == other.HashCode && 
                   EqualityComparer<T>.Default.Equals(Record, other.Record);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((WalEntry<T>) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int) Action;
                hashCode = (hashCode * 397) ^ HashCode;
                hashCode = (hashCode * 397) ^ EqualityComparer<T>.Default.GetHashCode(Record);
                return hashCode;
            }
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