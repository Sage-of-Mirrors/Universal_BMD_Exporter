using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assimp;
using GameFormatReader.Common;

namespace BMDExporter.Materials
{
    class TexMatrix
    {
        public byte Projection;
        public byte Type;
        public float CenterS;
        public float CenterT;
        public float Unknown0;
        public float ScaleS;
        public float ScaleT;
        public float Rotation;
        public float TranslateS;
        public float TranslateT;
        public Matrix4x4 PreMatrix;

        public TexMatrix()
        {
            Projection = 1;
            CenterS = .5f;
            CenterT = .5f;

            ScaleS = 1.0f;
            ScaleT = 1.0f;

            Unknown0 = .5f;

            PreMatrix = Matrix4x4.Identity;
        }

        public void Write(EndianBinaryWriter writer)
        {
            writer.Write(Projection);
            writer.Write(Type);

            // Padding
            writer.Write((short)-1);

            writer.Write(CenterS);
            writer.Write(CenterT);
            writer.Write(Unknown0);
            writer.Write(ScaleS);
            writer.Write(ScaleT);
            writer.Write((short)(Rotation));

            // Padding
            writer.Write((short)-1);

            writer.Write(TranslateS);
            writer.Write(TranslateT);

            writer.Write(PreMatrix.A1);
            writer.Write(PreMatrix.A2);
            writer.Write(PreMatrix.A3);
            writer.Write(PreMatrix.A4);
            writer.Write(PreMatrix.B1);
            writer.Write(PreMatrix.B2);
            writer.Write(PreMatrix.B3);
            writer.Write(PreMatrix.B4);
            writer.Write(PreMatrix.C1);
            writer.Write(PreMatrix.C2);
            writer.Write(PreMatrix.C3);
            writer.Write(PreMatrix.C4);
            writer.Write(PreMatrix.D1);
            writer.Write(PreMatrix.D2);
            writer.Write(PreMatrix.D3);
            writer.Write(PreMatrix.D4);
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(TexMatrix))
                return Compare((TexMatrix)obj);
            else
                return false;
        }

        private bool Compare(TexMatrix obj)
        {
            if ((Projection == obj.Projection) && (Type == obj.Type) && (CenterS == obj.CenterS) && (CenterT == obj.CenterT)
                && (Unknown0 == obj.Unknown0) && (ScaleS == obj.ScaleS) && (ScaleT == obj.ScaleT) && (Rotation == obj.Rotation)
                && (TranslateS == obj.TranslateS) && (TranslateT == obj.TranslateT) && (PreMatrix == obj.PreMatrix))
                return true;
            else
                return false;
        }

        public static bool operator ==(TexMatrix left, TexMatrix right)
        {
            if (System.Object.ReferenceEquals(left, right))
                return true;

            if (((object)left == null) || ((object)right == null))
                return false;

            if ((left.Projection == right.Projection) && (left.Type == right.Type) && (left.CenterS == right.CenterS) && (left.CenterT == right.CenterT)
                && (left.Unknown0 == right.Unknown0) && (left.ScaleS == right.ScaleS) && (left.ScaleT == right.ScaleT) && (left.Rotation == right.Rotation)
                && (left.TranslateS == right.TranslateS) && (left.TranslateT == right.TranslateT) && (left.PreMatrix == right.PreMatrix))
                return true;
            else
                return false;
        }

        public static bool operator !=(TexMatrix left, TexMatrix right)
        {
            if (System.Object.ReferenceEquals(left, right))
                return false;

            if (((object)left == null) || ((object)right == null))
                return true;

            if ((left.Projection == right.Projection) && (left.Type == right.Type) && (left.CenterS == right.CenterS) && (left.CenterT == right.CenterT)
                && (left.Unknown0 == right.Unknown0) && (left.ScaleS == right.ScaleS) && (left.ScaleT == right.ScaleT) && (left.Rotation == right.Rotation)
                && (left.TranslateS == right.TranslateS) && (left.TranslateT == right.TranslateT) && (left.PreMatrix == right.PreMatrix))
                return false;
            else
                return true;
        }
    }
}
