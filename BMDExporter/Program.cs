using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using BMDExporter.Geometry;
using BMDExporter.BMD;
using Assimp;
using GameFormatReader.Common;

namespace BMDExporter
{
    class Program
    {
        static void Main(string[] args) 
        {
            if (args.Length <= 0 || args[0] == "help" || args[0] == "h" || !File.Exists(args[0]))
            {
                DisplayUsageMessage();
                return;
            }

            string inputFile = args[0];
            string outputFile = Path.Combine(Path.GetDirectoryName(inputFile), Path.GetFileNameWithoutExtension(inputFile) + ".bmd");

            if (args.Length == 2)
                outputFile = args[1];

            List<Batch> Batches = new List<Batch>(); // A list of the meshes in the scene

            AssimpContext cont = new AssimpContext();
            // We flip the winding order of the meshes because BMD and BDL are clockwise rather than counter-clockwise
            Scene scene = cont.ImportFile(inputFile, PostProcessSteps.FlipWindingOrder);

            Node skeletonRoot = GetSkeletonRoot(scene);

            foreach (Mesh mesh in scene.Meshes)
            {
                Batches.Add(new Batch(mesh));
            }

            VTX1 vtx = new VTX1(Batches);

            SHP1 shp = new SHP1(Batches);

            EVP1 evp = new EVP1(Batches);

            DRW1 drw = new DRW1(Batches);

            JNT1 jnt = new JNT1(Batches);

            INF1 inf = new INF1(Batches);

            MAT3 mat = new MAT3(scene.Materials, inputFile, Batches);

            TEX1 tex = new TEX1();

            List<byte> fileBuffer = new List<byte>();

            Console.WriteLine("\nWriting header...");

            // Add header to fileBuffer
            using (MemoryStream headerStream = new MemoryStream())
            {
                EndianBinaryWriter writer = new EndianBinaryWriter(headerStream, Endian.Big);
                WriteHeader(writer);

                fileBuffer.AddRange(headerStream.ToArray());
            }

            Console.WriteLine("Writing INF1...");

            // Add INF1 to fileBuffer
            using (MemoryStream inf1Stream = new MemoryStream())
            {
                EndianBinaryWriter writer = new EndianBinaryWriter(inf1Stream, Endian.Big);
                inf.WriteINF1(writer, vtx.VertexPositions.Count);

                fileBuffer.AddRange(inf1Stream.ToArray());
            }

            Console.WriteLine("Writing VTX1...");

            // Add VTX1 to fileBuffer
            using (MemoryStream vtx1Stream = new MemoryStream())
            {
                EndianBinaryWriter writer = new EndianBinaryWriter(vtx1Stream, Endian.Big);
                vtx.WriteVTX1(writer);

                fileBuffer.AddRange(vtx1Stream.ToArray());
            }

            Console.WriteLine("Writing EVP1...");

            // Add EVP1 to fileBuffer
            using (MemoryStream evp1Stream = new MemoryStream())
            {
                EndianBinaryWriter writer = new EndianBinaryWriter(evp1Stream, Endian.Big);
                evp.WriteEVP1(writer);

                fileBuffer.AddRange(evp1Stream.ToArray());
            }

            Console.WriteLine("Writing DRW1...");

            // Add DRW1 to fileBuffer
            using (MemoryStream drw1Stream = new MemoryStream())
            {
                EndianBinaryWriter writer = new EndianBinaryWriter(drw1Stream, Endian.Big);
                drw.WriteDRW1(writer);

                fileBuffer.AddRange(drw1Stream.ToArray());
            }

            Console.WriteLine("Writing JNT1...");

            // Add JNT1 to fileBuffer
            using (MemoryStream jnt1Stream = new MemoryStream())
            {
                EndianBinaryWriter writer = new EndianBinaryWriter(jnt1Stream, Endian.Big);
                jnt.WriteJNT1(writer);

                fileBuffer.AddRange(jnt1Stream.ToArray());
            }

            Console.WriteLine("Writing SHP1...");

            // Add SHP1 to fileBuffer
            using (MemoryStream shp1Stream = new MemoryStream())
            {
                EndianBinaryWriter writer = new EndianBinaryWriter(shp1Stream, Endian.Big);
                shp.WriteSHP1(writer);

                fileBuffer.AddRange(shp1Stream.ToArray());
            }

            Console.WriteLine("Writing MAT3...");

            // Add MAT3 to fileBuffer
            using (MemoryStream mat3Stream = new MemoryStream())
            {
                EndianBinaryWriter writer = new EndianBinaryWriter(mat3Stream, Endian.Big);
                mat.WriteMAT3(writer);

                fileBuffer.AddRange(mat3Stream.ToArray());
            }

            Console.WriteLine("Writing TEX1...");

            // Add TEX1 to fileBuffer
            using (MemoryStream tex1Stream = new MemoryStream())
            {
                EndianBinaryWriter writer = new EndianBinaryWriter(tex1Stream, Endian.Big);
                tex.WriteTEX1(writer, mat.TextureList);

                fileBuffer.AddRange(tex1Stream.ToArray());
            }

            Console.WriteLine("Writing file size...");

            // Write file size
            using (MemoryStream sizeStream = new MemoryStream(fileBuffer.ToArray()))
            {
                EndianBinaryWriter writer = new EndianBinaryWriter(sizeStream, Endian.Big);
                writer.Seek(8, SeekOrigin.Begin);
                writer.Write((int)sizeStream.Length);

                fileBuffer.Clear(); // Clear original data

                fileBuffer.AddRange(sizeStream.ToArray()); // Add data with size field attatched
            }

            //string outPath = Path.Combine(Path.GetDirectoryName(inputFile), Path.GetFileNameWithoutExtension(inputFile) + ".bmd");

            // Write final file
            using (FileStream stream = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
            {
                EndianBinaryWriter writer = new EndianBinaryWriter(stream, Endian.Big);
                writer.Write(fileBuffer.ToArray());
            }

            Console.WriteLine("\nBMD written to {0}./nDone!", outputFile);
        }

        static Node GetSkeletonRoot(Scene scene)
        {
            // This will hold the names of the bones
            List<string> boneNames = new List<string>();

            // We're going to run through the meshes and see if they have bones.
            // If they do, we'll grab the names and put them in our list
            foreach (Mesh mesh in scene.Meshes)
            {
                if (mesh.HasBones)
                {
                    foreach (Bone bone in mesh.Bones)
                    {
                        if (!boneNames.Contains(bone.Name))
                            boneNames.Add(bone.Name);
                    }
                }
            }

            // This is the node that eventually leads to the skeleton's root
            // Assimp has a couple layers of transformation between the scene's root
            // and the skeleton's root
            Node boneTree = null;
            string rootBoneName = "";

            // The layers of translation for the skeleton have the first bone's name in them. So
            // We'll check each node in the scene against each bone name we got earlier
            // If we get a match, we've found the node that eventually reaches the boneTree!
            foreach (Node node in scene.RootNode.Children)
            {
                foreach (string name in boneNames)
                {
                    if (node.Name.Contains(name))
                    {
                        boneTree = node;
                        rootBoneName = name;
                    }
                }
            }

            if (boneTree == null)
            {
                // Something went wrong!
            }

            Node skeletonRoot = boneTree;

            // Run through the transformation layers until we find a node that has only the root bone's name
            while (true)
            {
                if (skeletonRoot.Name == rootBoneName)
                    break;
                skeletonRoot = skeletonRoot.Children[0];
            }

            // If the parent of the skeletonRoot has more than one child,
            // That means that the skeleton has multiple branches.
            // ...So, we're going to create a new world root and
            // add the children to it
            if (skeletonRoot.Parent.ChildCount > 1 && skeletonRoot.Parent.Name != "RootNode")
            {
                Node newWorldRoot = new Node("World_Root");
                newWorldRoot.Children.AddRange(skeletonRoot.Parent.Children.ToArray());

                skeletonRoot = newWorldRoot;
            }

            return skeletonRoot;
        }

        static void WriteHeader(EndianBinaryWriter writer)
        {
            writer.Write("J3D2bmd3".ToCharArray());
            writer.Write((int)0); // Placeholder for file size
            writer.Write((int)8); // Chunk count. For BMD this is 8, for BDL it's 9

            writer.Write("SVR3".ToCharArray()); // SVR3 is a dummy chunk. It's not included in the chunk count above
            for (int i = 0; i < 12; i++)
                writer.Write(byte.MaxValue); // Write the 12 0xFF bytes after SVR3's tag
        }

        static void DisplayUsageMessage()
        {
            Console.WriteLine("\nUniversal BMD Exporter v0.1 written by Sage_of_Mirrors\nConverts FBX/OBJ/DAE/etc models to BMD format.");
            Console.WriteLine("\nTwitter: @SageOfMirrors");
            Console.WriteLine("Github: Sage-of-Mirrors");
            Console.WriteLine("\nUsage: BMDExporter input_model_path [output_bmd_path]");
        }
    }
}
