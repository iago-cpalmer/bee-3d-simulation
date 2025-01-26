using Assets.Scripts.Components;
using Latios.Kinemation;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Entities.SystemAPI;

namespace Assets.Scripts.Systems
{
    public partial struct AnimationPlayerSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            /*
            if(BeeWorld.Instance!=null)
                new AnimationPlayerJob { et = (float)Time.ElapsedTime, PlayerPosition = BeeWorld.Instance.PlayerTransform.position }.ScheduleParallel();*/
        }


        [BurstCompile]
        [WithNone(typeof(IsDead))]
        partial struct AnimationPlayerJob : IJobEntity
        {
            public float et;
            public float3 PlayerPosition;

            public void Execute(OptimizedSkeletonAspect skeleton, in AnimationStateComponent animationState, in LocalTransform localTransform)
            {
                if(math.distancesq(localTransform.Position, PlayerPosition)<=1024)
                {
                    ref var clip = ref animationState.blob.Value.clips[animationState.CurrentAnimationPlaying];
                    var clipTime = clip.LoopToClipTime(et);

                    clip.SamplePose(ref skeleton, clipTime, 1f);
                    skeleton.EndSamplingAndSync();
                }
            }
        }
    }
}
