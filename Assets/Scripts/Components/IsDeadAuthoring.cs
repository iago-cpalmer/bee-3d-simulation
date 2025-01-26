using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.Components
{
    class IsDeadAuthoring : MonoBehaviour
    {
        public class Baker : Baker<IsDeadAuthoring>
        {
            public override void Bake(IsDeadAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new IsDead());
            }
        }
    }


    public struct IsDead : IComponentData, IEnableableComponent
    {

    }
}
