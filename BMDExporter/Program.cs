using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BMDExporter.Geometry;
using Assimp;

namespace BMDExporter
{
    class Program
    {
        static void Main(string[] args)
        {
            string inputFile = @"C:\Program Files (x86)\SZS Tools\MazeRoom\MazeModel.obj";

            List<Batch> Batches = new List<Batch>(); // A list of the meshes in the scene

            AssimpContext cont = new AssimpContext();
            Scene scene = cont.ImportFile(inputFile);

            foreach (Mesh mesh in scene.Meshes)
                Batches.Add(new Batch(mesh));

            foreach (Batch bat in Batches)
            {
                Console.WriteLine(string.Format("Batch count: {0}", Batches.Count));
                Console.WriteLine(string.Format("Number of vertexes: {0}\nNumber of vertex colors: {1}\nNumber of normals: {2}\nNumber of UVs: {3}\n\n",
                    bat.VertexPositions.Count, bat.VertexColors[0].Count, bat.VertexNormals.Count, bat.VertexUVWs[0].Count));
            }

            Console.ReadLine();
        }
    }
}
