using Assets.Scripts.Buffers;
using Assets.Scripts.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public readonly partial struct NestAspect : IAspect
{
    public readonly Entity Entity;
    public readonly RefRW<BeeNestState> NestState;
    public readonly RefRO<ColonySpawnerConfig> ColonySpawnerConfig;
    public readonly RefRO<LocalTransform> localTransform;
    public readonly DynamicBuffer<PolenRequest> PolenRequests;
    public readonly DynamicBuffer<DanceUnsubscription> DanceUnsubscriptions;
    public readonly DynamicBuffer<BeeDanceInformation> DanceSubscriptions;
    public readonly DynamicBuffer<DeathRequest> DeathRequests;
    /*
    public void HandlePolenRequests(ComponentLookup<BeeStateComponent> beeStates)
    {
        int quantity = NestState.ValueRW.PolenStored;
        foreach (PolenRequest p in PolenRequests)
        {
            int request = p.Value;
            if(request<0)
            {
                int left = quantity - request;
                int amountToTake = math.min(request, quantity);
                quantity -= amountToTake;
                RefRW<BeeStateComponent> bee;
                if(beeStates.TryGetComponent(p.Requester, out bee))
                {

                }

                bee.ValueRW.FoodEnergy = amountToTake;
            } else
            {
                quantity += request;
            }
        }
        NestState.ValueRW.PolenStored = quantity;
        PolenRequests.Clear();
    }*/

    public void HandleDanceUnsubscriptions()
    {
        foreach (DanceUnsubscription unsubscription in DanceUnsubscriptions)
        {
            for (int i = 0; i < DanceSubscriptions.Length; i++)
            {
                if (unsubscription.EntityId == DanceSubscriptions[i].EntityId)
                {
                    DanceSubscriptions.RemoveAt(i);
                    break;
                }
            }
        }

        DanceUnsubscriptions.Clear();
    }

    public void HandlePopulationGrowth(EntityCommandBuffer.ParallelWriter ecb, int sortKey)
    {
        // Check conditions
        if(NestState.ValueRO.Population<ColonySpawnerConfig.ValueRO.AmountToSpawn 
            && NestState.ValueRO.PolenStored >= (NestState.ValueRO.Population*Globals.MIN_POLEN_PER_BEE)
            && DeathRequests.Length > 0)
        {
            // Revive one dead bee
            ecb.SetComponentEnabled<IsDead>(sortKey, DeathRequests[DeathRequests.Length - 1].DeadEntity, false);
            DeathRequests.RemoveAt(DeathRequests.Length - 1);
            // Increment population
            
            //Debug.Log("Bee Spawned, population: " + NestState.ValueRO.Population);
        }
        NestState.ValueRW.Population = ColonySpawnerConfig.ValueRO.AmountToSpawn - DeathRequests.Length;
    }
}
