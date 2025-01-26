using Assets.Scripts.Buffers;
using Assets.Scripts.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

namespace Assets.Scripts.Systems
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    //[UpdateAfter(typeof(EndSimulationEntityCommandBufferSystem))]
    //[UpdateBefore(typeof(PhysicsSystemGroup))]
    public partial struct BeeBehaviorSystem : ISystem
    {
        private ComponentLookup<BeeNestState> _nests;
        private PhysicsWorldSingleton _physicsWorld;
        private Entity BufferHolder;
        private Entity HeightBufferHolder;
        private BufferLookup<BeeDanceInformation> beeDanceLookup;
        public void OnCreate(ref SystemState state)
        {
            _nests = state.GetComponentLookup<BeeNestState>(true);
            BufferHolder = state.EntityManager.CreateSingletonBuffer<PolenRequestAtFlowerBufferElement>();
            HeightBufferHolder = state.EntityManager.CreateSingletonBuffer<HeightWorldBufferElement>();
            beeDanceLookup = state.GetBufferLookup<BeeDanceInformation>(true);
        }

        public void OnUpdate(ref SystemState state)
        {
            _physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
            _nests.Update(ref state);
            EndSimulationEntityCommandBufferSystem.Singleton ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            beeDanceLookup.Update(ref state);
            BeeBehaveJob beeBehaveJob = new BeeBehaveJob(SystemAPI.Time.DeltaTime * Globals.SIMULATION_SPEED, (uint)(UnityEngine.Random.value * uint.MaxValue), _nests,
                ecb.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(), _physicsWorld, BufferHolder, beeDanceLookup, HeightBufferHolder);
            beeBehaveJob.ScheduleParallel();
        }

        [BurstCompile]
        [WithAll(typeof(BeeTag))]
        [WithNone(typeof(IsDead))]
        public partial struct BeeBehaveJob : IJobEntity
        {
            public float DeltaTime;
            public uint EntitySeed;
            public EntityCommandBuffer.ParallelWriter ECB;
            [ReadOnly] public PhysicsWorldSingleton PhysicsWorld;
            [ReadOnly] public ComponentLookup<BeeNestState> Nests;
            [ReadOnly] public Entity PolenRequestsBufferHolder;
            [ReadOnly] public Entity HeightBufferHolder;
            [ReadOnly] public BufferLookup<BeeDanceInformation> DanceBufferLookup;

            public BeeBehaveJob(float deltaTime, uint entitySeed, ComponentLookup<BeeNestState> nests, EntityCommandBuffer.ParallelWriter ecb, PhysicsWorldSingleton physicsWorld, 
                Entity bufferHolder, BufferLookup<BeeDanceInformation> danceBufferLookup, Entity heightBufferHolder) : this()
            {
                DeltaTime = deltaTime;
                EntitySeed = entitySeed;
                Nests = nests;
                ECB = ecb;
                PhysicsWorld = physicsWorld;
                PolenRequestsBufferHolder = bufferHolder;
                //EntityManager = entityManager;
                DanceBufferLookup = danceBufferLookup;
                HeightBufferHolder = heightBufferHolder;
            }

            public void Execute(BeeStateAspect bee, [EntityIndexInQuery] int sortKey)
            {
                bee.RandomComp.ValueRW.Random.InitState(EntitySeed);

                bee.HandleLifeStats(DeltaTime, ECB, sortKey);
                

                //bee.LocalTransform.ValueRW.ApplyScale(0.025f);
                RefRO<BeeNestState> beeNest = Nests.GetRefRO(bee.BeeNest.ValueRO.NestEntity);
                switch (bee.BeeState.ValueRO.State)
                {
                    case BeeStates.NONE:
                        // TODO: TEMPORARY
                        bee.BeeState.ValueRW.State = BeeStates.FORAGING;
                        break;
                    case BeeStates.FORAGING:
                        bee.Forage(DeltaTime, ECB, sortKey, HeightBufferHolder);
                        break;
                    case BeeStates.RECOLLECTING:
                        bee.Recollect(DeltaTime, ECB, sortKey);
                        break;
                    case BeeStates.SCANNING:
                        bee.Scan(DeltaTime, PhysicsWorld, ECB, sortKey);
                        break;
                    case BeeStates.COLLECTING:
                        bee.Collect(DeltaTime, ECB, sortKey, PolenRequestsBufferHolder);
                        break;
                    case BeeStates.RESTING:
                        bee.Rest(DeltaTime, ECB, sortKey, DanceBufferLookup[bee.BeeNest.ValueRO.NestEntity]);
                        break;
                    case BeeStates.DANCING:
                        bee.Dance(DeltaTime, ECB, sortKey);
                        break;
                    case BeeStates.WATCHING:
                        bee.Watch(DeltaTime, ECB, sortKey);
                        break;
                    case BeeStates.GROWING:
                        bee.Grow(DeltaTime, ECB, sortKey);
                        break;
                    case BeeStates.DEAD:
                        bee.BeeState.ValueRW.State = BeeStates.GROWING;
                        break;
                }
                if(bee.BeeState.ValueRO.State != BeeStates.GROWING)
                {
                    bee.LocalTransform.ValueRW.Scale = 0.025f;
                } else
                {
                    bee.LocalTransform.ValueRW.Scale = 0f;
                    bee.LocalTransform.ValueRW.Position = new float3(100000, 0, 100000);
                }
                
                EntitySeed++;
            }
        }
    }
}
