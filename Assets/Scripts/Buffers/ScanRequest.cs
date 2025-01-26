using Unity.Entities;
using Unity.Mathematics;

public struct ScanRequest : IBufferElementData
{
    public Entity Requester;
    public float3 BeePosition;
    public int FlowerType;
}
