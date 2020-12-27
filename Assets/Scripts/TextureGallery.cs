using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TextureGallery : MonoBehaviour
{
    [SerializeField]
    private GameObject gallery_;

    [SerializeField]
    private List<GameObject> textures_;

    private int cnt_;

    private void Start()
    {
        gallery_.SetActive(false);
        cnt_ = 0;
    }

    public void Show()
    {
        // スケールを変更
        gallery_.transform.localScale = Vector3.one * 0.25f;
        gallery_.SetActive(true);
        gallery_.transform.DOScale(Vector3.one, 1.0f).SetEase(Ease.InOutQuad);
    }

    // 正解した絵だけを保存
    public void AddTexture(Color[] buf)
    {
        Texture2D mainTexture = (Texture2D)textures_[cnt_].GetComponent<Renderer>().material.mainTexture;

        Texture2D targetTexture = new Texture2D(mainTexture.width, mainTexture.height, TextureFormat.RGBA32, false);
        targetTexture.filterMode = FilterMode.Point;

        // 縦横半分に圧縮する

        Color[] tmp = new Color[buf.Length / 4];

        for (int i = 0; i < buf.Length / 4; ++i) tmp[i] = Color.white;

        for(int i = 0; i < buf.Length; ++i)
        {
            int x = i % (mainTexture.height * 2);
            int y = i / (mainTexture.width * 2);

            int j = x / 2 + y / 2 * mainTexture.width;

            if (buf[i].a > 0.25f) tmp[j] -= new Color(0.25f, 0.25f, 0.25f, 0.0f);
        }

        targetTexture.SetPixels(tmp);
        targetTexture.Apply();
        textures_[cnt_++].GetComponent<Renderer>().material.mainTexture = targetTexture;
    }
}
