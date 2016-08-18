using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assimp;
using BMDExporter.Geometry;
using GameFormatReader.Common;
using BMDExporter.util;

namespace BMDExporter.BMD
{
    class VTX1
    {
        List<VertexAttributes> MasterAttributes = new List<VertexAttributes>(); // A list of all the unique attributes from all the batches

        public List<Vector3D> VertexPositions; // X/Y/Z data for vertexes. Any model will have these
        // The following are not required but often present
        public List<Color4D>[] VertexColors; // RGBA Color data for vertexes. There can be up to 2 sets.
        public List<Vector3D> VertexNormals; // Normal data for vertexes
        public List<Vector3D>[] VertexUVWs; // UVW data for vertexes. We're only going to use UV for the BMD though, and there can be up to 8 different sets.

        public VTX1()
        {

        }

        public VTX1(List<Batch> batches)
        {
            MasterAttributes = new List<VertexAttributes>();
            VertexPositions = new List<Vector3D>();
            VertexColors = new List<Color4D>[2] { new List<Color4D>(), new List<Color4D>() };
            VertexNormals = new List<Vector3D>();
            VertexUVWs = new List<Vector3D>[8] { new List<Vector3D>(), new List<Vector3D>(), new List<Vector3D>(), new List<Vector3D>(),
                                                 new List<Vector3D>(), new List<Vector3D>(), new List<Vector3D>(), new List<Vector3D>()};

            foreach (Batch bat in batches)
            {
                foreach (VertexAttributes attrib in bat.ActiveAttributes)
                {
                    if (!MasterAttributes.Contains(attrib))
                        MasterAttributes.Add(attrib);
                }

                // We're going to go through each face in this batch.
                // For each face, we're going to go through each index that the face has. There should be 3, because faces are only supported as triangles.
                // For each index, we're going to go through each active attribute that this batch has.
                // Finally, for each attribute, we're going to add data to the corresponding list (Position -> VertexPositions, 
                // Normal -> VertexNormals, etc) if necessary, and then fix the face's index
                // so that it points to the correct data in the global list.
                foreach (Face face in bat.Faces)
                {
                    for (int i = 0; i < face.IndexCount; i++)
                    {
                        foreach (VertexAttributes attr in bat.ActiveAttributes)
                        {
                            switch (attr)
                            {
                                case VertexAttributes.Position:
                                    // If the vertex position that this face's index at position i in its index array
                                    // isn't in the VertexPositions list, we need to add it.
                                    if (!VertexPositions.Contains(bat.VertexPositions[face.Indices[i]]))
                                        VertexPositions.Add(bat.VertexPositions[face.Indices[i]]);

                                    // Then we set the index equal to the index of the position if it existed, or to the
                                    // position we just added if it didn't.
                                    bat.FaceIndexes.Add((short)VertexPositions.IndexOf(bat.VertexPositions[face.Indices[i]]));
                                    break;
                                case VertexAttributes.Normal:
                                    if (!VertexNormals.Contains(bat.VertexNormals[face.Indices[i]]))
                                        VertexNormals.Add(bat.VertexNormals[face.Indices[i]]);

                                    bat.FaceIndexes.Add((short)VertexNormals.IndexOf(bat.VertexNormals[face.Indices[i]]));
                                    break;
                                case VertexAttributes.Color0:
                                    if (!VertexColors[0].Contains(bat.VertexColors[0][face.Indices[i]]))
                                        VertexColors[0].Add(bat.VertexColors[0][face.Indices[i]]);

                                    bat.FaceIndexes.Add((short)VertexColors[0].IndexOf(bat.VertexColors[0][face.Indices[i]]));
                                    break;
                                case VertexAttributes.Color1:
                                    if (!VertexColors[1].Contains(bat.VertexColors[1][face.Indices[i]]))
                                        VertexColors[1].Add(bat.VertexColors[1][face.Indices[i]]);

                                    bat.FaceIndexes.Add((short)VertexColors[1].IndexOf(bat.VertexColors[1][face.Indices[i]]));
                                    break;
                                case VertexAttributes.Tex0:
                                    if (!VertexUVWs[0].Contains(bat.VertexUVWs[0][face.Indices[i]]))
                                        VertexUVWs[0].Add(bat.VertexUVWs[0][face.Indices[i]]);

                                    bat.FaceIndexes.Add((short)VertexUVWs[0].IndexOf(bat.VertexUVWs[0][face.Indices[i]]));
                                    break;
                                case VertexAttributes.Tex1:
                                    if (!VertexUVWs[1].Contains(bat.VertexUVWs[1][face.Indices[i]]))
                                        VertexUVWs[1].Add(bat.VertexUVWs[1][face.Indices[i]]);

                                    bat.FaceIndexes.Add((short)VertexUVWs[1].IndexOf(bat.VertexUVWs[1][face.Indices[i]]));
                                    break;
                                case VertexAttributes.Tex2:
                                    if (!VertexUVWs[2].Contains(bat.VertexUVWs[2][face.Indices[i]]))
                                        VertexUVWs[2].Add(bat.VertexUVWs[2][face.Indices[i]]);

                                    bat.FaceIndexes.Add((short)VertexUVWs[2].IndexOf(bat.VertexUVWs[2][face.Indices[i]]));
                                    break;
                                case VertexAttributes.Tex3:
                                    if (!VertexUVWs[3].Contains(bat.VertexUVWs[3][face.Indices[i]]))
                                        VertexUVWs[3].Add(bat.VertexUVWs[3][face.Indices[i]]);

                                    bat.FaceIndexes.Add((short)VertexUVWs[3].IndexOf(bat.VertexUVWs[3][face.Indices[i]]));
                                    break;
                                case VertexAttributes.Tex4:
                                    if (!VertexUVWs[4].Contains(bat.VertexUVWs[4][face.Indices[i]]))
                                        VertexUVWs[4].Add(bat.VertexUVWs[4][face.Indices[i]]);

                                    bat.FaceIndexes.Add((short)VertexUVWs[4].IndexOf(bat.VertexUVWs[4][face.Indices[i]]));
                                    break;
                                case VertexAttributes.Tex5:
                                    if (!VertexUVWs[5].Contains(bat.VertexUVWs[5][face.Indices[i]]))
                                        VertexUVWs[5].Add(bat.VertexUVWs[5][face.Indices[i]]);

                                    bat.FaceIndexes.Add((short)VertexUVWs[5].IndexOf(bat.VertexUVWs[5][face.Indices[i]]));
                                    break;
                                case VertexAttributes.Tex6:
                                    if (!VertexUVWs[6].Contains(bat.VertexUVWs[6][face.Indices[i]]))
                                        VertexUVWs[6].Add(bat.VertexUVWs[6][face.Indices[i]]);

                                    bat.FaceIndexes.Add((short)VertexUVWs[6].IndexOf(bat.VertexUVWs[6][face.Indices[i]]));
                                    break;
                                case VertexAttributes.Tex7:
                                    if (!VertexUVWs[7].Contains(bat.VertexUVWs[7][face.Indices[i]]))
                                        VertexUVWs[7].Add(bat.VertexUVWs[7][face.Indices[i]]);

                                    bat.FaceIndexes.Add((short)VertexUVWs[7].IndexOf(bat.VertexUVWs[7][face.Indices[i]]));
                                    break;
                                case VertexAttributes.NullAttr:
                                    break;
                                default:
                                    throw new ArgumentException(string.Format("Unsupported vertex attribute {0}!", attr.ToString()));
                            }
                        }
                    }
                }
            }

            // Just to make sure the attributes are in numerical order
            MasterAttributes.Sort();
        }

