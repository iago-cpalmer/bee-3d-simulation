using Unity.Entities;

public struct PolenRequest : IBufferElementData
{
    /*
    public static implicit operator int(PolenRequest e) { return e.Value; }
    public static implicit operator PolenRequest(int e) { return new PolenRequest { Value = e }; }
    */
    public int Value;
    public Entity Requester;
    public bool IsPollen;
}
