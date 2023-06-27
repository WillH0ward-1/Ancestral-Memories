using UnityEngine;

public class RandomizeTreeTexture : MonoBehaviour
{
    public Texture2D[] barkTextures;  // Assign your bark textures in the inspector

    public void ApplyRandomTexture()
    {
        // Get a random index for bark texture
        int randomIndex = UnityEngine.Random.Range(0, barkTextures.Length);

        // Get the material of the tree
        Material treeMaterial = GetComponentInChildren<Renderer>().material;

        // Set the _MainTex of the material to the randomly selected bark texture
        treeMaterial.SetTexture("_MainTex", barkTextures[randomIndex]);
    }
}
