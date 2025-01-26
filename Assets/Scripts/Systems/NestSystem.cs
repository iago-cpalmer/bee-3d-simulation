using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.Scripts.Systems
{
    [BurstCompile]
    [UpdateAfter(typeof(EndSimulationEntityCommandBufferSystem))]
    public partial class NestSystem : SystemBase
    {
        public PolenRequest PolenRequestPlayer;
        public PolenRequest NectarRequestPlayer;

        private DynamicBuffer<PolenRequest> _polenRequests;

        protected override void OnCreate()
        {
            base.OnCreate();
        }

        [BurstCompile]
        protected override void OnUpdate()
        {
            EndSimulationEntityCommandBufferSystem.Singleton ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();

            Entities
               .WithAll<BeeNestState>()
               .ForEach((Entity entity, ref BeeNestState beeNestState) =>
               {
                   _polenRequests = SystemAPI.GetBuffer<PolenRequest>(entity);
                   HandlePolenRequests(ref beeNestState);
               }).WithoutBurst().Run();

            NestHandleJob nestHandleJob = new NestHandleJob
            {
                PolenRequestPlayer = PolenRequestPlayer,
                NectarRequestPlayer = NectarRequestPlayer,
                ECB = ecb.CreateCommandBuffer(EntityManager.WorldUnmanaged).AsParallelWriter()
            };
            nestHandleJob.ScheduleParallel();
            PolenRequestPlayer.Value = 0;
        }

        private void HandlePolenRequests(ref BeeNestState beeNestState)
        {
            _polenRequests = SystemAPI.GetSingletonBuffer<PolenRequest>(false);
            int quantity = beeNestState.PolenStored;
            int nectarQuantity = beeNestState.NectarStored;
            foreach (PolenRequest p in _polenRequests)
            {
                int request = p.Value;
                if (request < 0)
                {
                    request *= -1;

                    RefRW<BeeStateComponent> bee = SystemAPI.GetComponentRW<BeeStateComponent>(p.Requester);
                    if (p.IsPollen)
                    {
                        int amountToTake = math.min(request, quantity);
                        if (quantity > 0)
                        {
                            quantity -= amountToTake;
                            bee.ValueRW.Protein = amountToTake * 0.4f;
                            bee.ValueRW.Fats = amountToTake * 0.3f;
                            
                        }
                    }
                    else
                    {
                        int amountToTake = math.min(request, nectarQuantity);
                        if (nectarQuantity > 0)
                        {
                            bee.ValueRW.Carbohydrates = amountToTake * 0.7f;
                            nectarQuantity -= amountToTake;
                        }
                    }

                }
                else
                {
                    if (p.IsPollen)
                    {
                        quantity += request;
                    }
                    else
                    {
                        nectarQuantity += request;
                    }
                }
            }
            beeNestState.PolenStored = quantity;
            beeNestState.NectarStored = nectarQuantity;
            _polenRequests.Clear();
        }
}



[BurstCompile]
[WithAll(typeof(NestTag))]
public partial struct NestHandleJob : IJobEntity
{
    public PolenRequest PolenRequestPlayer;
    public PolenRequest NectarRequestPlayer;
    public EntityCommandBuffer.ParallelWriter ECB;

    public void Execute(NestAspect nest, [EntityIndexInQuery] int sortKey)
    {
        if (PolenRequestPlayer.Value > 0)
        {
            nest.PolenRequests.Add(PolenRequestPlayer);
            nest.PolenRequests.Add(NectarRequestPlayer);
        }
        nest.HandleDanceUnsubscriptions();
        nest.HandlePopulationGrowth(ECB, sortKey);
        /*
        if(FirstUpdate)
        {
            nest.HandlePopulationGrowth(ECB, sortKey, EntityManager);
        }*/

    }
}
}
