using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

namespace Assets.Scripts
{
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct VertexData
    {
        public float3 Position;
        public float3 Normal;
        public float2 UV;
    }
    
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct PackedVertex
    {
        public uint Data;
    }
    public class BufferedChunkMesh
    {
        public static int VertexCount {get {return _vertexCount;} }
        public static PackedVertex[] _vertexDataArray = new PackedVertex[(Globals.CHUNK_SIZE+1) * (Globals.CHUNK_SIZE + 1)];
        public static ushort[] _indices = new ushort[((Globals.CHUNK_SIZE+1)) * ((Globals.CHUNK_SIZE + 1)) * 6];

        private static int _vertexCount;
        private static int _indexCount;

        public Mesh Mesh;

        public BufferedChunkMesh(int lodIncrement)
        {
            Mesh = new Mesh();
            VertexAttributeDescriptor[] vertexAttributeDescriptors = new[]
            {
                new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.UInt32, 1),
            };
            if(lodIncrement>1)
            {
                Mesh.SetVertexBufferParams((_vertexDataArray.Length / lodIncrement), vertexAttributeDescriptors);
                Mesh.SetIndexBufferParams(_indices.Length / lodIncrement, IndexFormat.UInt16);
            } else
            {
                Mesh.SetVertexBufferParams(_vertexDataArray.Length / lodIncrement, vertexAttributeDescriptors);
                Mesh.SetIndexBufferParams(_indices.Length / lodIncrement, IndexFormat.UInt16);
            }
           
            Mesh.subMeshCount = 1;
            for (int i = 0; i < 1; i++)
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

        public PackedVertex PackVertex(VertexData vertex, int lod)
        {
            PackedVertex packed = new PackedVertex();
            uint plod = (uint)(lod & 3);
            uint py = (uint)math.floor(vertex.Position.y * 10) & 1023;

            uint nx = (uint)math.floor(math.abs(vertex.Normal.x) * 10.0f) & 15;
            uint ny = (uint)math.floor(math.abs(vertex.Normal.y) * 10.0f) & 15;
            uint nz = (uint)math.floor(math.abs(vertex.Normal.z) * 10.0f) & 15;
            uint sx = (uint)(vertex.Normal.x > 0 ? 1 : 0);
            uint sz = (uint)(vertex.Normal.z > 0 ? 1 : 0);
            packed.Data = plod | (py << 2) | (nx << 12) | (ny << 16) | (nz << 20) | (sx << 24) | (sz<< 25);
            return packed;
        }

        public void AddVertex(VertexData vertexData, int lod)
        {
            _vertexDataArray[_vertexCount++] = PackVertex(vertexData, lod);
        }

        public void AddTriangleIndices(ushort i1, ushort i2, ushort i3)
        {
            _indices[_indexCount++] = i1;
            _indices[_indexCount++] = i2;
            _indices[_indexCount++] = i3;
        }
        public void UploadMeshData()
        {
            Mesh.SetVertexBufferData(_vertexDataArray, 0, 0, _vertexCount, 0, (MeshUpdateFlags.DontNotifyMeshUsers | MeshUpdateFlags.DontRecalculateBounds | MeshUpdateFlags.DontValidateIndices));
            Mesh.SetIndexBufferData(_indices, 0, 0, _indexCount, (MeshUpdateFlags.DontNotifyMeshUsers | MeshUpdateFlags.DontRecalculateBounds | MeshUpdateFlags.DontValidateIndices));
            SubMeshDescriptor subMeshDescriptor = new SubMeshDescriptor();
            subMeshDescriptor.baseVertex = 0;
            subMeshDescriptor.indexStart = 0;
            subMeshDescriptor.indexCount = _indexCount;
            subMeshDescriptor.topology = MeshTopology.Triangles;
            subMeshDescriptor.firstVertex = 0;

            Bounds bounds = new Bounds(new float3(Globals.CHUNK_SIZE / 2, 200, Globals.CHUNK_SIZE / 2), new float3(Globals.CHUNK_SIZE, 400, Globals.CHUNK_SIZE));

            subMeshDescriptor.bounds = bounds;

            Mesh.SetSubMesh(0, subMeshDescriptor, MeshUpdateFlags.DontRecalculateBounds | MeshUpdateFlags.DontValidateIndices | MeshUpdateFlags.DontNotifyMeshUsers);
            Mesh.bounds = bounds;
            //Mesh.RecalculateNormals();
            ClearMesh();
        }
    
        public void ClearMesh()
        {
            _vertexCount = 0;
            _indexCount = 0;
        }
    }
}
