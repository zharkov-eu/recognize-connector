using System;

namespace ExpertSystem.Common.Models
{
    public class CustomSocket
    {
        public static readonly Type Type = typeof(CustomSocket);
        public string Color;
        public string ContactMaterial;
        public string ContactPlating;
        public string Gender;
        public string HousingColor;
        public string HousingMaterial;
        public string Material;
        public string MountingStyle;
        public int NumberOfContacts;
        public int NumberOfPositions;
        public int NumberOfRows;
        public string Orientation;
        public float PinPitch;
        public float SizeDiameter;
        public float SizeHeight;
        public float SizeLength;
        public float SizeWidth;
        public string SocketName;

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            var socket = obj as CustomSocket;
            if (socket == null)
                return false;
            return SocketName == socket.SocketName &&
                   Gender == socket.Gender &&
                   ContactMaterial == socket.ContactMaterial &&
                   ContactPlating == socket.ContactPlating &&
                   Color == socket.Color &&
                   HousingColor == socket.HousingColor &&
                   HousingMaterial == socket.HousingMaterial &&
                   MountingStyle == socket.MountingStyle &&
                   NumberOfContacts == socket.NumberOfContacts &&
                   NumberOfPositions == socket.NumberOfPositions &&
                   NumberOfRows == socket.NumberOfRows &&
                   Orientation == socket.Orientation &&
                   PinPitch == socket.PinPitch &&
                   Material == socket.Material &&
                   SizeDiameter == socket.SizeDiameter &&
                   SizeLength == socket.SizeLength &&
                   SizeHeight == socket.SizeHeight &&
                   SizeWidth == socket.SizeWidth;
        }

        public override int GetHashCode()
        {
            var hash = 19;
            hash += SocketName.GetHashCode();
            hash += Gender.GetHashCode();
            hash += ContactMaterial.GetHashCode();
            hash += ContactPlating.GetHashCode();
            hash += Color.GetHashCode();
            hash += HousingColor.GetHashCode();
            hash += HousingMaterial.GetHashCode();
            hash += MountingStyle.GetHashCode();
            hash += NumberOfContacts.GetHashCode();
            hash += NumberOfPositions.GetHashCode();
            hash += NumberOfRows.GetHashCode();
            hash += Orientation.GetHashCode();
            hash += PinPitch.GetHashCode();
            hash += Material.GetHashCode();
            hash += SizeDiameter.GetHashCode();
            hash += SizeLength.GetHashCode();
            hash += SizeHeight.GetHashCode();
            hash += SizeWidth.GetHashCode();
            return base.GetHashCode();
        }
    }
}