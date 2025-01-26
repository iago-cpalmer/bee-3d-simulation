using Unity.Entities;
using Unity.Physics;
using UnityEngine;

public class PhysicsDataAuthoring : MonoBehaviour
{
    public LayerMask BelongsTo;
    public LayerMask CollidesWith;
    public int GroupIndex;

    public class Baker : Baker<PhysicsDataAuthoring>
    {
        public override void Bake(PhysicsDataAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new PhysicsDataComponent { BelongsTo = authoring.BelongsTo.value, CollidesWith = authoring.CollidesWith.value, GroupIndex = authoring.GroupIndex});
        }
    }
}

public struct PhysicsDataComponent : IComponentData
{
    public int BelongsTo;
    public int CollidesWith;
    public int GroupIndex;
    public CollisionFilter CollisionFilter;
}
