using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assimp;
using OpenTK;

namespace BMDExporter.Geometry
{
    class Batch
    {
        public string Name;
        public List<VertexAttributes> ActiveAttributes; // The attributes that the batch has
        public List<Vector3D> VertexPositions; // X/Y/Z data for vertexes. Any model will have these
        // The following are not required but often present
        public List<Color4D>[] VertexColors; // RGBA Color data for vertexes. There can be up to 2 sets.
        public List<Vector3D> VertexNormals; // Normal data for vertexes
        public List<Vector3D>[] VertexUVWs; // UVW data for vertexes. We're only going to use UV for the BMD though, and there can be up to 8 different sets.
        public List<Face> Faces; // Vertex index data for faces
        public List<short> FaceIndexes;

        public float BoundingSphereRadius; // Radius of the bounding sphere for this batch
        public Vector3D BoundingMin; // Minimum of bounding box
        public Vector3D BoundingMax; // Maximum of bounding box

        public int MaterialIndex;

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

            Name = mesh.Name;
            MaterialIndex = mesh.MaterialIndex;

            ActiveAttributes = new List<VertexAttributes>();
            VertexColors = new List<Color4D>[2] { new List<Color4D>(), new List<Color4D>() };
            VertexNormals = new List<Vector3D>();
            VertexUVWs = new List<Vector3D>[8] { new List<Vector3D>(), new List<Vector3D>(), new List<Vector3D>(), new List<Vector3D>(),
                                                 new List<Vector3D>(), new List<Vector3D>(), new List<Vector3D>(), new List<Vector3D>()};
            Faces = new List<Face>();
            FaceIndexes = new List<short>();

            ActiveAttributes.Add(VertexAttributes.Position);
            VertexPositions = mesh.Vertices;

            // Most modern modeling programs have the Z-axis as up.
            // This matrix will rotate the vertexes so that the Y-axis is up.
            Matrix4 rotateMat = Matrix4.CreateTranslation(0f, 0f, 0f)
                              * Matrix4.CreateFromAxisAngle(Vector3.UnitX, (float)-Math.PI / 2)
                              * Matrix4.CreateScale(1f, 1f, 1f);

            for (int i = 0; i < VertexPositions.Count; i++)
            {
                Vector3 tkVec = new Vector3(VertexPositions[i].X, VertexPositions[i].Y, VertexPositions[i].Z);
                tkVec = Vector3.Transform(tkVec, rotateMat);

                VertexPositions[i] = new Vector3D(tkVec.X, tkVec.Y, tkVec.Z);
            }

            Faces = mesh.Faces;

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

            ActiveAttributes.Add(VertexAttributes.NullAttr);

            ActiveAttributes.Sort();

            // Let's get the bounding info

            float MinX = float.MaxValue;
            float MinY = float.MaxValue;
            float MinZ = float.MaxValue;

            float MaxX = float.MinValue;
            float MaxY = float.MinValue;
            float MaxZ = float.MinValue;

            foreach (Vector3D vec in VertexPositions)
            {
                if (vec.X > MaxX)
                    MaxX = vec.X;
                if (vec.X < MinX)
                    MinX = vec.X;

                if (vec.Y > MaxY)
                    MaxY = vec.Y;
                if (vec.Y < MinY)
                    MinY = vec.Y;

                if (vec.Z > MaxZ)
                    MaxZ = vec.Z;
                if (vec.Z < MinZ)
                    MinZ = vec.Z;
            }

            BoundingMin = new Vector3D(MinX, MinY, MinZ);
            BoundingMax = new Vector3D(MaxX, MaxY, MaxZ);

            Vector3D boxCenter = new Vector3D(MinX + MaxX / 2, MinY + MaxY / 2, MinZ + MaxZ / 2);

            BoundingSphereRadius = (boxCenter - BoundingMin).Length();

            float maxFromCenter = (boxCenter - BoundingMax).Length();
            if (maxFromCenter > BoundingSphereRadius)
                BoundingSphereRadius = maxFromCenter;
        }
    }
}
