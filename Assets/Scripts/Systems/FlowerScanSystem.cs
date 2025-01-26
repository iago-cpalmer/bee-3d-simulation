using Assets.Scripts.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Assets.Scripts.Systems
{
    public partial class FlowerScanSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            this.Enabled = false;
            /*
            if (BeeWorld.Instance == null)
                return;
            
            Entities
               .WithAll<BeeFlowerScanComponent, BeeStateComponent, LocalTransform>()
               .WithNone<IsDead>()
               .ForEach((Entity entity, ref BeeFlowerScanComponent flowerScan, ref BeeStateComponent beeState, in LocalTransform transform) =>
               {
                   if (beeState.State == BeeStates.SCANNING)
                   {
                       float3 position = transform.Position;
                       PolenSourceData polenSourceData = BeeWorld.Instance.GetClosestFlower(position, 2);
                       if(polenSourceData.ChunkCoord.Equals(flowerScan.ChunkCoord) && polenSourceData.FlowerPosition.Equals(flowerScan.FlowerPosition))
                       {
                           beeState.State = BeeStates.FORAGING;
                       } else
                       {
                           flowerScan.ChunkCoord = polenSourceData.ChunkCoord;
                           flowerScan.FlowerId = polenSourceData.PolenSourceId;
                           flowerScan.FlowerPosition = polenSourceData.FlowerPosition;
                       }                       
                   }
               }).WithoutBurst().Run();*/
        }
    }
}
