using Unity.Entities;
using UnityEngine;

public class RandomAuthoring : MonoBehaviour
{
    public class Baker : Baker<RandomAuthoring>
    {
        public override void Bake(RandomAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new RandomComponent());
        }
    }
}

public struct RandomComponent : IComponentData
{
    public Unity.Mathematics.Random Random;
}
