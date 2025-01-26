using Assets.Scripts;
using Assets.Scripts.Player;
using Assets.Scripts.Systems;
using Assets.Scripts.World;
using Assets.Scripts.World.WorldGenerators;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
/*
public class WorldStatic
{
    public static readonly SharedStatic<BeeWorld> Instance = SharedStatic<BeeWorld>.GetOrCreate<WorldStatic, BeeWorld>();
}*/
public class BeeWorld : MonoBehaviour
{
    public static BeeWorld Instance;

    public HandleScans SpawnerSystem;
    public NestSystem NestSystem;

    public Matrix4x4[] PolenParticleMatrices;
    public int PolenParticleEmitters;

    public int Seed { get { return seed; } }
    public NormalChunkGenerator ChunkGenerator { get { return _chunkGenerator; } }

    public float3 NestPosition;

    [Header("World settings")]
    [SerializeField] private int seed;

    [Space(10)]
    [SerializeField] public RendererData RendererData;

    [Space(10)]
    [Header("Player")]
    public Transform PlayerTransform;
    [SerializeField] private SpectatorController spectatorController;

    private Chunk[] _chunks;
    private Queue<int2> _chunksToGenerate;
    private bool _isCorroutineCreatingChunks;
    private NormalChunkGenerator _chunkGenerator;

    // Player related variables
    private int2 _playerLastChunkCoords = new int2(2, 2);

    private int2[] _visibleChunks;

    private float _timeToRegen;
    /////////////////////////////////////////////////////////////////
    /// GAME OBJECT LIFE CYCLE FUNCTIONS
    /////////////////////////////////////////////////////////////////


    public void Awake()
    {
        Globals.SEED = seed;
        Instance = this;
        UnityEngine.Random.InitState((int)Globals.SEED);
        SpawnerSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<HandleScans>();
        NestSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<NestSystem>();
        QualitySettings.maxQueuedFrames = 10;
        PolenParticleMatrices = new Matrix4x4[100];
    }
    private void Start()
    {
        RendererData.Init();
        _chunks = new Chunk[Globals.WORLD_SIZE * Globals.WORLD_SIZE];
        _visibleChunks = new int2[Globals.VIEW_CHUNK_RANGE * Globals.VIEW_CHUNK_RANGE];
        _chunksToGenerate = new Queue<int2>();
        _chunkGenerator = new NormalChunkGenerator(this);
        //FlowerPositions = 
        StartCoroutine(InstantiateChunks());

        for (int i = 0; i < Globals.WORLD_SIZE; i++)
        {
            for (int j = 0; j < Globals.WORLD_SIZE; j++)
            {
                int2 coord = new int2(i, j);
                if (ChunkInWorldRange(coord) && !GetChunk(coord).IsGenerated)
                {
                    _chunksToGenerate.Enqueue(coord);
                }
            }
        }
        if (_chunksToGenerate.Count > 0 & !_isCorroutineCreatingChunks)
        {
            StartCoroutine(InstantiateChunks());
        }
    }
    IEnumerator RegenChunks()
    {
        for(int i = 0; i < _chunks.Length; i++)
        {
            if(_chunks[i].IsGenerated)
            {
                //_chunks[i].PollenLeft =
                for(int f = 0; f < _chunks[i].Flowers.Length; f++) {
                    _chunks[i].Flowers[f].Pollen = UnityEngine.Random.Range(Globals.MIN_POLEN_AMOUNT, Globals.MAX_POLEN_AMOUNT);
                    _chunks[i].Flowers[f].Nectar = UnityEngine.Random.Range(Globals.MIN_POLEN_AMOUNT, Globals.MAX_POLEN_AMOUNT);
                    yield return null;
                }
            }
        }
    }

