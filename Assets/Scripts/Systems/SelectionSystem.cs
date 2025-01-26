using Assets.Scripts.Components;
using Assets.Scripts.Enums;
using Assets.Scripts.Player;
using System;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace Assets.Scripts.Systems
{
    [BurstCompile]
    public partial class SelectionSystem : SystemBase
    {
        public static SelectionSystem Instance { get { return _instance; } }
        private static SelectionSystem _instance;

        private PhysicsWorldSingleton _physicsWorld;

        private Entity _selectedEntity;
        private EntityType _selectedEntityType;

        public event Action OnSelectEntity;
        public event Action OnUnselectEntity;

        private float _timeToUpdate = 1.0f;
        private float _elapsedTime = 0.0f;

        private bool _subscribed;

        protected override void OnCreate()
        {
            base.OnCreate();
            _instance = this;
            
        }

        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            Physics.simulationMode = SimulationMode.Script;
            if (SelectionHandler.Instance!=null)
            {
                //SelectionHandler.Instance.OnClick += OnClick;
                SelectionHandler.Instance.OnClick += NewOnClick;
                _subscribed = true;
            }
            
            _selectedEntity = Entity.Null;
        }
        protected override void OnUpdate()
        {
            if (!_subscribed && SelectionHandler.Instance != null)
            {
                //SelectionHandler.Instance.OnClick += OnClick;
                SelectionHandler.Instance.OnClick += NewOnClick;
                _subscribed = true;
            }
            _elapsedTime += SystemAPI.Time.DeltaTime;

            if (_selectedEntity != Entity.Null)
            {
                // There is an entity selected
                RefRO<LocalTransform> beeTransform = SystemAPI.GetComponentRO<LocalTransform>(_selectedEntity);
                //SelectionHandler.Instance.DictDataTypeData[DataType.POSITION] = new DataUnion { Float3 = beeTransform.ValueRO.Position };
                //SelectionHandler.Instance.DictDataTypeData[DataType.POSITION] = new DataUnion { Float3 = math.lerp(SelectionHandler.Instance.DictDataTypeData[DataType.POSITION].Float3, beeTransform.ValueRO.Position, SystemAPI.Time.DeltaTime * 10)};
                SelectionHandler.Instance.DictDataTypeData[DataType.ROTATION] = new DataUnion { Quaternion = beeTransform.ValueRO.Rotation };

                if (_elapsedTime >= _timeToUpdate)
                {
                    _elapsedTime = 0;
                    // Update other info
                    switch(_selectedEntityType)
                    {
                        case EntityType.BEE:
                            RefRO<BeeStateComponent> beeState = SystemAPI.GetComponentRO<BeeStateComponent>(_selectedEntity);
                            if(beeState.ValueRO.State == BeeStates.GROWING)
                            {
                                // Deselect entity
                                _selectedEntity = Entity.Null;
                                _selectedEntityType = EntityType.NONE;
                                OnUnselectEntity?.Invoke();
                            }
                            SelectionHandler.Instance.DictDataTypeData[DataType.ENERGY] = new DataUnion { Float = beeState.ValueRO.Energy };
                            SelectionHandler.Instance.DictDataTypeData[DataType.STATE] = new DataUnion { BeeState = beeState.ValueRO.State };
                            SelectionHandler.Instance.DictDataTypeData[DataType.POLEN_AMOUNT] = new DataUnion { Int = beeState.ValueRO.PollenTaken };
                            SelectionHandler.Instance.DictDataTypeData[DataType.AGE] = new DataUnion { Float = beeState.ValueRO.Age };
                            SelectionHandler.Instance.DictDataTypeData[DataType.NECTAR_AMOUNT] = new DataUnion { Float = beeState.ValueRO.NectarTaken };
                            SelectionHandler.Instance.DictDataTypeData[DataType.PROT] = new DataUnion { Float = beeState.ValueRO.Protein };
                            SelectionHandler.Instance.DictDataTypeData[DataType.CARBS] = new DataUnion { Float = beeState.ValueRO.Carbohydrates };
                            SelectionHandler.Instance.DictDataTypeData[DataType.FATS] = new DataUnion { Float = beeState.ValueRO.Fats };
                            break;
                        case EntityType.BEE_NEST:
                            RefRO<BeeNestState> beeNestState = SystemAPI.GetComponentRO<BeeNestState>(_selectedEntity);
                            SelectionHandler.Instance.DictDataTypeData[DataType.POLEN_AMOUNT] = new DataUnion { Int = beeNestState.ValueRO.PolenStored };
                            SelectionHandler.Instance.DictDataTypeData[DataType.NECTAR_AMOUNT] = new DataUnion { Int = beeNestState.ValueRO.NectarStored };
                            SelectionHandler.Instance.DictDataTypeData[DataType.POPULATION] = new DataUnion { Int = beeNestState.ValueRO.Population };
                            break;
                        default:
                            break;
                    }
                }
            }
            
        }
        protected override void OnStopRunning()
        {
            base.OnStopRunning();
            _subscribed = false;
        }

        public float3 GetEntityPos()
        {
            RefRO<LocalTransform> beeTransform = SystemAPI.GetComponentRO<LocalTransform>(_selectedEntity);
            return beeTransform.ValueRO.Position;
        }

        private void OnClick(float3 src, float3 direction, float range)
        {
            _physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
            RaycastInput input = new RaycastInput
            {
                Start = src,
                End = src + direction * range,
                Filter = new CollisionFilter
                {
                    BelongsTo = ~0u,
                    CollidesWith = ~0u,
                    GroupIndex = -10
                }
            };

            Unity.Physics.RaycastHit hit;
            if(_physicsWorld.CastRay(input, out hit) && SystemAPI.HasComponent<SelectableTag>(hit.Entity))
            {
                // Select entity
                _selectedEntity = hit.Entity;
                _selectedEntityType = SystemAPI.GetComponent<SelectableTag>(hit.Entity).EntityType;
                SelectionHandler.Instance.DictDataTypeData[DataType.ENTITY_TYPE] = new DataUnion { EntityType = _selectedEntityType };
                OnSelectEntity?.Invoke();
            } else
            {
                // Deselect entity
                _selectedEntity = Entity.Null;
                _selectedEntityType = EntityType.NONE;
                OnUnselectEntity?.Invoke();
            }
        }

        private void NewOnClick(float3 src, float3 direction, float range)
        {
            double currentTimeInMillis = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            //float closestDistsq = float.MaxValue;
            float closestDot = float.MinValue;
            Entity closestEntity = Entity.Null;
            EntityType closestEntityType = EntityType.NONE;
            range *= range;
            Entities.WithAll<SelectableTag, LocalTransform>().WithNone<IsDead>().ForEach((Entity entity, in SelectableTag tag, in LocalTransform transform) =>
            {
                float distsq = math.distancesq(src, transform.Position);
                if(distsq <= range)
                {
                    float3 dirToBee = transform.Position - src;
                    float dotProduct = math.dot(math.normalize(dirToBee), math.normalize(direction));
                    float threshold = math.lerp(0.9875f, 1.0f, distsq / range);
                    if(dotProduct>=threshold)
                    {
                        // Check if it's inside sphere
                        float3 pointNearBee = (math.normalize(direction) * math.sqrt(distsq)) + src;
                        if(math.distancesq(pointNearBee, transform.Position)<=tag.CollisionRadius) {
                            if(dotProduct > closestDot)
                            {
                                closestEntity = entity;
                                closestEntityType = tag.EntityType;
                                closestDot = dotProduct;
                            }
                            
                        }

                        
                    }
                }
            }).Run();

            if(closestEntityType!=EntityType.NONE)
            {
                // Select entity
                _selectedEntity = closestEntity;
                _selectedEntityType = closestEntityType;
                SelectionHandler.Instance.DictDataTypeData[DataType.ENTITY_TYPE] = new DataUnion { EntityType = _selectedEntityType };
                OnSelectEntity?.Invoke();
            } else
            {
                // Deselect entity
                _selectedEntity = Entity.Null;
                _selectedEntityType = EntityType.NONE;
                OnUnselectEntity?.Invoke();
            }

            Debug.Log((DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - currentTimeInMillis);
        }
    }
}
