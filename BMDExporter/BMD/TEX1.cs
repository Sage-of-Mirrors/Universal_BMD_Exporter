using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BMDExporter.Textures;
using BMDExporter.util;
using GameFormatReader.Common;

namespace BMDExporter.BMD
{
    class TEX1
    {
        public TEX1()
        {

        }

        public void WriteTEX1(EndianBinaryWriter writer, List<BinaryTextureImage> textures)
        {
            // Base address for the start of the BTI header data for each image
            int imageHeadersBase = 0x20;

            writer.Write("TEX1".ToCharArray()); // Chunk tag, "TEX1"
            writer.Write((int)0); // Placeholder for chunk size
            writer.Write((short)textures.Count); // Texture count
            writer.Write(ushort.MaxValue); // Padding
            writer.Write((int)imageHeadersBase);
            writer.Write((int)0); // Placeholder for string table offset

            Util.PadStream(writer, 32, true);

            foreach (BinaryTextureImage tex in textures)
                tex.WriteHeader(writer);

            // Write image data offsets and image data
            for (int i = 0; i < textures.Count; i++)
            {
                int curHeaderOffset = imageHeadersBase + (i * 0x20);
                uint imageDataOffset = (uint)(writer.BaseStream.Length - curHeaderOffset);

                // 0x20 is the TEX1 header size, 0x0C is the offset to paletteDataOffset,
                // i * 0x20 is the current header
                writer.Seek(imageHeadersBase + 0x0C + (i * 0x20), 0);
                writer.Write((uint)(imageDataOffset - (i * 0x20)));

                // 0x20 is the TEX1 header size, 0x1C is the offset to the imageDataOffset,
                // i * 0x20 is the current header
                writer.Seek(imageHeadersBase + 0x1C + (i * 0x20), 0);
                writer.Write((uint)imageDataOffset);

                // Write actual data
                writer.Seek((int)writer.BaseStream.Length, 0);

                //imageDataOffset = (int)writer.BaseStream.Length - 0x20;
                writer.Write(textures[i].GetData());
            }

            // Write string table offset
            writer.Seek(0x10, 0);
            writer.Write((uint)writer.BaseStream.Length);
            writer.Seek((int)writer.BaseStream.Length, 0);

            // Write string table
            writer.Write((ushort)textures.Count);
            writer.Write((short)-1);

            ushort stringOffset = (ushort)(4 + ((textures.Count) * 4));

            // Hash and string offset
            foreach (BinaryTextureImage tex in textures)
            {
                writer.Write((ushort)HashName(tex.Name));
                writer.Write(stringOffset);

                stringOffset += (ushort)(tex.Name.Length + 1);
            }

            // String data with null terminators
            foreach (BinaryTextureImage tex in textures)
            {
                writer.Write(tex.Name.ToCharArray());
                writer.Write((byte)0);
            }

            Util.PadStream(writer, 32, true);

            writer.Seek(4, 0);
            writer.Write((uint)writer.BaseStream.Length);
            writer.Seek(0, System.IO.SeekOrigin.End);
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
