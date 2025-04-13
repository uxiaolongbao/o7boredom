using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;
public class SlipperyWall
{
    //This is part of the tools
    [MenuItem("Tools/Create PhysicsMaterial2D")]
    public static void CreateMaterial()
    {
        //Setting friction
        PhysicsMaterial2D material = new PhysicsMaterial2D("SlipperyMaterial");
        material.friction = 0f;
        material.bounciness = 0f;

        AssetDatabase.CreateAsset(material, "Assets/SlipperyMaterial.physicsMaterial2D");
        AssetDatabase.SaveAssets();

        Debug.Log("SlipperyMaterial2D is created for assets");
    }
}
