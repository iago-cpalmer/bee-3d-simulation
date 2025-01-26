using Assets.Scripts.Buffers;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class BeeNestStateAuthoring : MonoBehaviour
{
    public class Baker : Baker<BeeNestStateAuthoring>
    {
        public override void Bake(BeeNestStateAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new BeeNestState());
            AddBuffer<PolenRequest>(entity);
            AddBuffer<BeeDanceInformation>(entity);
            AddBuffer<DanceUnsubscription>(entity);
            AddBuffer<DeathRequest>(entity);
            AddBuffer<ScanRequest>(entity);
        }
    }
}


public struct BeeNestState : IComponentData
{
    public int PolenStored;
    public int NectarStored;
    public int Population;
}

