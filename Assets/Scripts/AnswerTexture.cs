using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnswerTexture : TextureBase
{
    private TextureLoader texLoader_;

    [SerializeField]
    private int keyNum_ = 3;

    private Texture2D targetTexture_;

    public int Width => targetTexture_.width;
    public int Height => targetTexture_.height;

    // Start is called before the first frame update
    private void Start()
    {
        texLoader_ = GetComponent<TextureLoader>();

        Texture2D mainTexture = (Texture2D)GetComponent<Renderer>().material.mainTexture;
        Color[] pixels = mainTexture.GetPixels();

        targetTexture_ = new Texture2D(mainTexture.width, mainTexture.height, TextureFormat.RGBA32, false);
        targetTexture_.filterMode = FilterMode.Point;

        Reset(0);
    }

    private Color[] Load(int level)
    {
        return texLoader_.Load(drawColor_, level, Random.Range(0, keyNum_), Width, Height);
    }

    private void ApplyTexture()
    {
        targetTexture_.SetPixels(Buffer_);
        targetTexture_.Apply();
        GetComponent<Renderer>().material.mainTexture = targetTexture_;
    }

    public override void ChangeColor()
    {
        for (int y = 0; y < targetTexture_.height; ++y)
        {
            for (int x = 0; x < targetTexture_.width; ++x)
            {
                if (Buffer_[x + targetTexture_.width * y].a > 0.25f)
                    Buffer_.SetValue(answerColor_, x + targetTexture_.width * y);
                else
                    Buffer_.SetValue(new Color(1, 1, 1, 0.0f), x + targetTexture_.width * y);
            }
        }

        ApplyTexture();
    }

    public override void Reset(int level)
    {
        // 鍵データを読み込む
        Buffer_ = Load(level);

        ApplyTexture();
    }
}
