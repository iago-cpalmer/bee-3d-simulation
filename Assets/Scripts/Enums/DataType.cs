
using System.Runtime.InteropServices;
using Unity.Mathematics;

namespace Assets.Scripts.Enums
{
    public enum DataType
    {
        NONE,
        AGE,
        ENERGY,
        STATE,
        BEE_AMOUNT,
        POLEN_AMOUNT,
        POSITION,
        ROTATION,
        ENTITY_TYPE,
        POPULATION,
        NECTAR_AMOUNT,
        CARBS,
        FATS,
        PROT
    }
    [StructLayout(LayoutKind.Explicit)]
    public struct DataUnion
    {
        [FieldOffset(0)] public float3 Float3;
        [FieldOffset(0)] public quaternion Quaternion;
        [FieldOffset(0)] public int Int;
        [FieldOffset(0)] public float Float;
        [FieldOffset(0)] public BeeStates BeeState;
        [FieldOffset(0)] public EntityType EntityType;
    }
}
