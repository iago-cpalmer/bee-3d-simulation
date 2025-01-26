using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/World decorational object")]
public class WorldObject : ScriptableObject
{
    public Mesh[] LODs;
    public Material[] Materials;
    public float ScaleMultiplier;
}

public struct WorldObjectDataInstance
{
    public byte worldObjectId;
    public Matrix4x4[] TransformMatrix;
}
