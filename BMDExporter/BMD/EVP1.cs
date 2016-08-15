using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assimp;
using BMDExporter.Geometry;
using BMDExporter.util;
using GameFormatReader.Common;

namespace BMDExporter.BMD
{
    class EVP1
    {
        // Get ready for a lot of indexes

        List<int> Indexes; // Indexes into the bone index array
        List<int> BoneIndexes; // Bone indexes for weights
        List<float> Weights; // Blending weights
        List<Matrix4x4> InverseBindMatrices; // Matrices for bones that define the inverse bind pose

        public EVP1()
        {

        }

        public EVP1(List<Batch> batches)
        {
            Indexes = new List<int>();
            BoneIndexes = new List<int>();
            Weights = new List<float>();
            InverseBindMatrices = new List<Matrix4x4>();

            // More will come later ;_;
        }

        public void WriteEVP1(EndianBinaryWriter writer)
        {
            writer.Write("EVP1".ToCharArray()); // Chunk tag EVP1
            writer.Write((int)0); // Placeholder for chunk size

            if (Indexes.Count != 0)
            {
                // ...This means we have skinning data, but support for that will come much much later
            }

            else
            {
                writer.Write((short)0);
                writer.Write(ushort.MaxValue);
                for (int i = 0; i < 16; i++)
                    writer.Write((byte)0);
            }

            Util.PadStream(writer, 32, true);

            // Go to and write section length
            writer.BaseStream.Position = 4;
            writer.Write((int)writer.BaseStream.Length);
            writer.BaseStream.Seek(0, System.IO.SeekOrigin.End);
        }
    }
}
