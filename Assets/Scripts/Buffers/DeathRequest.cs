using Unity.Entities;

namespace Assets.Scripts.Buffers
{
    public struct DeathRequest : IBufferElementData
    {
        public Entity DeadEntity;
    }

}
