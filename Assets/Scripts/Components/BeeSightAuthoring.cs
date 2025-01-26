using Unity.Entities;
using UnityEngine;

public class BeeSightAuthoring : MonoBehaviour
{
    public int SightRange { get { return sightRange; } }
    [SerializeField] private int sightRange;

    public class Baker : Baker<BeeSightAuthoring>
    {
        public override void Bake(BeeSightAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new BeeSightComponent { SightRange = authoring.SightRange });
        }
    }
}

public struct BeeSightComponent : IComponentData
{
    public int SightRange;
}
