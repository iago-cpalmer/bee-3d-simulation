using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace Assets.Scripts.Systems
{
    [BurstCompile]
    [UpdateAfter(typeof(EndSimulationEntityCommandBufferSystem))]
    public partial class HeightRequestsHandlerSystem : SystemBase
    {
        private DynamicBuffer<HeightWorldBufferElement> _heightRequests;
        protected override void OnStartRunning()
        {
            base.OnStartRunning();
        }

        [BurstCompile]
        protected override void OnUpdate()
        {
            _heightRequests = SystemAPI.GetSingletonBuffer<HeightWorldBufferElement>();
            foreach (HeightWorldBufferElement s in _heightRequests)
            {
                Entity requester = s.Requester;
                RefRW<GoalPositionComponent> beeComp = SystemAPI.GetComponentRW<GoalPositionComponent>(requester);
                RefRO<BeeStateComponent> beeComp2 = SystemAPI.GetComponentRO<BeeStateComponent>(requester);
                RefRO<LocalTransform> beeTransform = SystemAPI.GetComponentRO<LocalTransform>(requester);
                if (beeComp2.ValueRO.State == BeeStates.FORAGING)
                {
                    beeComp.ValueRW.Position.y = BeeWorld.Instance.GetHeightAt(beeComp.ValueRW.Position) + 2;
                }
            }

            _heightRequests.Clear();
        }
    }
}