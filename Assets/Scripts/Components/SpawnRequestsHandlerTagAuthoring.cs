using UnityEngine;
using Unity.Entities;
public class SpawnRequestsHandlerTagAuthoring : MonoBehaviour
{
    public class Baker : Baker<SpawnRequestsHandlerTagAuthoring>
    {
        public override void Bake(SpawnRequestsHandlerTagAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new SpawnRequestsHandlerTag());
            //AddBuffer<SpawnRequestBufferElement>(entity);
        }
    }
}


public struct SpawnRequestsHandlerTag : IComponentData
{

}

