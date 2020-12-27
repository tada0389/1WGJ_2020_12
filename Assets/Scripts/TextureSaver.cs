using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;

public class TextureSaver : MonoBehaviour
{
    [SerializeField]
    private DrawableTexture drawCtrl_;

    [SerializeField]
    private int keyLevel_ = 0;

    [SerializeField]
    private int keyIndex_ = 0;

    private string filePath_;

    [SerializeField]
    private UnityEngine.UI.Slider levelSlider_;
    [SerializeField]
    private TextMeshProUGUI levelText_;

    [SerializeField]
    private UnityEngine.UI.Slider indexSlider_;
    [SerializeField]
    private TextMeshProUGUI indexText_;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            Save();
        }
    }

    public void ChangeLevel()
    {
        int i = (int)levelSlider_.value;
        keyLevel_ = i;
        levelText_.text = "LEVEL : " + keyLevel_.ToString();
    }

    public void ChangeKeyIndex()
    {
        int i = (int)indexSlider_.value;
        keyIndex_ = i;
        indexText_.text = "LEVEL : " + keyIndex_.ToString();
    }

    private void Save()
    {
        string directoryPath = Application.dataPath + @"\Resources\File\Level" + keyLevel_.ToString();
        filePath_ = directoryPath + @"\key" + keyIndex_.ToString() + ".txt";

        if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);
        if (!File.Exists(filePath_)) File.Create(filePath_);

        string str = "";
        int width = drawCtrl_.Width;
        int height = drawCtrl_.Height;

        //str += width.ToString() + "\n";
        //str += height.ToString() + "\n";

        var buffer = drawCtrl_.Buffer_;

        string prev = ConvertToString(buffer[0].a);
        int cnt = 0;

        // ピクセル情報を入力
        for(int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                string add = ConvertToString(buffer[x + y * width].a);
                if (add == prev)
                {
                    ++cnt;
                }
                else
                {
                    str += prev + cnt.ToString();
                    prev = add;
                    cnt = 1;
                }
            }
        }

        str += prev + cnt.ToString();

        // 書き込み
        File.WriteAllText(filePath_, str);

        Debug.Log("セーブしました @" + filePath_);

        //drawCtrl_.Reset();
    }

    private string ConvertToString(float alpha)
    {
        if (alpha > 0.5f) return "b";
        else return "w";
    }
}
