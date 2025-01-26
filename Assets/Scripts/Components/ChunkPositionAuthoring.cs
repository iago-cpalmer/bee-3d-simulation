using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class ChunkPositionAuthoring : MonoBehaviour
{
    public class Baker : Baker<ChunkPositionAuthoring>
    {
        public override void Bake(ChunkPositionAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ChunkPositionComponent());
        }
    }
}

public struct ChunkPositionComponent : IComponentData
{
    public int2 Coord;
} 
