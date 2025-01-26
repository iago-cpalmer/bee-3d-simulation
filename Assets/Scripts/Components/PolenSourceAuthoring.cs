using Unity.Entities;
using UnityEngine;

public class PolenSourceAuthoring : MonoBehaviour
{
    public class Baker : Baker<PolenSourceAuthoring>
    {
        public override void Bake(PolenSourceAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new PolenSourceComponent { PolenAmount = 0});
        }
    }
}

public struct PolenSourceComponent : IComponentData
{
    public int PolenAmount;
    public int NectarAmount;
}
