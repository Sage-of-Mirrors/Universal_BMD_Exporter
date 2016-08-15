using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameFormatReader.Common;

namespace BMDExporter.util
{
    public static class Util
    {
        static string PadString = "This is padding data to align.  ";

        public static void PadStream(EndianBinaryWriter writer, int padValue, bool usePaddingString)
        {
            // Pad up to a 32 byte alignment
            // Formula: (x + (n-1)) & ~(n-1)
            long nextAligned = (writer.BaseStream.Length + (padValue - 1)) & ~(padValue - 1);

            long delta = nextAligned - writer.BaseStream.Length;
            writer.BaseStream.Position = writer.BaseStream.Length;
            for (int i = 0; i < delta; i++)
            {
                if (usePaddingString)
                    writer.Write(PadString[i]);
                else
                    writer.Write((byte)0);
            }
        }

        public static int PadLength(int length, int padValue)
        {
            long nextAligned = (length + (padValue - 1)) & ~(padValue - 1);
            long delta = nextAligned - length;
            for (int i = 0; i < delta; i++)
                length++;
            return length;
        }
    }
}
