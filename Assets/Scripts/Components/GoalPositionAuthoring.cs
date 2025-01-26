using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class GoalPositionAuthoring : MonoBehaviour
{
    public float3 Boundaries {  get { return boundaries; } }
    [SerializeField]
    private float3 boundaries;

    public class Baker : Baker<GoalPositionAuthoring>
    {
        public override void Bake(GoalPositionAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new GoalPositionComponent());
        }
    }
}

public struct GoalPositionComponent : IComponentData
{
    public float3 Position;
    public quaternion Rotation;
}
