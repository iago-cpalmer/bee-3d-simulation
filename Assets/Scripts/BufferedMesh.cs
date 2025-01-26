using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using System;

namespace Assets.Scripts
{
    public class BufferedMesh
    {
        public static int VertexCount { get { return _vertexCount; } }
        public static int IndexCount { get { return _indexCount; } }
        private static VertexData[] _vertexDataArray = new VertexData[ushort.MaxValue];
        private static int[] _indices = new int[ushort.MaxValue * 3];
        public static readonly int LOD_COUNT = 3;
        private static int _vertexCount;
        private static int _indexCount;

        private int _maxVertexCount, _maxIndexCount;
        public Mesh Mesh;

        public BufferedMesh(int vertexCount, int indexCount)
        {
            _maxVertexCount = vertexCount;
            _maxIndexCount = indexCount;
            Mesh = new Mesh();
            VertexAttributeDescriptor[] vertexAttributeDescriptors = new[]
            {
                new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3),
                new VertexAttributeDescriptor(VertexAttribute.Normal, VertexAttributeFormat.Float32, 3),
                new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float32, 2),
            };
            Mesh.SetVertexBufferParams(_maxVertexCount, vertexAttributeDescriptors);
            Mesh.SetIndexBufferParams(_maxIndexCount, IndexFormat.UInt32);
            Mesh.subMeshCount = LOD_COUNT;
            for (int i = 0; i < LOD_COUNT; i++)
            {
                SubMeshDescriptor desc = new SubMeshDescriptor();
                desc.baseVertex = 0;
                desc.firstVertex = 0;
                desc.indexCount = 0;
                desc.indexStart = 0;
                desc.topology = MeshTopology.Triangles;
                desc.vertexCount = 0;
                Mesh.SetSubMesh(i, desc, MeshUpdateFlags.DontRecalculateBounds | MeshUpdateFlags.DontValidateIndices | MeshUpdateFlags.DontNotifyMeshUsers);
            }
        }

        public static void AddVertex(VertexData vertex)
        {
            if (_vertexCount >= _vertexDataArray.Length)
            {
                // Create new array
                VertexData[] aux = new VertexData[_vertexDataArray.Length + _vertexDataArray.Length / 2];
                Array.Copy(_vertexDataArray, aux, _vertexDataArray.Length);
                _vertexDataArray = aux;
            }
            _vertexDataArray[_vertexCount++] = vertex;

        }

        public static void AddIndex(int i)
        {
            if (_indexCount >= _indices.Length - 3)
            {
                int[] aux = new int[_indices.Length + ushort.MaxValue * 3];
                Array.Copy(_indices, aux, _indices.Length);
                _indices = aux;
            }
            _indices[_indexCount++] = i;
        }

        public static void AddTriangle(int i1, int i2, int i3)
        {
            if (_indexCount >= _indices.Length - 3)
            {
                int[] aux = new int[_indices.Length + ushort.MaxValue * 3];
                Array.Copy(_indices, aux, _indices.Length);
                _indices = aux;
            }
            _indices[_indexCount++] = i1;
            _indices[_indexCount++] = i2;
            _indices[_indexCount++] = i3;
        }

        public void UploadMeshData()
        {
            Mesh.SetVertexBufferData(_vertexDataArray, 0, 0, _maxVertexCount, 0, (MeshUpdateFlags.DontNotifyMeshUsers | MeshUpdateFlags.DontRecalculateBounds | MeshUpdateFlags.DontValidateIndices));
            Mesh.SetIndexBufferData(_indices, 0, 0, _maxIndexCount, (MeshUpdateFlags.DontNotifyMeshUsers | MeshUpdateFlags.DontRecalculateBounds | MeshUpdateFlags.DontValidateIndices));
            SubMeshDescriptor subMeshDescriptor = new SubMeshDescriptor();
            subMeshDescriptor.baseVertex = 0;
            subMeshDescriptor.indexStart = 0;
            subMeshDescriptor.indexCount = _maxIndexCount;
            subMeshDescriptor.topology = MeshTopology.Triangles;
            subMeshDescriptor.firstVertex = 0;

            Bounds bounds = new Bounds(new float3(Globals.CHUNK_SIZE / 2, 200, Globals.CHUNK_SIZE / 2), new float3(Globals.CHUNK_SIZE, 400, Globals.CHUNK_SIZE));

            subMeshDescriptor.bounds = bounds;

            Mesh.SetSubMesh(0, subMeshDescriptor, MeshUpdateFlags.DontRecalculateBounds | MeshUpdateFlags.DontValidateIndices | MeshUpdateFlags.DontNotifyMeshUsers);
            Mesh.bounds = bounds;
            Mesh.RecalculateNormals();
            ClearMesh();
        }


        public static void ClearMesh()
        {
            _indexCount = 0;
            _vertexCount = 0;
        }

    }
}
