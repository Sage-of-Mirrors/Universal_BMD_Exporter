using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameFormatReader.Common;
using Assimp;
using BMDExporter.Geometry;
using BMDExporter.util;

namespace BMDExporter.BMD
{
    class JNT1
    {
        List<Batch> Batches; // List of batches
        List<Joint> Joints; // List of joints

        public JNT1()
        {

        }

        public JNT1(List<Batch> batches)
        {
            Batches = batches;
        }

        public void WriteJNT1(EndianBinaryWriter writer)
        {
            writer.Write("JNT1".ToCharArray()); // Chunk tag JNT1
            writer.Write((int)0); // Placeholder for chunk size
            writer.Write((short)(Batches.Count + 1)); // Number of joints
            writer.Write(ushort.MaxValue); // Padding
            writer.Write((int)0x18); // Offset to joint data
            writer.Write((int)0); // Placeholder for offset to string ID table
            writer.Write((int)0); // Placeholder for offset to string table

            // Write world root
            writer.Write((short)0);
            writer.Write((short)0x00FF);
            writer.Write((float)1.0f);
            writer.Write((float)1.0f);
            writer.Write((float)1.0f);
            writer.Write((short)0);
            writer.Write((short)0);
            writer.Write((short)0);
            writer.Write(ushort.MaxValue);
            writer.Write((float)0.0f);
            writer.Write((float)0.0f);
            writer.Write((float)0.0f);
            writer.Write(0.0f);
            writer.Write(0.0f);
            writer.Write(0.0f);
            writer.Write(0.0f);
            writer.Write(0.0f);
            writer.Write(0.0f);
            writer.Write(0.0f);

            foreach (Batch bat in Batches)
            {
                writer.Write((short)0);
                writer.Write((short)0x00FF);
                writer.Write((float)1.0f);
                writer.Write((float)1.0f);
                writer.Write((float)1.0f);
                writer.Write((short)0);
                writer.Write((short)0);
                writer.Write((short)0);
                writer.Write(ushort.MaxValue);
                writer.Write((float)0.0f);
                writer.Write((float)0.0f);
                writer.Write((float)0.0f);
                writer.Write(bat.BoundingSphereRadius);
                writer.Write(bat.BoundingMin.X);
                writer.Write(bat.BoundingMin.Y);
                writer.Write(bat.BoundingMin.Z);
                writer.Write(bat.BoundingMax.X);
                writer.Write(bat.BoundingMax.Y);
                writer.Write(bat.BoundingMax.Z);
            }

            Util.PadStream(writer, 32, true);

            writer.Seek(0x10, 0);
            writer.Write((int)writer.BaseStream.Length);
            writer.Seek(0, System.IO.SeekOrigin.End);

            writer.Write((short)0);

            for (int i = 0; i < Batches.Count; i++)
                writer.Write((short)(i + 1));

            Util.PadStream(writer, 32, true);

            writer.Seek(0x14, 0);
            writer.Write((int)writer.BaseStream.Length);
            writer.Seek(0, System.IO.SeekOrigin.End);

            int stringTableStartOffset = (int)writer.BaseStream.Position;

            writer.Write((short)(Batches.Count + 1));
            writer.Write(ushort.MaxValue);

            writer.Write(HashName("world_root"));
            writer.Write((short)0);

            foreach (Batch bat in Batches)
            {
                writer.Write(HashName(bat.Name));
                writer.Write((short)0);
            }

            int rootNameOffset = (int)writer.BaseStream.Position - stringTableStartOffset;

            writer.Seek(stringTableStartOffset + 6, 0);
            writer.Write((short)rootNameOffset);
            writer.Seek(0, System.IO.SeekOrigin.End);

            writer.Write("world_root".ToCharArray());
            writer.Write((byte)0);

            for (int i = 0; i < Batches.Count; i++)
            {
                int thisNameOffset = (int)writer.BaseStream.Position;

                writer.Seek((stringTableStartOffset + 6) + ((i + 1) * 4), 0);
                writer.Write((short)(thisNameOffset - stringTableStartOffset));
                writer.Seek(0, System.IO.SeekOrigin.End);

                writer.Write(Batches[i].Name.ToCharArray());
                writer.Write((byte)0);
            }

            Util.PadStream(writer, 32, true);

            // Go to and write section length
            writer.BaseStream.Position = 4;
            writer.Write((int)writer.BaseStream.Length);
            writer.BaseStream.Seek(0, System.IO.SeekOrigin.End);
        }

        private ushort HashName(string name)
        {
            ushort hash = 0;
            foreach (char c in name)
            {
                hash *= 3;
                hash += (ushort)c;
            }

            return hash;
        }
    }
}