    IEnumerator InstantiateChunks()
    {
        _isCorroutineCreatingChunks = true;
        while (_chunksToGenerate.Count>0)
        {
            int2 coord = _chunksToGenerate.Dequeue();
            int i = GetIndexFromCoord(coord);
            _chunks[i] = new Chunk(this, new float3(coord.x * Globals.CHUNK_SIZE, 0, coord.y * Globals.CHUNK_SIZE), _chunkGenerator);
            yield return null;
        }
        _isCorroutineCreatingChunks = false;
    }
    private void Update()
    {
        // Check if player has changed its chunk coordinates
        int2 playerCurrentChunkCoords = WorldToChunk(PlayerTransform.position);
        if(!playerCurrentChunkCoords.Equals(_playerLastChunkCoords))
        {
            // Player has changed its chunk coordinates
            _playerLastChunkCoords = playerCurrentChunkCoords;
            // Update visible chunks
            int i = 0;
            for(int x = -Globals.VIEW_CHUNK_RANGE/2; x < Globals.VIEW_CHUNK_RANGE/2; x++)
            {
                for (int z = -Globals.VIEW_CHUNK_RANGE/2; z < Globals.VIEW_CHUNK_RANGE/2; z++)
                {
                    _visibleChunks[i] = _playerLastChunkCoords + new int2(x, z);
                    i++;
                }
            }
        }

        _timeToRegen += Time.deltaTime;
        if (_timeToRegen >= 300)
        {
            _timeToRegen = 0;
            StartCoroutine(RegenChunks());
        }
        
    }

    private void LateUpdate()
    {
        SelectionHandler.Instance.MyLateUpdate();
        spectatorController.MyLateUpdate();
        RenderChunks();
        RendererData.RenderParticles(0, ref PolenParticleMatrices, PolenParticleEmitters);
        spectatorController.MyLateUpdate();
    }

    private void RenderChunks()
    {
        for (int i = 0; i < _visibleChunks.Length; i++)
        {
            if (ChunkInWorldRange(_visibleChunks[i]) && GetChunk(_visibleChunks[i]).IsGenerated)
            {
                Chunk chunk = GetChunk(_visibleChunks[i]);
                //Graphics.RenderMesh(rendererData.RenderParams[0], GetChunk(_visibleChunks[i]).ChunkMesh.Mesh, 0, Matrix4x4.Translate(GetChunk(_visibleChunks[i]).Position));
                float3 playerPos = PlayerTransform.position;
                playerPos.y = 0;
                float dist = math.distancesq(chunk.Position + new float3(Globals.CHUNK_SIZE / 2, 0, Globals.CHUNK_SIZE / 2), playerPos);
                int lod = 15;

                if (dist < 4096)
                {
                    lod = 0;
                }
                else if (dist < 24336)
                {
                    lod = 1;
                }
                else
                {
                    lod = 2;
                }
                Graphics.RenderMesh(RendererData.RenderParams[lod], chunk.ChunkMesh[lod].Mesh, 0, Matrix4x4.Translate(chunk.Position));
                
                // Render decoration
                if (dist < 2304)
                {
                    // lod 0

                    lod = 0;
                }
                else if (dist < 16384)
                {
                    // lod 1
                    lod = 1;
                }
                else if (dist < 65536)
                {
                    // lod 2
                    lod = 2;
                }
                else
                {
                    continue;
                }
                // Render grass
                if (lod < 2)
                {
                    RendererData.RenderInstance(RendererData.GrassLODMeshes[lod], 3, ref chunk.GrassLODPositions[lod]);
                    if(chunk.NumberOfFlowers>0)
                    {
                        RendererData.RenderInstance(RendererData.DecorationalObjects[1].LODs[lod], 6+chunk.FlowerType, ref chunk.FlowerMatrices);
                    }
                    
                }
                if (lod < 3)
                {
                    // Render decorational objects
                    for (int j = 0; j < chunk.DecorationObjects.Length; j++)
                    {
                        for (int l = 0; l < RendererData.DecorationalObjects[chunk.DecorationObjects[j].worldObjectId].LODs.Length / 3; l++)
                        {
                            RendererData.RenderInstance(RendererData.DecorationalObjects[chunk.DecorationObjects[j].worldObjectId].LODs[l * 3 + lod], chunk.DecorationObjects[j].worldObjectId + 4 + l, ref chunk.DecorationObjects[j].TransformMatrix);
                        }
                    }
                }
                

            }
        }
    }

