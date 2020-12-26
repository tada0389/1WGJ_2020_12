using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class ClearCheckController : MonoBehaviour
{
    [SerializeField]
    private float clearThr_ = 0.6f;

    [SerializeField]
    private TextureBase leftTexture_;

    [SerializeField]
    private TextureBase rightTexture_;

    private Transform leftParent_;
    private Transform rightParent_;

    [SerializeField]
    private float moveDuration_ = 1.0f;

    [SerializeField]
    private float moveY_ = -5.0f;

    [SerializeField]
    private TextMeshProUGUI text_;

    [SerializeField]
    private Animator doorAnimation_;

    [SerializeField]
    private List<Ease> accuEases = new List<Ease>();

    private Vector3 defaultPos_;

    private bool isCheking_ = false;

    // Start is called before the first frame update
    void Start()
    {
        leftParent_ = leftTexture_.transform.parent;
        rightParent_ = rightTexture_.transform.parent;

        defaultPos_ = leftParent_.transform.position;
    }

    public void CheckStart()
    {
        if (isCheking_) return;
        StartCoroutine(Check());
    }

    private IEnumerator Check()
    {
        isCheking_ = true;

        leftTexture_.ChangeColor();
        rightTexture_.ChangeColor();

        float leftPos = leftParent_.position.x;
        float rightPos = rightParent_.position.x;

        // 正答率を求める
        float accuRate = CalcAccuracy();

        bool isClear = accuRate >= clearThr_;

        // 正答率の文字の表示
        text_.text = System.String.Format("{0:p2}", 0.0);
        text_.DOFade(1.0f, 0.25f);

        yield return new WaitForSeconds(0.25f);

        // 正答率を表示
        float tmpRate = 0.0f;
        Ease useEase = accuEases[Random.Range(0, accuEases.Count)];
        // 徐々に上げる
        DOTween.To(() => tmpRate, (n) => tmpRate = n, accuRate, 0.75f).SetEase(useEase).OnUpdate(
            () => text_.text = System.String.Format("{0:p2}", tmpRate)) ;

        yield return new WaitForSeconds(0.75f);

        // エフェクトを表示
        if (isClear) ;
        else;

        yield return new WaitForSeconds(0.25f);

        // 鍵を下に隠す
        leftParent_.DOMoveY(defaultPos_.y + moveY_, moveDuration_);
        rightParent_.DOMoveY(defaultPos_.y + moveY_, moveDuration_);

        // 正解ならドアを開ける
        if (isClear) doorAnimation_.Play("Open", 0, 0.0f);

        yield return new WaitForSeconds(moveDuration_ + 0.1f);

        // テクスチャ情報をリセットする
        leftTexture_.Reset();
        rightTexture_.Reset();

        // テキストを消す
        text_.DOFade(0.0f, 0.25f);

        // 鍵を戻す
        leftParent_.DOMoveY(defaultPos_.y, moveDuration_);
        rightParent_.DOMoveY(defaultPos_.y, moveDuration_);

        isCheking_ = false;
    }

    private float CalcAccuracy()
    {
        // 正答率の計算
        var leftBuffer = leftTexture_.Buffer_;
        var rightBuffer = rightTexture_.Buffer_;

        int correctNum = 0;
        int sum = 0;

        for (int i = 0; i < leftBuffer.Length; ++i)
        {
            bool left = leftBuffer[i].a >= 0.2f;
            bool right = rightBuffer[i].a >= 0.2f;

            if (!left && !right) continue;

            ++sum;

            if (left && right) ++correctNum;
        }

        float correctRate = (float)correctNum / sum;

        correctRate /= 0.95f;

        return Mathf.Min(1.0f, Mathf.Sqrt(correctRate));
    }
}
