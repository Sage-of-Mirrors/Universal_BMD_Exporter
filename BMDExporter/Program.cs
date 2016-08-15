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
            string inputFile = @"C:\Program Files (x86)\SZS Tools\Amori.fbx";

            List<Batch> Batches = new List<Batch>(); // A list of the meshes in the scene

            AssimpContext cont = new AssimpContext();
            Scene scene = cont.ImportFile(inputFile);

            foreach (Mesh mesh in scene.Meshes)
                Batches.Add(new Batch(mesh));

            VTX1 vtx = new VTX1(Batches);

            SHP1 shp = new SHP1(Batches);

            EVP1 evp = new EVP1(Batches);

            DRW1 drw = new DRW1(Batches);

            using (FileStream stream = new FileStream(@"C:\Program Files (x86)\SZS Tools\evp1test.bin", FileMode.Create, FileAccess.Write))
            {
                EndianBinaryWriter writer = new EndianBinaryWriter(stream, Endian.Big);
                drw.WriteDRW1(writer);
            }
        }
    }
}
