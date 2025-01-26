using UnityEngine;
[CreateAssetMenu(fileName = "Terrain", menuName = "Terrain/DefaultWorldObject")]
public class WorldObjectScriptableObject : ScriptableObject
{
    public string Name;
    public Mesh Mesh;
    public Material Material;
}
