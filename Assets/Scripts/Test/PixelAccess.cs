using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelAccess : MonoBehaviour
{
    Texture2D drawTexture;
    Color[] buffer;

    // Start is called before the first frame update
    void Start()
    {
        Texture2D mainTexture = (Texture2D)GetComponent<Renderer>().material.mainTexture;
        Color[] pixels = mainTexture.GetPixels();

        buffer = new Color[pixels.Length];
        pixels.CopyTo(buffer, 0);

        for(int y = 0; y < 256; ++y)
        {
            for(int x = 0; x < 256; ++x)
            {
                //buffer.SetValue(Color.black, x + 256 * y);
                buffer.SetValue(new Color(1.0f, 1.0f, 1.0f, 0.0f), x + 256 * y);
            }
        }

        drawTexture = new Texture2D(mainTexture.width, mainTexture.height, TextureFormat.RGBA32, false);
        drawTexture.filterMode = FilterMode.Point;

        drawTexture.SetPixels(buffer);
        drawTexture.Apply();
        GetComponent<Renderer>().material.mainTexture = drawTexture;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit , 100.0f))
            {
                Draw(hit.textureCoord * 256);
            }

            drawTexture.SetPixels(buffer);
            drawTexture.Apply();
            GetComponent<Renderer>().material.mainTexture = drawTexture;
        }

        if (Input.GetMouseButtonDown(1))
        {
            for (int y = 0; y < 256; ++y)
            {
                for (int x = 0; x < 256; ++x)
                {
                    if(buffer[x + 256 * y].a > 0.5f)
                    {
                        Color c = Color.blue;
                        c.a = 0.5f;
                        buffer.SetValue(c, x + 256 * y);
                    }
                }
            }
            drawTexture.SetPixels(buffer);
            drawTexture.Apply();
            GetComponent<Renderer>().material.mainTexture = drawTexture;
        }
    }

    public void Draw(Vector2 p)
    {
        for(int x = 0; x < 256; ++x)
        {
            for(int y = 0; y < 256; ++y)
            {
                if((p - new Vector2(x, y)).magnitude < 5)
                {
                    buffer.SetValue(Color.black, x + 256 * y);
                }
            }
        }
    }
}
