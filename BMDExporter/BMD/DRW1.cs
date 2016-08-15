using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assimp;
using GameFormatReader.Common;
using BMDExporter.Geometry;
using BMDExporter.util;

namespace BMDExporter.BMD
{
    class DRW1
    {
        List<bool> IsPartiallyWeightedBools; // Determines if the vertexes that got referred to drw1 are fully or partially weighted
        List<short> FullyWeightedIndexes; // Indexes for bones that are fully weighted
        List<short> PartiallyWeightedIndexes; // Indexes for EVP1 section partial weighting

        public DRW1()
        {

        }

        public DRW1(List<Batch> batches)
        {
            IsPartiallyWeightedBools = new List<bool>();
            FullyWeightedIndexes = new List<short>();
            PartiallyWeightedIndexes = new List<short>();

            // More will have to be added to this to determine full vs partial weight

            for (int i = 0; i < batches.Count; i++)
            {
                IsPartiallyWeightedBools.Add(false);
                FullyWeightedIndexes.Add((short)(i + 1));
            }
        }

        public void WriteDRW1(EndianBinaryWriter writer)
        {
            writer.Write("DRW1".ToCharArray()); // Chunk tag DRW1
            writer.Write((int)0); // Placeholder for chunk size
            writer.Write((short)IsPartiallyWeightedBools.Count); // Number of bools/indexes
            writer.Write(ushort.MaxValue); // Padding
            writer.Write((int)0x14); // Offset to IsPartiallyWeighted bools
            writer.Write((int)(0x14 + IsPartiallyWeightedBools.Count)); // Offset to indexes

            foreach (bool bl in IsPartiallyWeightedBools)
                writer.Write(bl);

            foreach (short shr in FullyWeightedIndexes)
                writer.Write(shr);

            foreach (short shr in PartiallyWeightedIndexes)
                writer.Write(shr);

            Util.PadStream(writer, 32, true);

            // Go to and write section length
            writer.BaseStream.Position = 4;
            writer.Write((int)writer.BaseStream.Length);
            writer.BaseStream.Seek(0, System.IO.SeekOrigin.End);
        }
    }
}
