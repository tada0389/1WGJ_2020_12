using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private Texture2D drawTexture;
    Color[] buffer;
    
    // Start is called before the first frame update
    void Start()
    {
        Texture2D mainTexture = (Texture2D)GetComponent<Renderer>().material.mainTexture;
        Color[] pixels = mainTexture.GetPixels();

        buffer = new Color[pixels.Length];

        // 画面上半分を塗りつぶす
        Debug.Log(mainTexture.width + " " + mainTexture.height);
        for(int x = 0; x < mainTexture.width; ++x)
        {
            for(int y = 0; y < mainTexture.height; ++y)
            {
                if (y < mainTexture.height / 2)
                {
                    buffer.SetValue(Color.blue, x + mainTexture.width * y);
                }
                else buffer.SetValue(Color.white, x + mainTexture.width * y);
            }
        }

        drawTexture = new Texture2D(mainTexture.width, mainTexture.height, TextureFormat.RGBA32, false);
        drawTexture.filterMode = FilterMode.Point;
    }

    // Update is called once per frame
    void Update()
    {
        drawTexture.SetPixels(buffer);
        drawTexture.Apply();
        GetComponent<Renderer>().material.mainTexture = drawTexture;
    }
}
