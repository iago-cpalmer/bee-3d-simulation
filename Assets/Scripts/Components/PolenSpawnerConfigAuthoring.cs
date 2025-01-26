using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class PolenSpawnerConfigAuthoring : MonoBehaviour
{
    public GameObject Prefab { get { return prefab; } }
    [SerializeField] private GameObject prefab;
    public int Type { get { return _type; } }
    [SerializeField] private int _type;

    public class Baker : Baker<PolenSpawnerConfigAuthoring>
    {
        public override void Bake(PolenSpawnerConfigAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new PolenSpawnerConfigComponent { Type = authoring.Type, Prefab = GetEntity(authoring.Prefab, TransformUsageFlags.Dynamic) });
        }
    }
}

public struct PolenSpawnerConfigComponent : IComponentData
{
    public int Type;
    public Entity Prefab;
}