    private void OnPreRender()
    {
        
    }


    /////////////////////////////////////////////////////////////////
    /// AUXILIARY & UTILITY FUNCTIONS
    ///////////////////////////////////////////////////////////////// 

    /// <summary>
    /// From a world position, get the correspondent chunk
    /// </summary>
    /// <param name="position">Position in world</param>
    /// <returns>Chunk coordinate</returns>
    public static int2 WorldToChunk(float3 position)
    {
        return new int2((int)(position.x / Globals.CHUNK_SIZE), (int)(position.z / Globals.CHUNK_SIZE));
    }

    /// <summary>
    /// Get local position in a chunk by the world position
    /// </summary>
    /// <param name="position">World position</param>
    /// <returns>Local position in chunk</returns>
    public float3 WorldToLocal(float3 position)
    {
        return new float3(position.x % Globals.CHUNK_SIZE, position.y, position.z % Globals.CHUNK_SIZE);
    }
    public int GetIndexFromCoord(int2 coord)
    {
        return GetIndexFromCoord(coord.x, coord.y);
    }
    public int GetIndexFromCoord(int x, int y)
    {
        return y << Globals.BIT_CHUNK_COORD_OFFSET | x;
    }

    public int2 GetChunkCoord(int index)
    {
        return new int2(index & Globals.BIT_MASK_CHUNK_COORD, (index >> Globals.BIT_CHUNK_COORD_OFFSET) & Globals.BIT_MASK_CHUNK_COORD);
    }
    /// <summary>
    /// </summary>
    /// <param name="coord">Coordinate of chunk</param>
    /// <returns>Reference to the specified chunk</returns>
    public ref Chunk GetChunk(int2 coord)
    {
        return ref _chunks[GetIndexFromCoord(coord)];
    }

    public ref Chunk GetChunk(int x, int z)
    {
        return ref _chunks[GetIndexFromCoord(x,z)];
    }

    /// <summary>
    /// </summary>
    /// <param name="position">World position</param>
    /// <returns>Reference to the chunk given a world position</returns>
    public ref Chunk GetChunkFromWorldPosition(float3 position)
    {
        return ref GetChunk(WorldToChunk(position));

    }
    /// <summary>
    /// </summary>
    /// <param name="coord"></param>
    /// <returns>Whether a given chunk coordinate is inside world boundaries</returns>
    public static bool ChunkInWorldRange(int2 coord)
    {
        return coord.x >= 0 && coord.y >= 0 && coord.x < Globals.WORLD_SIZE && coord.y < Globals.WORLD_SIZE;
    }

    public static float3 WorldToRelative(float3 worldPosition, int2 chunkCoord)
    {
        float x = worldPosition.x - chunkCoord.x * Globals.CHUNK_SIZE;
        float y = worldPosition.y;
        float z = worldPosition.z - chunkCoord.y * Globals.CHUNK_SIZE;
        return new float3(x, y, z);
    }

    /// <summary>
    /// </summary>
    /// <param name="worldObject"></param>
    /// <returns>True if the world object is equal to NULL_WORLD_OBJECT</returns>
    public bool IsWorldObjectNull(WorldObject worldObject)
    {
        //return worldObject.Type == NULL_WORLD_OBJECT.Type;
        return true;
    }

