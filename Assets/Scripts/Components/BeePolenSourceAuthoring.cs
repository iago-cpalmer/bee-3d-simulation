using Unity.Entities;
using UnityEngine;

public class BeePolenSourceAuthoring : MonoBehaviour
{
    public class Baker : Baker<BeePolenSourceAuthoring>
    {
        public override void Bake(BeePolenSourceAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new BeePolenSourceComponent { PolenAnswered = -1 });
        }
    }
}

public struct BeePolenSourceComponent : IComponentData
{
    //public Entity PolenSourceEntity;
    public int PolenAnswered; // Polen that got as an answer when requesting the flower for polen. When no answer to process, -1
    public int NectarAnswered; // Nectar that got as an answer when requesting the flower for polen. When no answer to process, -1
}
