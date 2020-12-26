using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TextureBase : MonoBehaviour
{
    [SerializeField]
    protected Color drawColor_ = Color.black;

    [SerializeField]
    protected Color answerColor_ = Color.blue - new Color(0, 0, 0, 0.5f);

    public Color[] Buffer_ { protected set; get; }

    public abstract void ChangeColor();

    public abstract void Reset();
}
