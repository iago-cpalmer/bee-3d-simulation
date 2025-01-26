using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class BeeNestAuthoring : MonoBehaviour
{
    public class Baker : Baker<BeeNestAuthoring>
    {
        public override void Bake(BeeNestAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new BeeNestComponent());
        }
    }
}

public struct BeeNestComponent : IComponentData
{
    public float3 NestPosition;
    public Entity NestEntity;

    public BeeNestComponent(float3 nestPosition, Entity nestEntity)
    {
        NestPosition = nestPosition;
        NestEntity = nestEntity;
    }
}
