using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpriteMerger : MonoBehaviour
{
    [SerializeField] private Sprite[] spritesToMerge = null;
    [SerializeField] private SpriteRenderer finalSpriteRenderer = null;

    private void Start()
    {
        Merge();
    }

    private void Merge()
    {
        Resources.UnloadUnusedAssets();
        var newTex = new Texture2D(100, 100);

        for (int x = 0; x < newTex.width; x++)
        {
            for (int y = 0; y < newTex.height; y++)
            {
                newTex.SetPixel(x, y, new Color(1,1,1,0)); 
            }
        }

        for (int i = 0; i < spritesToMerge.Length; i++)
        {
            for (int x = 0; x < spritesToMerge[i].texture.width; i++)
            {
                for (int y = 0; y < spritesToMerge[i].texture.width; i++)
                {
                    var color = spritesToMerge[i].texture.GetPixel(x, y).a == 0 ?
                        newTex.GetPixel(x, y) :
                        spritesToMerge[i].texture.GetPixel(x, y);
                    
                    newTex.SetPixel(x, y, color);
                }
            }
        }

        newTex.Apply();
        var finalSprite = Sprite.Create(newTex, new Rect(0, 0, newTex.width, newTex.height), new Vector2(0.5f, 0.5f));
        finalSprite.name = "New Sprite";
        finalSpriteRenderer.sprite = finalSprite;
    }
}
