using Unity.Entities;
using Latios.Kinemation;
using Latios.Authoring;
using Latios.Kinemation.Authoring;
using Unity.Collections;
using UnityEngine;
public struct AnimationStateComponent : IComponentData
{
    public BlobAssetReference<SkeletonClipSetBlob> blob;
    public int CurrentAnimationPlaying;
}
[DisallowMultipleComponent]
public class AnimationStateAuthoring : MonoBehaviour
{
    public AnimationClip[] Clips;
}

[TemporaryBakingType]
struct AnimationStateSmartBakeItem : ISmartBakeItem<AnimationStateAuthoring>
{
    SmartBlobberHandle<SkeletonClipSetBlob> blob;

    public bool Bake(AnimationStateAuthoring authoring, IBaker baker)
    {
        baker.AddComponent<AnimationStateComponent>(baker.GetEntity(TransformUsageFlags.Dynamic));
        var clips = new NativeArray<SkeletonClipConfig>(authoring.Clips.Length, Allocator.Temp);
        for(int i = 0; i < clips.Length; i++)
        {
            clips[i] = new SkeletonClipConfig { clip = authoring.Clips[i], settings = SkeletonClipCompressionSettings.kDefaultSettings };
        }
        
        blob = baker.RequestCreateBlobAsset(baker.GetComponent<Animator>(), clips);
        return true;
    }

    public void PostProcessBlobRequests(EntityManager entityManager, Entity entity)
    {
        entityManager.SetComponentData(entity, new AnimationStateComponent { blob = blob.Resolve(entityManager) });
    }
}

class AnimationStateBaker : SmartBaker<AnimationStateAuthoring, AnimationStateSmartBakeItem>
{
}






