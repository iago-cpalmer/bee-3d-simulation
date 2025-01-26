using Unity.Entities;
using UnityEngine;

public class WorldTagAuthoring : MonoBehaviour
{
    public class Baker : Baker<WorldTagAuthoring>
    {
        public override void Bake(WorldTagAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new WorldTag());
            AddBuffer<HeightWorldBufferElement>(entity);
        }
    }
}


public struct WorldTag : IComponentData
{

}
