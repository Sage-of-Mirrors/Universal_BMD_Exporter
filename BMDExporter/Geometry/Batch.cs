using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assimp;

namespace BMDExporter.Geometry
{
    class Batch
    {
        public List<VertexAttributes> ActiveAttributes; // The attributes that the batch has
        public List<Vector3D> VertexPositions; // X/Y/Z data for vertexes. Any model will have these
        // The following are not required but often present
        public List<Color4D>[] VertexColors; // RGBA Color data for vertexes. There can be up to 2 sets.
        public List<Vector3D> VertexNormals; // Normal data for vertexes
        public List<Vector3D>[] VertexUVWs; // UVW data for vertexes. We're only going to use UV for the BMD though, and there can be up to 8 different sets.

        public Batch()
        {

        }

        public Batch(Mesh mesh)
        {
            if (!mesh.HasVertices)
            {
                // Mesh with no vertexes? Is that even a mesh?
                throw new ArgumentException(string.Format("Mesh {0} had no vertexes!", mesh.Name));
            }

            ActiveAttributes = new List<VertexAttributes>();
            VertexColors = new List<Color4D>[2] { new List<Color4D>(), new List<Color4D>() };
            VertexNormals = new List<Vector3D>();
            VertexUVWs = new List<Vector3D>[8] { new List<Vector3D>(), new List<Vector3D>(), new List<Vector3D>(), new List<Vector3D>(),
                                                 new List<Vector3D>(), new List<Vector3D>(), new List<Vector3D>(), new List<Vector3D>()};

            ActiveAttributes.Add(VertexAttributes.Position);
            VertexPositions = mesh.Vertices;

            if (mesh.HasNormals)
            {
                ActiveAttributes.Add(VertexAttributes.Normal);
                VertexNormals = mesh.Normals;
            }

            for (int i = 0; i < 2; i++)
            {
                if (mesh.VertexColorChannels[i].Count != 0)
                {
                    ActiveAttributes.Add(VertexAttributes.Color0 + i);
                    VertexColors[i] = mesh.VertexColorChannels[i];
                }
            }

            for (int i = 0; i < mesh.TextureCoordinateChannelCount; i++)
            {
                ActiveAttributes.Add(VertexAttributes.Tex0 + i);
                VertexUVWs[i] = mesh.TextureCoordinateChannels[i];
            }
        }
    }
}
