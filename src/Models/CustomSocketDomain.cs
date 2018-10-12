using System;
using System.Linq;
using System.Collections.Generic;

namespace ExpertSystem.Models
{
    public class CustomSocketDomain
    {
        public static readonly Dictionary<SocketDomain, Type> SocketDomainType = new Dictionary<SocketDomain, Type>
        {
            { SocketDomain.Empty, typeof(string) },
            { SocketDomain.AmperageCircuit, typeof(float) },
            { SocketDomain.Gender, typeof(string) },
            { SocketDomain.ContactMaterial, typeof(string) },
            { SocketDomain.ContactPlating, typeof(string) },
            { SocketDomain.Color, typeof(string) },
            { SocketDomain.HousingColor, typeof(string) },
            { SocketDomain.HousingMaterial, typeof(string) },
            { SocketDomain.MountingStyle, typeof(string) },
            { SocketDomain.NumberOfContacts, typeof(int) },
            { SocketDomain.NumberOfPositions, typeof(int) },
            { SocketDomain.NumberOfRows, typeof(int) },
            { SocketDomain.Orientation, typeof(string) },
            { SocketDomain.PinPitch, typeof(float) },
            { SocketDomain.Material, typeof(string) },
            { SocketDomain.SizeDiameter, typeof(float) },
            { SocketDomain.SizeLength, typeof(float) },
            { SocketDomain.SizeHeight, typeof(float) },
            { SocketDomain.SizeWidth, typeof(float) },
            { SocketDomain.SocketName, typeof(string) }
        };

        public static readonly Dictionary<Type, object> SocketDefaultValue = new Dictionary<Type, object>
        {
            { typeof(string), "" },
            { typeof(int), -1 },
            { typeof(float), -1.0f }
        };

        public static string GetSocketDomainName(SocketDomain domain)
        {
            var type = domain.GetType();
            var memInfo = type.GetMember(domain.ToString());
            if (memInfo != null && memInfo.Length > 0)
            {
                var attrs = memInfo[0].GetCustomAttributes(typeof(Output), false);
                if (attrs != null && attrs.Length > 0)
                    return ((Output)attrs[0]).Text;
            }
            return domain.ToString();
        }

        public static IEnumerable<SocketDomain> GetSocketDomains()
        {
            return Enum.GetValues(typeof(SocketDomain)).Cast<SocketDomain>().Where(p => !DomainIgnore.Contains(p));
        }

        public static Dictionary<SocketDomain, int> GetFuzzySocketDomains()
        {
            return new Dictionary<SocketDomain, int>() {
                { SocketDomain.AmperageCircuit, 5 },
                { SocketDomain.NumberOfContacts, 5 },
                { SocketDomain.SizeLength, 4 },
                { SocketDomain.SizeWidth, 3 },
            };
        }

        public static Func<CustomSocket, double> GetAmperageCircuitFormula(Dictionary<SocketDomain, FuzzyRule> rules)
        {
            int nocCluster = rules[SocketDomain.NumberOfContacts].Cluster;
            int slCluster = rules[SocketDomain.SizeLength].Cluster;
            int swCluster = rules[SocketDomain.SizeWidth].Cluster;

            return socket => socket.NumberOfContacts * Math.Log(nocCluster, 2) + (
                (socket.SizeLength * Math.Exp(slCluster) * 100) * (socket.SizeWidth * Math.Pow(2, swCluster) * 100)
            );
        }

        public enum SocketDomain
        {
            [Output("не определен")]
            Empty,
            [Output("макимальная сила тока при разрыве цепи")]
            AmperageCircuit,
            [Output("пол разъема")]
            Gender,
            [Output("контактный материал")]
            ContactMaterial,
            [Output("контактная плата")]
            ContactPlating,
            [Output("цвет")]
            Color,
            [Output("цвет корпуса")]
            HousingColor,
            [Output("материал корпуса")]
            HousingMaterial,
            [Output("тип установки")]
            MountingStyle,
            [Output("число контактов")]
            NumberOfContacts,
            [Output("число позиций")]
            NumberOfPositions,
            [Output("число контактных линий")]
            NumberOfRows,
            [Output("положение разъема")]
            Orientation,
            [Output("шаг между контактами")]
            PinPitch,
            [Output("материал разъема")]
            Material,
            [Output("диаметр разъема")]
            SizeDiameter,
            [Output("длина разъема")]
            SizeLength,
            [Output("высота разъема")]
            SizeHeight,
            [Output("ширина разъема")]
            SizeWidth,
            [Output("разъем")]
            SocketName
        }

        public static List<SocketDomain> DomainIgnore = new List<SocketDomain> {
            SocketDomain.Empty,
            SocketDomain.AmperageCircuit,
        };

        private class Output : Attribute
        {
            public readonly string Text;
            public Output(string text)
            {
                Text = text;
            }
        }
    }
}