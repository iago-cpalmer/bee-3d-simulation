using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ColonySpawnerConfigAuthoring : MonoBehaviour
{
    public GameObject Prefab { get { return prefab; } }
    public int AmountToSpawn { get { return amountToSpawn; } }
    public int InitialPopulation { get { return initialPopulation; } }

    [SerializeField] private GameObject prefab;
    [SerializeField] private int amountToSpawn;
    [SerializeField] private int initialPopulation;
    public class Baker : Baker<ColonySpawnerConfigAuthoring>
    {
        public override void Bake(ColonySpawnerConfigAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ColonySpawnerConfig
            {
                Prefab = GetEntity(authoring.Prefab, TransformUsageFlags.Dynamic),
                AmountToSpawn = authoring.AmountToSpawn,
                InitialPopulation = authoring.InitialPopulation
            });
        }
    }
}

public struct ColonySpawnerConfig : IComponentData
{
    public Entity Prefab;
    public int AmountToSpawn;
    public int InitialPopulation;
}
