using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using Latios.Kinemation;
using Assets.Scripts.Components;
using Assets.Scripts.Buffers;

namespace Assets.Scripts.Systems
{
    public partial class ColonySpawnerSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate<ColonySpawnerConfig>();
        }

        protected override void OnUpdate()
        {
            this.Enabled = false; // Ensure to run only once
            Entities
                .WithAll<ColonySpawnerConfig, BeeNestState, LocalTransform>()
                .ForEach((Entity spawnerEntity, in ColonySpawnerConfig spawnerConfig, in BeeNestState beeNestState, in LocalTransform localTransform) =>
                {
                    DynamicBuffer<DeathRequest> deathRequests = EntityManager.GetBuffer<DeathRequest>(spawnerEntity, false);
                    BeeWorld.Instance.NestPosition = localTransform.Position;
                    int leftOfInitialPopulation = spawnerConfig.InitialPopulation;
                    for (int i = 0; i < spawnerConfig.AmountToSpawn; i++)
                    {
                        Entity spawnedEntity = EntityManager.Instantiate(spawnerConfig.Prefab);
                        EntityManager.SetComponentData(spawnedEntity, new LocalTransform
                        {
                            Position = localTransform.Position,
                            Rotation = quaternion.identity,
                            Scale = 0.0f
                        });

                        EntityManager.SetComponentData(spawnedEntity, new GoalPositionComponent
                        {
                            Position = localTransform.Position,
                            Rotation = localTransform.Rotation
                        });

                        EntityManager.SetComponentData(spawnedEntity, new BeeNestComponent
                        {
                            NestPosition = localTransform.Position,
                            NestEntity = spawnerEntity // Store the entity of the nest
                        });
                        
                       // EntityManager.GetAspect<OptimizedSkeletonAspect>(spawnedEntity).ForceInitialize();

                        EntityManager.SetComponentData(spawnedEntity, new RandomComponent { Random = new Random(seed: (uint)(System.DateTime.Now.Millisecond==0?1: System.DateTime.Now.Millisecond)) });
                        /*
                        PhysicsCollider collider = EntityManager.GetComponentData<PhysicsCollider>(spawnedEntity);
                        PhysicsDataComponent physicsDataComponent = EntityManager.GetComponentData<PhysicsDataComponent>(spawnedEntity);
                        uint belongsToMask = (uint)(1 << (int)physicsDataComponent.BelongsTo);

                        uint collidesWithMask = (uint)1 << (int)physicsDataComponent.CollidesWith;
                        var filter = new CollisionFilter { BelongsTo = belongsToMask, CollidesWith = collidesWithMask, GroupIndex = physicsDataComponent.GroupIndex };
                        physicsDataComponent.CollisionFilter = filter;
                        EntityManager.SetComponentData(spawnedEntity, physicsDataComponent);
                        collider.Value.Value.SetCollisionFilter(filter);
                        EntityManager.SetComponentData(spawnedEntity, collider);*/
                        if (leftOfInitialPopulation > 0)
                        {
                            EntityManager.SetComponentEnabled<IsDead>(spawnedEntity, false);
                            leftOfInitialPopulation--;
                        } else
                        {
                            BeeStateComponent beeState = EntityManager.GetComponentData<BeeStateComponent>(spawnedEntity);
                            beeState.State = BeeStates.GROWING;
                            EntityManager.SetComponentData(spawnedEntity, beeState);
                            EntityManager.SetComponentEnabled<IsDead>(spawnedEntity, true);
                            deathRequests.Add(new DeathRequest { DeadEntity = spawnedEntity });
                            EntityManager.SetComponentData(spawnedEntity, new LocalTransform
                            {
                                Position = new float3(100000,0,100000),
                                Rotation = quaternion.identity,
                                Scale = 0.0f
                            });
                        }
                    }
                    EntityManager.SetComponentData<BeeNestState>(spawnerEntity, new BeeNestState { Population = spawnerConfig.InitialPopulation });
                }).WithStructuralChanges().Run();
        }
    }
}
