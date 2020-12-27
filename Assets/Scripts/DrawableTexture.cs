using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawableTexture : TextureBase
{
    [SerializeField]
    private float thickness_ = 5.0f;

    [SerializeField]
    private int undoEnableNum_ = 3;

    private Texture2D targetTexture_;

    public int Width => targetTexture_.width;
    public int Height => targetTexture_.height;

    private Vector2 prevMousePos;

    private bool prevClicked_;

    private List<Color[]> undoBuffer_;

    private bool isLocked_;

    // Start is called before the first frame update
    private void Start()
    { 
        Texture2D mainTexture = (Texture2D)GetComponent<Renderer>().material.mainTexture;
        Color[] pixels = mainTexture.GetPixels();

        Buffer_ = new Color[pixels.Length];

        targetTexture_ = new Texture2D(mainTexture.width, mainTexture.height, TextureFormat.RGBA32, false);
        targetTexture_.filterMode = FilterMode.Point;

        undoBuffer_ = new List<Color[]>(undoEnableNum_);

        Reset();
    }


    // Update is called once per frame
    private void Update()
    {
        if (isLocked_) return;

        if (Input.GetMouseButton(0))
        {
            if (Input.GetMouseButtonDown(0))
            {
                Color[] tmp = new Color[Buffer_.Length];
                System.Array.Copy(Buffer_, tmp, Buffer_.Length);
                undoBuffer_.Add(tmp);

                if(undoBuffer_.Count == undoEnableNum_ + 1)
                {
                    var unused = undoBuffer_[0];
                    unused = null;
                    undoBuffer_.RemoveAt(0);
                }
            }

            Vector2 to = Vector2.zero;
            Vector2 from = Vector2.zero;

            bool hit = ConvertTexcoordPos(Input.mousePosition, ref to);

            if (hit)
            {
                if (prevClicked_)
                {
                    bool prevHit = ConvertTexcoordPos(prevMousePos, ref from);
                    // 二分探索で境目を探す
                    if (!prevHit)         // 前回範囲外、今回範囲内
                    {
                        float left = 0.0f;
                        float right = 1.0f;

                        while(right - left > 1e-3)
                        {
                            float mid = (right + left) / 2.0f;
                            Vector2 tmp = prevMousePos + ((Vector2)Input.mousePosition - prevMousePos) * mid;

                            if (!ConvertTexcoordPos(tmp, ref from)) left = mid;
                            else right = mid;
                        }

                        Vector2 edgePos = prevMousePos + ((Vector2)Input.mousePosition - prevMousePos) * right;
                        ConvertTexcoordPos(edgePos, ref from);
                    }
                }
                else    // これが最初のタッチ
                {
                    from = to;
                }

                Draw(from, to);
                ApplyTexture();
            }
            else if(prevClicked_)
            {
                bool prevHit = ConvertTexcoordPos(prevMousePos, ref from);
                if (prevHit)    // 前回範囲内、今回範囲外
                {
                    // 二分探索で境目を探す

                    float left = 0.0f;
                    float right = 1.0f;

                    while (right - left > 1e-3)
                    {
                        float mid = (right + left) / 2.0f;
                        Vector2 tmp = prevMousePos + ((Vector2)Input.mousePosition - prevMousePos) * mid;

                        if (ConvertTexcoordPos(tmp, ref to)) left = mid;
                        else right = mid;
                    }

                    Vector2 edgePos = prevMousePos + ((Vector2)Input.mousePosition - prevMousePos) * left;
                    ConvertTexcoordPos(edgePos, ref to);


                    Draw(from, to);
                    ApplyTexture();
                }
            }

            prevClicked_ = true;
            prevMousePos = Input.mousePosition;
        }
        else
        {
            prevClicked_ = false;

            if (Input.GetMouseButtonDown(1)) Undo();
        }
    }

    private bool ConvertTexcoordPos(Vector3 inPos, ref Vector2 outPos)
    {
        Ray ray = Camera.main.ScreenPointToRay(inPos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100.0f))
        {
            outPos = hit.textureCoord * targetTexture_.width;
            return true;
        }

        return false;
    }

    private void Undo()
    {
        Debug.Log(undoBuffer_.Count);
        if (undoBuffer_.Count == 0) return;

        Buffer_ = null;
        Buffer_ = undoBuffer_[undoBuffer_.Count - 1];
        undoBuffer_.RemoveAt(undoBuffer_.Count - 1);

        ApplyTexture();
    }

    private void Draw(Vector2 from, Vector2 to)
    {
        float length = (to - from).magnitude;
        // 何等分すべきか
        int divNum = (int)(length / thickness_) + 1;

        int intThickness = (int)thickness_;

        for(int i = 0; i <= divNum; ++i)
        {
            float t = (float)(i) / divNum;
            Vector2 pos = from + (to - from) * t;

            // これの近くのを塗る
            for(int dy = -intThickness; dy <= intThickness; ++dy)
            {
                int y = (int)pos.y + dy;

                if (y < 0 || y >= targetTexture_.height) continue;

                for(int dx = -intThickness; dx <= intThickness; ++dx)
                {
                    int x = (int)pos.x + dx;

                    if (x < 0 || x >= targetTexture_.width) continue;

                    // これが近くかどうか
                    if(lineToPointDistance(from, to, new Vector2(x, y)) < thickness_)
                    {
                        Buffer_.SetValue(drawColor_, x + y * targetTexture_.width);
                    }
                }
            }
        }
    }

    private void ApplyTexture()
    {
        targetTexture_.SetPixels(Buffer_);
        targetTexture_.Apply();
        GetComponent<Renderer>().material.mainTexture = targetTexture_;
    }

    public override void ChangeColor()
    {
        isLocked_ = true;

        for(int y = 0; y < targetTexture_.height; ++y)
        {
            for (int x = 0; x < targetTexture_.width; ++x)
            {
                if (Buffer_[x + targetTexture_.width * y].a > 0.75f)
                    Buffer_.SetValue(answerColor_, x + targetTexture_.width * y);
                else
                    Buffer_.SetValue(new Color(1, 1, 1, 0.0f), x + targetTexture_.width * y);
            }
        }

        ApplyTexture();
    }

    public override void Reset(int level = 0)
    {
        // 白紙に戻す
        undoBuffer_.Clear();

        // 色の初期化
        for (int y = 0; y < targetTexture_.height; ++y)
        {
            for (int x = 0; x < targetTexture_.width; ++x)
            {
                Buffer_.SetValue(new Color(1, 1, 1, 0.5f), x + targetTexture_.width * y);
            }
        }

        ApplyTexture();

        prevClicked_ = false;
        isLocked_ = false;
    }

    //-----------------------------------------------------------------------
    //! 点にもっとも近い直線上の点(isSegmentがtrueで線分判定)
    //-----------------------------------------------------------------------
    private Vector2 nearestPointOnLine(Vector2 p1, Vector2 p2, Vector2 p, bool isSegment = true)
    {
        Vector2 d = p2 - p1;
        if (d.sqrMagnitude == 0) return p1;
        float t = (d.x * (p - p1).x + d.y * (p - p1).y) / d.sqrMagnitude;
        if (isSegment)
        {
            if (t < 0) return p1; if (t > 1) return p2;
        }
        Vector2 c = new Vector2((1 - t) * p1.x + t * p2.x, (1 - t) * p1.y + t * p2.y);
        return c;
    }

    //-----------------------------------------------------------------------
    //! 直線と点の距離(isSegmentがtrueで線分判定)
    //-----------------------------------------------------------------------
    private float lineToPointDistance(Vector2 p1, Vector2 p2, Vector2 p, bool isSegment = true)
    {
        return (p - nearestPointOnLine(p1, p2, p, isSegment)).magnitude;
    }
}
