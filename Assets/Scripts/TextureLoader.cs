using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Assertions;

public class TextureLoader : MonoBehaviour
{
    private string directoryPath_;

    private void Awake()
    {
        directoryPath_ = "File";
    }

    public Color[] Load(Color drawColor, int keyIndex, int width, int height)
    {
        Color[] ret = new Color[width * height];

        string filePath = directoryPath_ + @"\key" + keyIndex.ToString();
        Debug.Log(filePath);

        var file = Resources.Load(filePath) as TextAsset;

        // 解凍
        string str = file.text;

        int length = str.Length;

        int cur = 0;

        for(int i = 0; i < length; ++i)
        {
            int to = i + 1;
            for(int j = i + 1; j < length; ++j)
            {
                if (str[j] == 'b' || str[j] == 'w') break;

                to = j;
            }

            string tmp = str.Substring(i + 1, to - i);
            int len = int.Parse(tmp);

            for (int j = 0; j < len; ++j)
            {
                ret[cur++] = (str[i] == 'b')? drawColor : new Color(1, 1, 1, 0.0f);
            }

            i += tmp.Length;
        }

        return ret;
    }
}
