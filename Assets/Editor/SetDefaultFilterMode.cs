using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SetDefaultFilterMode
{
    [MenuItem("Tools/Set All Sprites Filter Mode to Point")]
    private static void SetAllSpritesFilterModeToPoint()
    {
        // Get all texture assets in the project
        string[] guids = AssetDatabase.FindAssets("t:Texture2D");
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;

            if (textureImporter != null)
            {
                textureImporter.filterMode = FilterMode.Point;
                textureImporter.SaveAndReimport();
            }
        }

        Debug.Log("All sprites' filter mode set to Point.");
    }
}