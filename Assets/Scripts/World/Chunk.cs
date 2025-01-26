using Assets.Scripts;
using Assets.Scripts.World;
using Assets.Scripts.World.WorldGenerators;
using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public struct Chunk
{
    public bool IsGenerated;
    public float3 Position;
    public int2 ChunkCoord;
    public BufferedChunkMesh[] ChunkMesh;
    public Matrix4x4[][] GrassLODPositions;
    public WorldObjectDataInstance[] DecorationObjects;
    public float[] ChunkData;
    public Texture2D Texture;

    public FlowerData[] Flowers;
    public Matrix4x4[] FlowerMatrices;
    public int FlowerType;
    public int PollenLeft;
    public int NumberOfFlowers;

    public Chunk(BeeWorld world, float3 position, NormalChunkGenerator chunkGenerator)
    {
        Position = position;
        Position.y = 0;
        ChunkMesh = new BufferedChunkMesh[BufferedMesh.LOD_COUNT];
        IsGenerated = false;
        ChunkData = new float[Globals.CHUNK_SIZE * Globals.CHUNK_SIZE];
        ChunkCoord = BeeWorld.WorldToChunk(position);
        Texture = new Texture2D(Globals.CHUNK_SIZE, Globals.CHUNK_SIZE);
        GrassLODPositions = new Matrix4x4[BufferedMesh.LOD_COUNT-1][];
        GrassLODPositions[0] = new Matrix4x4[2048];
        GrassLODPositions[1] = new Matrix4x4[512];
        DecorationObjects = new WorldObjectDataInstance[UnityEngine.Random.Range(0,Globals.MAX_DECORATION_OBJECTS_PER_CHUNK+1)];
        NumberOfFlowers = (UnityEngine.Random.Range(0.0f, 1.0f) >= 0.75f) ? UnityEngine.Random.Range(10,30): 0;
        FlowerType = UnityEngine.Random.Range(0, 2);
        Flowers = new FlowerData[NumberOfFlowers];
        FlowerMatrices = new Matrix4x4[NumberOfFlowers];
        Array.Fill(Flowers, new FlowerData { GlobalPosition = new float3(-1, -1, -1) });
        PollenLeft = UnityEngine.Random.Range(Globals.MIN_POLEN_AMOUNT, Globals.MAX_POLEN_AMOUNT);
        //DecorationObjects = new WorldObjectDataInstance[1];
        Init(world, chunkGenerator);
        IsGenerated = true;
    }

    private void Init(BeeWorld world, NormalChunkGenerator chunkGenerator)
    {
        if (IsGenerated)
            return;
        //Debug.Log("Generating chunk: " + ChunkCoord);
        // Procedural terrain generation
        // Polen sources generation
        // Procedurally generate chunk terrain
        chunkGenerator.GenerateChunk(ChunkCoord.x, ChunkCoord.y, ref this);
        // Generate meshes
        GenerateMesh(world, 0);
        GenerateMesh(world, 1);
        GenerateMesh(world, 2);
        
        GenerateGrassMesh(world, 0);
        GenerateGrassMesh(world, 1);
        
        GenerateDecoration(world);
        GenerateFlower(world);
    }

    private void GenerateMesh(BeeWorld world, int lod)
    {
        int lodIncrement = 1;
        if (lod == 0)
        {
            lodIncrement = 1;
        }
        else if (lod == 1)
        {
            lodIncrement = 4;
        }
        else if (lod == 2)
        {
            lodIncrement = 16;
        }
        ChunkMesh[lod] = new BufferedChunkMesh(lodIncrement);

        int vertexIndex = 0;
        int x = 0;
        int z = 0;
        // Generate mesh
        // All mesh 
        for (int i = 0; i <= Globals.CHUNK_SIZE; i += lodIncrement)
        {
            for (int j = 0; j <= Globals.CHUNK_SIZE; j += lodIncrement)
            {
                if (i < Globals.CHUNK_SIZE && j < Globals.CHUNK_SIZE)
                {
                    float3 p1 = new float3(i, GetHeightAt(i, j), j);
                    float3 p2 = new float3(i, world.GetHeightAt(i, j+1, ChunkCoord), j + 1);
                    float3 p3 = new float3(i+1, world.GetHeightAt(i+1, j, ChunkCoord), j);

                    float3 dir1 = p2 - p1;
                    float3 dir2 = p3 - p1;

                    float3 normal = math.normalize(math.cross(dir1, dir2));

                    VertexData v1 = new VertexData
                    {
                        Position = p1,
                        Normal = normal,
                        UV = new float2(x % 2 == 0 ? 0 : 1, z % 2 == 0 ? 0 : 1)
                    };
                    ChunkMesh[lod].AddVertex(v1, lod);
                    ChunkMesh[lod].AddTriangleIndices((ushort)(vertexIndex), (ushort)(vertexIndex + Globals.CHUNK_SIZE / lodIncrement + 2), (ushort)(vertexIndex + Globals.CHUNK_SIZE / lodIncrement + 1));
                    ChunkMesh[lod].AddTriangleIndices((ushort)(vertexIndex + Globals.CHUNK_SIZE / lodIncrement + 2), (ushort)(vertexIndex), (ushort)(vertexIndex + 1));
                }
                else
                {
                    float3 p1 = new float3(i, world.GetHeightAt(i, j, ChunkCoord), j);
                    float3 p2 = new float3(i, world.GetHeightAt(i, j+1, ChunkCoord), j + 1);
                    float3 p3 = new float3(i + 1, world.GetHeightAt(i+1, j, ChunkCoord), j);

                    float3 dir1 = p2 - p1;
                    float3 dir2 = p3 - p1;

                    float3 normal = math.normalize(math.cross(dir1, dir2));
                    VertexData v1 = new VertexData
                    {
                        Position = p1,
                        Normal = normal,
                        UV = new float2(x % 2 == 0 ? 0 : 1, z % 2 == 0 ? 0 : 1)
                    };
                    ChunkMesh[lod].AddVertex(v1, lod);
                }
                vertexIndex++;
                z++;
            }
            x++;
        }

        ChunkMesh[lod].UploadMeshData();
    }


    private void GenerateGrassMesh(BeeWorld world, int lod)
    {
        float lodIncrement = 1 * (lod+1);
        int i = 0;
        for (float x = 0; x < Globals.CHUNK_SIZE; x += lodIncrement)
        {
            for (float z = 0; z < Globals.CHUNK_SIZE; z += lodIncrement)
            {
                float scaleHorizontal = UnityEngine.Random.Range(100, 150);
                float yRot = UnityEngine.Random.Range(0,360);
                GrassLODPositions[lod][i++] = Matrix4x4.Translate(new float3(x + Position.x,GetHeightAt((int)x, (int)z) + 0.5f, z + Position.z)) * Matrix4x4.Scale(new float3(scaleHorizontal)) 
                    *  Matrix4x4.Rotate(Quaternion.Euler(0, yRot, 0));
            }
        }
    }
    
    private void GenerateDecoration(BeeWorld world) { 
        for(int i = 0; i < DecorationObjects.Length; i++)
        {
            int x = UnityEngine.Random.Range(0, Globals.CHUNK_SIZE);
            int z = UnityEngine.Random.Range(0, Globals.CHUNK_SIZE);
            byte id = (byte)UnityEngine.Random.Range(0, world.RendererData.DecorationalObjects.Length-1);
            float scale = UnityEngine.Random.Range(1, 1.5f) * world.RendererData.DecorationalObjects[id].ScaleMultiplier;
            float yRot = UnityEngine.Random.Range(0, 360);
            DecorationObjects[i] = new WorldObjectDataInstance
            {
                worldObjectId = id,
                TransformMatrix = new Matrix4x4[]{Matrix4x4.Translate(new float3(x + Position.x, GetHeightAt(x, z), z + Position.z)) * Matrix4x4.Scale(new float3(scale,  scale, scale))
                * Matrix4x4.Rotate(Quaternion.Euler(-90, yRot, 0)) }
            };

        }
    }

    private void GenerateFlower(BeeWorld world)
    {
        for(int i = 0; i < NumberOfFlowers; i++)
        {
            int x = UnityEngine.Random.Range(0, Globals.CHUNK_SIZE);
            int z = UnityEngine.Random.Range(0, Globals.CHUNK_SIZE);
            float y = GetHeightAt(x, z) + 0.5f;
            float scale = UnityEngine.Random.Range(1, 1.5f) * world.RendererData.DecorationalObjects[1].ScaleMultiplier;
            float yRot = UnityEngine.Random.Range(0, 360);
            //world.SpawnerSystem.AddSpawnRequest(new SpawnRequestBufferElement { Position = BeeWorld.GetWorldFromLocalPosition(ChunkCoord, new float3(x, y, z)), Type = (((int)y)%2==0?0:1)});
            Flowers[i] = new FlowerData
            {
                GlobalPosition = new float3(x, y, z) + Position,
                Nectar = UnityEngine.Random.Range(Globals.MIN_POLEN_AMOUNT, Globals.MAX_POLEN_AMOUNT),
                Pollen = UnityEngine.Random.Range(Globals.MIN_POLEN_AMOUNT, Globals.MAX_POLEN_AMOUNT)
            };
            FlowerMatrices[i] = Matrix4x4.Translate(Flowers[i].GlobalPosition) * Matrix4x4.Scale(new float3(scale, scale, scale))
                    * Matrix4x4.Rotate(Quaternion.Euler(0, yRot, 0));
        }
        
    }


    private int GetIndexFromLocalCoord(int x, int z)
    {
        return z << 5 | x;
    }

    public void SetHeightAt(int x, int z, float height)
    {
        ChunkData[GetIndexFromLocalCoord(x, z)] = height;
    }
    public float GetHeightAt(int x, int z)
    {
        return ChunkData[GetIndexFromLocalCoord(x, z)];
    }
}
