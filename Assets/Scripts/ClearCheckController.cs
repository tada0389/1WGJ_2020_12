using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NCMB;

public class ClearCheckController : MonoBehaviour
{
    [SerializeField]
    private TextureBase leftTexture_;

    [SerializeField]
    private TextureBase rightTexture_;

    private Transform leftParent_;
    private Transform rightParent_;

    [SerializeField]
    private float moveDuration_ = 1.0f;

    [SerializeField]
    private Text text_;

    private bool isCheking_ = false;

    // Start is called before the first frame update
    void Start()
    {
        leftParent_ = leftTexture_.transform.parent;
        rightParent_ = rightTexture_.transform.parent;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)) UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        if (isCheking_) return;

        if (Input.GetMouseButtonDown(1))
        {
            StartCoroutine(Check());
        }
    }

    private IEnumerator Check()
    {
        isCheking_ = true;

        leftTexture_.ChangeColor();
        rightTexture_.ChangeColor();

        float leftPos = leftParent_.position.x;
        float rightPos = rightParent_.position.x;

        float time = 0.0f;

        while(time < moveDuration_)
        {
            time += Time.deltaTime;

            float t = Mathf.Min(1.0f, time / moveDuration_);

            leftParent_.position = new Vector3(leftPos - (leftPos * t), 0f, 0f);
            rightParent_.position = new Vector3(rightPos - (rightPos * t), 0f, 0f);

            yield return null;
        }

        //ちょっと待つ

        yield return new WaitForSeconds(1.0f);

        // 正答率の計算
        var leftBuffer = leftTexture_.Buffer_;
        var rightBuffer = rightTexture_.Buffer_;

        int correctNum = 0;
        int sum = 0;

        for(int i = 0; i < leftBuffer.Length; ++i)
        {
            bool left = leftBuffer[i].a >= 0.2f;
            bool right = rightBuffer[i].a >= 0.2f;

            if (!left && !right) continue;

            ++sum;

            if (left && right) ++correctNum;
        }

        float correctRate = (float)correctNum / sum * 100.0f;

        text_.text = correctRate.ToString() + "%";

        /*
        NCMBObject testObj = new NCMBObject("TestClass");
        testObj["message"] = correctRate.ToString();
        testObj.SaveAsync();
        */

        // Type == Number の場合
        naichilab.RankingLoader.Instance.SendScoreAndShowRanking(correctRate);

        yield return new WaitForSeconds(1.5f);

        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);

        //isCheking_ = false;
    }
}
