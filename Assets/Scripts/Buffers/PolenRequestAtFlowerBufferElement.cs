using Unity.Entities;
using Unity.Mathematics;

public struct PolenRequestAtFlowerBufferElement : IBufferElementData
{
    public Entity Requester;
    public int FlowerId;
    public int2 ChunkCoord;
}
