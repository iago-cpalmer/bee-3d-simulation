using UnityEngine;
using Unity.Entities;
public class NestTagAuthoring : MonoBehaviour
{
    public class Baker : Baker<NestTagAuthoring>
    {
        public override void Bake(NestTagAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new NestTag());
        }
    }
}


public struct NestTag : IComponentData
{

}

