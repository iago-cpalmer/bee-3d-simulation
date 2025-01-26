using Unity.Entities;
using UnityEngine;


public class BeeStateAuthoring : MonoBehaviour
{
    public BeeStates InitialState {  get { return initialState; } }
    [SerializeField] private BeeStates initialState;

    public class Baker : Baker<BeeStateAuthoring>
    {
        public override void Bake(BeeStateAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new BeeStateComponent
            {
                State = authoring.InitialState,
                LifeLeftToDie = Globals.MAX_LIFE_EXPECTANCY
            });
        }
    }
}

public struct BeeStateComponent : IComponentData
{
    public BeeStates State;
    public float Energy;
    public float TimeWithoutEnergy;
    public float TimeWithoutFood;
    public float Stress;
    public float LifeLeftToDie;
    public float Age;
    public int PollenTaken;
    public int NectarTaken;
    public float TimeForAction;
    public BeeStates StateBeforeResting;
    //public float FoodEnergy;
    public float Carbohydrates;
    public float Protein;
    public float Fats;
}
