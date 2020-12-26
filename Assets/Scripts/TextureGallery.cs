using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureGallery : MonoBehaviour
{
    [SerializeField]
    private int maxReserveNum_ = 10;

    public List<Color[]> Textures_ { private set; get; }

    private void Start()
    {
        Textures_ = new List<Color[]>();
    }

    public void AddTexture(Color[] buf)
    {
        // もし許容数を超えたなら先頭のを削除
        if(Textures_.Count == maxReserveNum_)
        {
            var tex = Textures_[0];
            tex = null;
            Textures_.RemoveAt(0);
        }
        Textures_.Add(buf);
    }
}
