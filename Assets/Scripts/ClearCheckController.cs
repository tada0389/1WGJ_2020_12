using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using NCMB;

public class ClearCheckController : MonoBehaviour
{
    [SerializeField]
    private int doorNum_ = 5;

    [SerializeField]
    private int bossDoorNum_ = 1;

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

    [SerializeField]
    private CanvasGroup canvasGroup_;

    [SerializeField]
    private TextMeshProUGUI remainDoorText_;

    [SerializeField]
    private TextMeshProUGUI scoreText_;

    [SerializeField]
    private TextureGallery galleryCtrl_;

    private Vector3 defaultPos_;

    private bool isCheking_ = false;

    private int clearNum_ = 0;

    private float prevTime_;

    private int curScore_;

    // Start is called before the first frame update
    private IEnumerator Start()
    {
        leftParent_ = leftTexture_.transform.parent;
        rightParent_ = rightTexture_.transform.parent;

        defaultPos_ = leftParent_.transform.position;

        prevTime_ = Time.time;
        curScore_ = 0;

        remainDoorText_.text = "残り枚数\n" + (doorNum_ + bossDoorNum_).ToString();
        scoreText_.text = "SCORE\n0";

        isCheking_ = true;
        leftParent_.position += new Vector3(0f, moveY_, 0f);
        rightParent_.position += new Vector3(0f, moveY_, 0f);

        // 始めは扉が開いてスタート

        doorAnimation_.Play("Open", 0, 0.0f);

        // 文字がフェードイン
        canvasGroup_.DOFade(1.0f, 2.5f).SetEase(Ease.InCubic);

        yield return new WaitForSeconds(moveDuration_ + 0.1f);

        // 鍵を戻す
        leftParent_.DOMoveY(defaultPos_.y, moveDuration_);
        rightParent_.DOMoveY(defaultPos_.y, moveDuration_);

        yield return new WaitForSeconds(moveDuration_);

        prevTime_ = Time.time;

        isCheking_ = false;
    }

    public void CheckStart()
    {
        if (isCheking_) return;
        StartCoroutine(Check());
    }

    private IEnumerator Check()
    {
        float curTime = Time.time;

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
            () => text_.text = System.String.Format("{0:p2}", tmpRate));

        yield return new WaitForSeconds(0.75f);

        // エフェクトを表示
        if (isClear) ;
        else;

        yield return new WaitForSeconds(0.25f);

        // 鍵を下に隠す
        leftParent_.DOMoveY(defaultPos_.y + moveY_, moveDuration_);
        rightParent_.DOMoveY(defaultPos_.y + moveY_, moveDuration_);

        // 正解ならドアを開ける
        if (isClear)
        {
            doorAnimation_.Play("Open", 0, 0.0f);
            int score = CalcScore(curTime - prevTime_, accuRate);
            int tmpScore = curScore_;
            curScore_ += score;
            ++clearNum_;

            remainDoorText_.text = "残り枚数\n" + (doorNum_ + bossDoorNum_ - clearNum_).ToString();
            scoreText_.text = "SCORE\n0";

            DOTween.To(() => tmpScore, (n) => tmpScore = n, curScore_, 1.0f).OnUpdate(
                () => scoreText_.text = "SCORE\n" + (tmpScore).ToString() + "\n<color=red>+" + score.ToString() + "</color>")
                .OnComplete(() => scoreText_.text = "SCORE\n" + curScore_.ToString());

            if (galleryCtrl_ != null) galleryCtrl_.AddTexture(leftTexture_.Buffer_);
        }

        yield return new WaitForSeconds(moveDuration_ + 0.1f);

        if (clearNum_ == (bossDoorNum_ + doorNum_))
        {
            // Type == Number の場合
            naichilab.RankingLoader.Instance.SendScoreAndShowRanking(curScore_);

            if (galleryCtrl_ != null) galleryCtrl_.Show();
        }
        else
        {
            // テクスチャ情報をリセットする
            leftTexture_.Reset();
            rightTexture_.Reset();

            // テキストを消す
            text_.DOFade(0.0f, 0.25f);

            // 鍵を戻す
            leftParent_.DOMoveY(defaultPos_.y, moveDuration_);
            rightParent_.DOMoveY(defaultPos_.y, moveDuration_);

            yield return new WaitForSeconds(moveDuration_);

            if(isClear) prevTime_ = Time.time;
        }

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

    private int CalcScore(float eclipsedTime, float accuRate)
    {
        // スコア = max(0.0f, 10.0f - 経過時間) * 500 + 一致率 * 5000

        return (int)(Mathf.Max(0.0f, 10.0f - eclipsedTime) * 500 + accuRate * 2500);
    }
}