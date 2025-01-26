using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.Systems
{
    public partial class PolenSourceRegenSystem : SystemBase
    {
        private float _timerToUpdate;
        protected override void OnUpdate()
        {
            this.Enabled = false;
            _timerToUpdate += SystemAPI.Time.DeltaTime * Globals.SIMULATION_SPEED;
            if (_timerToUpdate >= Globals.TIME_TO_REGEN_POLEN)
            {
                _timerToUpdate = 0;
                new PolenRegenJob().ScheduleParallel();
            }
        }
    }
}

[BurstCompile]
[WithAll(typeof(PolenSourceComponent))]
partial struct PolenRegenJob : IJobEntity
{
    public void Execute(RefRW<PolenSourceComponent> comp, RefRW<RandomComponent> randomComponent)
    {
        comp.ValueRW.PolenAmount = randomComponent.ValueRW.Random.NextInt(Globals.MIN_POLEN_AMOUNT, Globals.MAX_POLEN_AMOUNT);
        comp.ValueRW.NectarAmount = randomComponent.ValueRW.Random.NextInt(Globals.MIN_POLEN_AMOUNT, Globals.MAX_POLEN_AMOUNT);
    }
}
