using Assets.Scripts.World;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
namespace Assets.Scripts.Systems
{
    [UpdateAfter(typeof(EndSimulationEntityCommandBufferSystem))]
    public partial class HandlePolenRequestsSystem : SystemBase
    {
        private DynamicBuffer<PolenRequestAtFlowerBufferElement> _polenRequests;
        private bool _cleared;
        protected override void OnStartRunning()
        {
            base.OnStartRunning();
        }

        protected override void OnUpdate()
        {
            /*
            if (firstUpdate)
            {
                if (!SystemAPI.TryGetSingletonBuffer<PolenRequestAtFlowerBufferElement>(out _polenRequests))
                {
                    BufferHolder = World.EntityManager.CreateSingletonBuffer<PolenRequestAtFlowerBufferElement>();
                }

                firstUpdate = false;
            }*/
            _polenRequests = SystemAPI.GetSingletonBuffer<PolenRequestAtFlowerBufferElement>();
            foreach (PolenRequestAtFlowerBufferElement s in _polenRequests)
            {
               

                Entity requester = s.Requester;
                int flower = s.FlowerId;
                int2 chunkCoord = s.ChunkCoord;

                if (_cleared)
                    break;
                RefRW<BeePolenSourceComponent> beeComp = SystemAPI.GetComponentRW<BeePolenSourceComponent>(requester);
                if (_cleared)
                    break;
                RefRW<RandomComponent> beeRandom = SystemAPI.GetComponentRW<RandomComponent>(requester);

                FlowerData flowerData = BeeWorld.Instance.GetFlowerData(flower, chunkCoord);
                int polenToGive = beeRandom.ValueRW.Random.NextInt(math.min(flowerData.Pollen, 1), math.min(Globals.MAX_BEE_POLLEN_TO_CARRY + 1, flowerData.Pollen + 1));
                beeComp.ValueRW.PolenAnswered = polenToGive;
                flowerData.Pollen -= polenToGive;

                int nectarToGive = beeRandom.ValueRW.Random.NextInt(math.min(flowerData.Nectar, 1), math.min(Globals.MAX_BEE_POLLEN_TO_CARRY + 1, flowerData.Nectar + 1));
                beeComp.ValueRW.NectarAnswered = nectarToGive;
                flowerData.Nectar -= nectarToGive;

                BeeWorld.Instance.SetFlowerData(flower, chunkCoord, flowerData);

                //Debug.Log("PolenRequest from: " + requester.ToString() + ", to: " + flower.ToString() + ". Amount: " + polenToGive + ", Max amount can be given: " + polenComp.ValueRO.PolenAmount);
            }
            _cleared = false;
            _polenRequests.Clear();
        }

        public void ClearRequests()
        {
            _cleared = true;
        }
    }
}