using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
namespace Assets.Scripts.Systems
{
    [UpdateAfter(typeof(EndSimulationEntityCommandBufferSystem))]
    public partial class HandleScans : SystemBase
    {
        public bool FirstUpdate = true;
        private DynamicBuffer<ScanRequest> _spawnRequests;
        ComponentLookup<BeeFlowerScanComponent> beeFlowerScan;
        private float timerToUpdateGoal = 0;
        private int currentFlowerGoal = 0;
        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            beeFlowerScan = SystemAPI.GetComponentLookup<BeeFlowerScanComponent>(false);           
        }

        protected override void OnUpdate()
        {
            if (FirstUpdate)
            {
                if (SystemAPI.TryGetSingletonBuffer<ScanRequest>(out _spawnRequests))
                {
                    FirstUpdate = false;
                } else
                {
                    return;
                }
            } else
            {
                _spawnRequests = SystemAPI.GetSingletonBuffer<ScanRequest>(false);
            }
            timerToUpdateGoal += SystemAPI.Time.DeltaTime;
            if (timerToUpdateGoal >= Globals.DAY_DURATION)
            {
                timerToUpdateGoal = 0;
                currentFlowerGoal = UnityEngine.Random.Range(0, 2);
            }


            beeFlowerScan.Update(this);

            foreach (ScanRequest s in _spawnRequests)
            {
                RefRW<BeeFlowerScanComponent> flowerScan = beeFlowerScan.GetRefRW(s.Requester);
                PolenSourceData polenSourceData = BeeWorld.Instance.GetClosestFlower(s.BeePosition, 2, currentFlowerGoal);
                
                if (polenSourceData.ChunkCoord.Equals(flowerScan.ValueRO.ChunkCoord) && polenSourceData.FlowerPosition.Equals(flowerScan.ValueRO.FlowerPosition))
                {
                    flowerScan.ValueRW.FlowerId = -3;
                }
                else
                {
                    flowerScan.ValueRW.ChunkCoord = polenSourceData.ChunkCoord;
                    flowerScan.ValueRW.FlowerId = polenSourceData.PolenSourceId;
                    flowerScan.ValueRW.FlowerPosition = polenSourceData.FlowerPosition;
                }
            }
            _spawnRequests.Clear();
        }
    }
}

