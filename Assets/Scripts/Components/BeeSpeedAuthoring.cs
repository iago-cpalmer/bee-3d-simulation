using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class BeeSpeedAuthoring : MonoBehaviour
{
    public float Speed { get { return speed; } }

    [SerializeField] private float speed;

    public class Baker : Baker<BeeSpeedAuthoring>
    {
        public override void Bake(BeeSpeedAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new BeeSpeedComponent { 
                Speed = authoring.Speed 
            });
        }
    }
}

public struct BeeSpeedComponent : IComponentData
{
    public float Speed;
}
