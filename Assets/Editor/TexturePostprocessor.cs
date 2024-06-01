using UnityEditor;
using UnityEngine;

public class TexturePostprocessor : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        TextureImporter textureImporter = (TextureImporter)assetImporter;
        textureImporter.filterMode = FilterMode.Point;
    }
}
