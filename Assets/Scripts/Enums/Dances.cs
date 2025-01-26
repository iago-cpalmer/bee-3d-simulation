using Unity.Entities;
using Unity.Mathematics;

public enum Dances
{
    NONE,
    POLEN_SOURCE,
    THREAT_DETECTED
}

[InternalBufferCapacity(100)]
public struct BeeDanceInformation : IBufferElementData
{
    public int EntityId;
    public float3 EntityPosition;
    public Dances DanceType;
    public float3 DanceMessage;
}
