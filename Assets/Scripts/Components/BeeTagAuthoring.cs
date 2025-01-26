using Unity.Entities;
using UnityEngine;

public class BeeTagAuthoring : MonoBehaviour
{
    public class Baker : Baker<BeeTagAuthoring>
    {
        public override void Bake(BeeTagAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new BeeTag());
        }
    }
}


public struct BeeTag : IComponentData
{

}
