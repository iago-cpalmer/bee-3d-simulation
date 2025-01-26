using Assets.Scripts.Sound;
using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.Components
{
    public class SoundEmitterAuthoring : MonoBehaviour
    {
        [SerializeField] private SoundType sound;
        public class Baker : Baker<SoundEmitterAuthoring>
        {
            public override void Bake(SoundEmitterAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new SoundEmitter { Sound = authoring.sound });
            }
        }
    }

    public struct SoundEmitter : IComponentData
    {
        public SoundType Sound;
        public float Pitch;
    }
}
