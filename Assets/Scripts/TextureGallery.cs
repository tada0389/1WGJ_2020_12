using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureGallery : MonoBehaviour
{
    [SerializeField]
    private int maxReserveNum_ = 10;

    private List<Color[]> textures_;

    private void Start()
    {
        textures_ = new List<Color[]>();
    }

    public void AddTexture(Color[] buf)
    {
        textures_.Add(buf);
    }
}
