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
    class INF1
    {
        List<Batch> Batches;

        public INF1(List<Batch> batches)
        {
            Batches = batches;
        }

        public void WriteINF1(EndianBinaryWriter writer, int VertexCount)
        {
            writer.Write("INF1".ToCharArray()); // Chunk tag INF1
            writer.Write((int)0); // Placeholder for chunk size
            writer.Write((short)0); // Unknown?
            writer.Write(ushort.MaxValue); // Padding
            writer.Write((int)Batches.Count); // Packet count
            writer.Write((int)VertexCount); // Vertex count
            writer.Write((int)0x18); // Hierarchy offset

            // Write world root node
            writer.Write((ushort)HierarchyDataTypes.Joint);
            writer.Write((short)0);

            writer.Write((ushort)HierarchyDataTypes.NewNode);
            writer.Write((short)0);

            for (int i = 0; i < Batches.Count; i++)
            {
                // Write joint
                writer.Write((ushort)HierarchyDataTypes.Joint);
                writer.Write((short)(i + 1));

                // Move down to material
                writer.Write((ushort)HierarchyDataTypes.NewNode);
                writer.Write((short)0);

                // Write material
                writer.Write((ushort)HierarchyDataTypes.Material);
                writer.Write((short)i);

                // Move down to batch
                writer.Write((ushort)HierarchyDataTypes.NewNode);
                writer.Write((short)0);

                // Write batch
                writer.Write((ushort)HierarchyDataTypes.Shape);
                writer.Write((short)i);

                // Return to material
                writer.Write((ushort)HierarchyDataTypes.EndNode);
                writer.Write((short)0);

                // Return to bone
                writer.Write((ushort)HierarchyDataTypes.EndNode);
                writer.Write((short)0);

                // Return to root
                //writer.Write((ushort)HierarchyDataTypes.EndNode);
                //writer.Write((short)0);
            }

            writer.Write((ushort)HierarchyDataTypes.Finish);
            writer.Write((short)0);

            Util.PadStream(writer, 32, true);

            // Go to and write section length
            writer.BaseStream.Position = 4;
            writer.Write((int)writer.BaseStream.Length);
            writer.BaseStream.Seek(0, System.IO.SeekOrigin.End);
        }
    }
}