    public static float3 GetWorldFromLocalPosition(int2 chunk, float3 local)
    {
        local.x += chunk.x * Globals.CHUNK_SIZE;
        local.z += chunk.y * Globals.CHUNK_SIZE;
        return local;
    }
    /// <summary>
    /// Gets the height at the local coords from a chunk
    /// </summary>
    /// <param name="x">Local x coord in chunk</param>
    /// <param name="z">Local z coord in chunk</param>
    /// <param name="chunkCoord">Chunk</param>
    /// <returns></returns>
    public float GetHeightAt(int x, int z, int2 chunkCoord)
    {
        int2 newChunk = (int2)(chunkCoord +  math.sign(new int2(x, z)));
        if(ChunkInWorldRange(newChunk))
        {
            int index = GetIndexFromCoord(newChunk);
            int3 globalPos = (int3)GetWorldFromLocalPosition(chunkCoord, new float3(x, 0, z));
            int3 localCoordsInChunk = (int3)WorldToLocal(globalPos);
            if (_chunks[index].IsGenerated)
            {
                return _chunks[index].GetHeightAt(localCoordsInChunk.x, localCoordsInChunk.z);
            } else
            {
                
                return ChunkGenerator.GetHeightAt(globalPos.x, globalPos.z);
            }
        } else
        {
            return 0;
        }
    }

    public float GetHeightAt(float3 globalPosition)
    {
        int2 chunk = WorldToChunk(globalPosition);
        if (ChunkInWorldRange(chunk))
        {
            int index = GetIndexFromCoord(chunk);
            int3 localCoordsInChunk = (int3)WorldToLocal(globalPosition);
            if (_chunks[index].IsGenerated)
            {
                return _chunks[index].GetHeightAt(localCoordsInChunk.x, localCoordsInChunk.z);
            }
            else
            {

                return ChunkGenerator.GetHeightAt((int)globalPosition.x, (int)globalPosition.z);
            }
        } else
        {
            return 0;
        }
    }

    public FlowerData GetFlowerData(int flowerId, int2 chunkCoord)
    {
        return GetChunk(chunkCoord).Flowers[flowerId];
    }
    public void SetFlowerData(int flowerId, int2 chunkCoord, FlowerData flowerData)
    {
        GetChunk(chunkCoord).Flowers[flowerId] = flowerData;
    }

    public PolenSourceData GetClosestFlower(float3 globalPosition, int range, int desiredFlowerType)
    {
        int2 chunkCoord = WorldToChunk(globalPosition);
        int closestFlowerId = -2;
        float3 flowerPosition = float3.zero;
        int2 closestChunk = int2.zero;
        float closestSqDist = float.MaxValue;
        for(int x = chunkCoord.x - range; x<=chunkCoord.x + range; x++)
        {
            for (int y = chunkCoord.y - range; y <= chunkCoord.y + range; y++)
            {
                int2 currentCoord = new int2(x, y);
                if (!ChunkInWorldRange(currentCoord))
                    continue;
                int chunkId = GetIndexFromCoord(currentCoord);
                if (!_chunks[chunkId].IsGenerated || (_chunks[chunkId].FlowerType!=desiredFlowerType && desiredFlowerType!=-1))
                    continue;

                for (int i = 0; i < _chunks[chunkId].Flowers.Length; i++)
                {
                    if(!_chunks[chunkId].Flowers[i].GlobalPosition.Equals(new float3(-1,-1,-1)))
                    {
                        float distsq = math.distancesq(_chunks[chunkId].Flowers[i].GlobalPosition, globalPosition);
                        if (distsq <= closestSqDist)
                        {
                            closestFlowerId = i;
                            closestChunk = currentCoord;
                            flowerPosition = _chunks[chunkId].Flowers[i].GlobalPosition;
                            closestSqDist = distsq;
                        }
                    }
                    
                }
            }
        }
        //Debug.Log(closestChunk + ", " + closestFlowerId + ", " + flowerPosition);
        return new PolenSourceData { ChunkCoord = closestChunk, PolenSourceId = closestFlowerId, FlowerPosition = flowerPosition };
    }
}

public struct PolenSourceData
{
    public int2 ChunkCoord;
    public int PolenSourceId;
    public float3 FlowerPosition;
}
