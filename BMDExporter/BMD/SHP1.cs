using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameFormatReader.Common;
using BMDExporter.Geometry;
using BMDExporter.util;
using Assimp;

namespace BMDExporter.BMD
{
    class SHP1
    {
        List<VertexAttributes> AttributesPerBatch; // A list made up of the attributes in each batch
        List<Batch> Batches;

        public SHP1()
        {

        }

        public SHP1(List<Batch> batches)
        {
            Batches = batches;
            AttributesPerBatch = new List<VertexAttributes>();

            foreach (Batch bat in batches)
            {
                //if (AttributesPerBatch.)
                AttributesPerBatch.AddRange(bat.ActiveAttributes);
                AttributesPerBatch.Add(VertexAttributes.NullAttr);
            }
        }

        public void WriteSHP1(EndianBinaryWriter writer)
        {
            writer.Write("SHP1".ToCharArray()); // Write chunk tag, SHP1
            writer.Write((int)0); // Write placeholder for chunk size
            writer.Write((short)Batches.Count); // Write number of chunks
            writer.Write(ushort.MaxValue); // Padding
            writer.Write((int)0x2C); // Offset to batch info

            // Write placeholders for data offsets
            for (int i = 0; i < 7; i++)
                writer.Write((int)0);

            // Write batch info
            for (int i = 0; i < Batches.Count; i++)
            {
                writer.Write((short)0x00FF); // Unknown
                writer.Write((short)1); // Packet count
                writer.Write((short)i); // Index to attribute data
                writer.Write((short)i); // Index to matrix info
                writer.Write((short)i); // Index to packet info
                writer.Write(ushort.MaxValue); // Padding

                writer.Write(Batches[i].BoundingSphereRadius);
                writer.Write(Batches[i].BoundingMin.X);
                writer.Write(Batches[i].BoundingMin.Y);
                writer.Write(Batches[i].BoundingMin.Z);
                writer.Write(Batches[i].BoundingMax.X);
                writer.Write(Batches[i].BoundingMax.Y);
                writer.Write(Batches[i].BoundingMax.Z);
            }

            Util.PadStream(writer, 32, true);

            writer.Seek(0x10, 0);
            writer.Write((int)writer.BaseStream.Length);
            writer.Seek(0, System.IO.SeekOrigin.End);

            // Write index table
            for (int i = 0; i < Batches.Count; i++)
                writer.Write((short)i);

            Util.PadStream(writer, 32, true);

            writer.Seek(0x18, 0);
            writer.Write((int)writer.BaseStream.Length);
            writer.Seek(0, System.IO.SeekOrigin.End);

            // Write attribute info
            foreach (VertexAttributes attrib in AttributesPerBatch)
            {
                writer.Write((int)attrib);
                if (attrib == VertexAttributes.NullAttr)
                    writer.Write((int)0); // Null has 0 in the data type field
                else
                    writer.Write((int)3); // The indexes are stored as signed16
            }

            Util.PadStream(writer, 32, true);

            writer.Seek(0x1C, 0);
            writer.Write((int)writer.BaseStream.Length);
            writer.Seek(0, System.IO.SeekOrigin.End);

            // Write matrix table
            for (int i = 0; i < Batches.Count; i++)
                writer.Write((short)i);

            Util.PadStream(writer, 32, true);

            writer.Seek(0x20, 0);
            writer.Write((int)writer.BaseStream.Length);
            writer.Seek(0, System.IO.SeekOrigin.End);

            // Write primitives
            foreach (Batch bat in Batches)
            {
                writer.Write((byte)0x90);
                writer.Write((short)bat.Faces.Count);
                foreach (short shr in bat.FaceIndexes)
                    writer.Write(shr);
                Util.PadStream(writer, 32, false);
            }

            writer.Seek(0x24, 0);
            writer.Write((int)writer.BaseStream.Length);
            writer.Seek(0, System.IO.SeekOrigin.End);

            // Write Matrix Data?
            for (int i = 0; i < Batches.Count; i++)
            {
                writer.Write((int)1);
                writer.Write((int)i);
            }

            Util.PadStream(writer, 32, false);

            writer.Seek(0x28, 0);
            writer.Write((int)writer.BaseStream.Length);
            writer.Seek(0, System.IO.SeekOrigin.End);

            int packetOffset = 0;

            // Write packet location data
            foreach (Batch bat in Batches)
            {
                // Padding (number of indexes * length of a short + primitive type byte + number of faces short
                int packetSize = Util.PadLength((bat.FaceIndexes.Count * 2) + 3, 32);

                writer.Write(packetSize);
                writer.Write(packetOffset);

                packetOffset += packetSize;
            }

            Util.PadStream(writer, 32, true);

            // Go to and write section length
            writer.BaseStream.Position = 4;
            writer.Write((int)writer.BaseStream.Length);
            writer.BaseStream.Seek(0, System.IO.SeekOrigin.End);
        }
    }
}