        public void WriteVTX1(EndianBinaryWriter writer)
        {
            // Header
            writer.Write("VTX1".ToCharArray()); // Chunk tag, VTX1
            writer.Write((int)0); // Placeholder for total section size
            writer.Write((int)0x40); // Offset to attribute table

            // Placeholders for attribute data offsets
            for (int i = 0; i < 13; i++)
                writer.Write((int)0);

            // Attribute table
            foreach (VertexAttributes attrib in MasterAttributes)
            {
                writer.Write((int)attrib);

                switch (attrib)
                {
                    case VertexAttributes.Position:
                        writer.Write((int)1);
                        writer.Write((int)4);
                        writer.Write((ushort)0x00FF);
                        writer.Write(ushort.MaxValue);
                        break;
                    case VertexAttributes.Normal:
                        writer.Write((int)0);
                        writer.Write((int)4);
                        writer.Write((ushort)0x00FF);
                        writer.Write(ushort.MaxValue);
                        break;
                    case VertexAttributes.Color0:
                    case VertexAttributes.Color1:
                        writer.Write((int)1);
                        writer.Write((int)5);
                        writer.Write((ushort)0x00FF);
                        writer.Write(ushort.MaxValue);
                        break;
                    case VertexAttributes.Tex0:
                    case VertexAttributes.Tex1:
                    case VertexAttributes.Tex2:
                    case VertexAttributes.Tex3:
                    case VertexAttributes.Tex4:
                    case VertexAttributes.Tex5:
                    case VertexAttributes.Tex6:
                    case VertexAttributes.Tex7:
                        writer.Write((int)1);
                        writer.Write((int)3);
                        writer.Write((ushort)0x08FF);
                        writer.Write(ushort.MaxValue);
                        break;
                    case VertexAttributes.NullAttr:
                        writer.Write((int)1);
                        writer.Write((int)0);
                        writer.Write((ushort)0x00FF);
                        writer.Write(ushort.MaxValue);
                        break;
                }
            }

            Util.PadStream(writer, 32, true);

            // Attribute data
            foreach (VertexAttributes attrib in MasterAttributes)
            {
                switch (attrib)
                {
                    case VertexAttributes.Position:
                        writer.Seek(0xC, 0);
                        writer.Write((int)writer.BaseStream.Length);
                        writer.Seek(0, System.IO.SeekOrigin.End);

                        foreach (Vector3D vec in VertexPositions)
                        {
                            writer.Write(vec.X);
                            writer.Write(vec.Y);
                            writer.Write(vec.Z);
                        }
                        break;
                    case VertexAttributes.Normal:
                        writer.Seek(0x10, 0);
                        writer.Write((int)writer.BaseStream.Length);
                        writer.Seek(0, System.IO.SeekOrigin.End);

                        foreach (Vector3D vec in VertexNormals)
                        {
                            writer.Write(vec.X);
                            writer.Write(vec.Y);
                            writer.Write(vec.Z);
                        }
                        break;
                    case VertexAttributes.Color0:
                    case VertexAttributes.Color1:
                        int colorID = (int)attrib - 11; // Whether this is Color0 or Color1

                        writer.Seek(0x18 + (colorID * 4), 0);
                        writer.Write((int)writer.BaseStream.Length);
                        writer.Seek(0, System.IO.SeekOrigin.End);

                        foreach (Color4D col in VertexColors[colorID])
                        {
                            writer.Write((byte)(col.R * 255.999));
                            writer.Write((byte)(col.G * 255.999));
                            writer.Write((byte)(col.B * 255.999));
                            writer.Write((byte)(col.A * 255.999));
                        }
                        break;
                    case VertexAttributes.Tex0:
                    case VertexAttributes.Tex1:
                    case VertexAttributes.Tex2:
                    case VertexAttributes.Tex3:
                    case VertexAttributes.Tex4:
                    case VertexAttributes.Tex5:
                    case VertexAttributes.Tex6:
                    case VertexAttributes.Tex7:
                        int texID = (int)attrib - 13; // Whether this is Tex0, Tex1, Tex2, etc
                        float scaleFactor = (float)Math.Pow(0.5, 8);

                        writer.Seek(0x20 + (texID * 4), 0);
                        writer.Write((int)writer.BaseStream.Length);
                        writer.Seek(0, System.IO.SeekOrigin.End);

                        foreach (Vector3D vec in VertexUVWs[texID])
                        {
                            writer.Write((short)(vec.X / scaleFactor));
                            writer.Write((short)(vec.Y / scaleFactor));
                        }
                        break;
                }

                Util.PadStream(writer, 32, true);
            }

            Util.PadStream(writer, 32, true);

            // Go to and write section length
            writer.BaseStream.Position = 4;
            writer.Write((int)writer.BaseStream.Length);
            writer.BaseStream.Seek(0, System.IO.SeekOrigin.End);
        }
    }
}
