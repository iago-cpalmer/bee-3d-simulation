using Assets.Scripts.Enums;
using Unity.Entities;
using UnityEngine;

public class SelectableTagAuthoring : MonoBehaviour
{
    public EntityType EntityType;
    public float CollisionRadius;
    public class Baker : Baker<SelectableTagAuthoring>
    {
        public override void Bake(SelectableTagAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new SelectableTag {EntityType=authoring.EntityType, CollisionRadius=authoring.CollisionRadius });
        }
    }
}


public struct SelectableTag : IComponentData
{
    public EntityType EntityType;
    public float CollisionRadius;
}

