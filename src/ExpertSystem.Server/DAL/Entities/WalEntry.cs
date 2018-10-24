﻿using ExpertSystem.Common.Generated;
using ExpertSystem.Common.Models;

namespace ExpertSystem.Server.DAL.Entities
{
    /// <summary>Структура записей в WAL</summary>
    /// <typeparam name="TR">Тип записи</typeparam>
    /// <typeparam name="TE">Тип расширения</typeparam>
    public class WalEntry<TR, TE>
    {
        /// <summary>Парсинг строки WAL файла </summary>
        /// <param name="line">Строка WAL файла</param>
        /// <returns>Запись </returns>
        public static WalEntry<TR, TE> ParseFromString(string line)
        {
            var parts = line.Split(Delimiter);
            var action = (CsvDbAction) int.Parse(parts[0]);
            var hashCode = int.Parse(parts[1]);
            var socket = TE.Deserialize(parts[2]);
            return new WalEntry<TR, TE>(action, hashCode, socket);
        }

        // Разделитель строки WAL-лога
        private const string Delimiter = ":::";

        internal readonly CsvDbAction Action;
        internal readonly int HashCode;
        internal readonly CustomSocket Socket;

        public WalEntry(CsvDbAction action, int hashCode, CustomSocket socket = null)
        {
            Action = action;
            HashCode = hashCode;
            Socket = socket;
        }

        public override string ToString()
        {
            return Action + Delimiter + HashCode + Delimiter + (Socket != null
                       ? TE.Serialize(Socket)
                       : "");
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