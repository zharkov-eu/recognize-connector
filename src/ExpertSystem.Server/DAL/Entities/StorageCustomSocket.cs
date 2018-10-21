using System;
using System.Collections.Generic;
using System.Globalization;
using ExpertSystem.Common.Generated;
using static ExpertSystem.Common.Models.CustomSocketDomain;

namespace ExpertSystem.Server.DAL.Entities
{
    /// <summary>Статические методы обработки CSV</summary>
    public static class StorageCustomSocket
    {
        // Разделитель CSV файла
        public static readonly char Delimiter = ';';

        /// <summary>Преобразует разъём в CSV строку</summary>
        /// <param name="socket">Разъём</param>
        /// <returns>CSV cтрока на основании данных разъёма c предобпределённым разделителем</returns>
        public static string Serialize(CustomSocket socket)
        {
            var socketType = typeof(CustomSocket);
            string result = "";
            foreach (var field in Fields)
                result += socketType.GetProperty(field.ToString()).GetValue(socket) + Delimiter.ToString();
            return result;
        }

        /// <summary>Десериализация CSV строки</summary>
        /// <param name="line">Строка</param>
        /// <returns>Разъём на основании переданной CSV строки</returns>
        public static CustomSocket Deserialize(string line)
        {
            return Deserialize(line.Split(Delimiter));
        }

        /// <summary>Десериализация CSV строки</summary>
        /// <param name="parts">Значения</param>
        /// <returns>Разъём на основании переданной CSV строки</returns>
        public static CustomSocket Deserialize(IList<string> parts)
        {
            var socket = new CustomSocket();
            var socketType = typeof(CustomSocket);
            for (var i = 0; i < Fields.Length; i++)
            {
                var domain = Fields[i];
                socketType.GetProperty(domain.ToString()).SetValue(socket, ParseProperty(domain, parts[i]));
            }
            return socket;
        }

        /// <summary>Преобразование к типу свойства</summary>
        /// <param name="domain">Домен свойства</param>
        /// <param name="value">Строковое значение</param>
        /// <returns>Значение свойства</returns>
        /// <exception cref="Exception">Тип не распознан</exception>
        private static object ParseProperty(SocketDomain domain, string value)
        {
            var type = SocketDomainType[domain];
            if (type == typeof(string))
                return value;
            if (type == typeof(int))
                return string.IsNullOrEmpty(value) ? -1 : int.Parse(value);
            if (type == typeof(float))
                return string.IsNullOrEmpty(value)
                    ? -1.0f
                    : float.Parse(value, CultureInfo.InvariantCulture);
            throw new Exception("Type not recognized");
        }

        // Порядок полей CSV файла
        private static readonly SocketDomain[] Fields =
        {
            SocketDomain.SocketName,
            SocketDomain.Gender,
            SocketDomain.ContactMaterial,
            SocketDomain.ContactPlating,
            SocketDomain.Color,
            SocketDomain.HousingColor,
            SocketDomain.HousingMaterial,
            SocketDomain.MountingStyle,
            SocketDomain.NumberOfContacts,
            SocketDomain.NumberOfPositions,
            SocketDomain.NumberOfRows,
            SocketDomain.Orientation,
            SocketDomain.PinPitch,
            SocketDomain.Material,
            SocketDomain.SizeDiameter,
            SocketDomain.SizeLength,
            SocketDomain.SizeHeight,
            SocketDomain.SizeWidth,
        };
    }
}
