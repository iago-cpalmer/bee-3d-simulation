using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class BeeFlowerScanAuthoring : MonoBehaviour
{
    public class Baker : Baker<BeeNestAuthoring>
    {
        public override void Bake(BeeNestAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new BeeFlowerScanComponent { FlowerId=-1});
        }
    }
}

public struct BeeFlowerScanComponent : IComponentData
{
    public float3 FlowerPosition;
    public int2 ChunkCoord;
    public int FlowerId;

}
