using UnityEngine;

namespace Assets.Scripts
{
    public class BufferedBakedMesh
    {
        public VertexData[] Vertices;
        public int[] Indices;

        public BufferedBakedMesh(int vertexCount, int indexCount)
        {
            Debug.Log("BufferedBakedMesh created");
            Vertices = new VertexData[vertexCount];
            Debug.Log("Vertices array created with length of: " + Vertices.Length);
            Indices = new int[indexCount];
            Debug.Log("Indices array created with length of: " + Indices.Length);
        }
    }
}
