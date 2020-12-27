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
    private CanvasGroup resultCanvasGroup_;

    [SerializeField]
    private TextMeshProUGUI remainDoorText_;

    [SerializeField]
    private TextMeshProUGUI scoreText_;

    [SerializeField]
    private TextMeshProUGUI levelText_;

    [SerializeField]
    private TextureGallery galleryCtrl_;

    [SerializeField]
    private Skelton skeletonCtrl_;

    [SerializeField]
    private CameraController cameraCtrl_;


    private Vector3 textureDefaultPos_;
    private Vector3 levelTextDefaultPos_;

    private bool isCheking_ = false;

    private int clearNum_ = 0;

    private float prevTime_;

    public static int curScore_ { private set; get; }

    // Start is called before the first frame update
    private IEnumerator Start()
    {
        curScore_ = 0;

        leftParent_ = leftTexture_.transform.parent;
        rightParent_ = rightTexture_.transform.parent;

        textureDefaultPos_ = leftParent_.transform.position;
        levelTextDefaultPos_ = levelText_.rectTransform.position;

        prevTime_ = Time.time;
        curScore_ = 0;

        remainDoorText_.text = "残り枚数\n" + (doorNum_ + bossDoorNum_).ToString();
        scoreText_.text = "SCORE\n0";

        isCheking_ = true;
        leftParent_.position += new Vector3(0f, moveY_, 0f);
        rightParent_.position += new Vector3(0f, moveY_, 0f);

        // 始めは扉が開いてスタート

        Vector3 skeltonPos = skeletonCtrl_.transform.localPosition;
        skeltonPos.z -= 10.0f;
        skeletonCtrl_.transform.localPosition = skeltonPos;

        doorAnimation_.Play("Open", 0, 0.0f);

        // 文字がフェードイン
        canvasGroup_.DOFade(1.0f, 2.5f).SetEase(Ease.InCubic);

        yield return new WaitForSeconds(moveDuration_ + 0.1f);

        // 鍵を戻す
        leftParent_.DOMoveY(textureDefaultPos_.y, moveDuration_);
        rightParent_.DOMoveY(textureDefaultPos_.y, moveDuration_);

        // レベルテキストを表示
        // 座標を変更
        levelText_.rectTransform.position = levelTextDefaultPos_;
        levelText_.text = "LEVEL " + (clearNum_ + 1).ToString();
        levelText_.DOFade(1.0f, 0.5f);
        levelText_.rectTransform.DOMoveY(levelTextDefaultPos_.y + 50f, 0.75f).SetEase(Ease.OutSine);

        yield return new WaitForSeconds(moveDuration_);

        levelText_.DOFade(0.0f, 0.5f);

        prevTime_ = Time.time;

        isCheking_ = false;
    }

    private void Update()
    {
        if(skeletonCtrl_.transform.localPosition.z > -3f)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        StopAllCoroutines();

        // ゲームオーバーシーン
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameOver");
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
        bool isLast = (clearNum_ == (bossDoorNum_ + doorNum_ - 1));

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

        yield return new WaitForSeconds(1.0f);

        // 鍵を下に隠す
        leftParent_.DOMoveY(textureDefaultPos_.y + moveY_, moveDuration_);
        rightParent_.DOMoveY(textureDefaultPos_.y + moveY_, moveDuration_);

        // 正解ならドアを開ける
        if (isClear)
        {
            if (isLast) skeletonCtrl_.enabled = false;
            else
            {
                Vector3 skeltonPos = skeletonCtrl_.transform.localPosition;
                skeltonPos.z -= 10.0f;
                skeletonCtrl_.transform.localPosition = skeltonPos;
            }

            if (!isLast) doorAnimation_.Play("Open", 0, 0.0f);
            else doorAnimation_.Play("Chest");

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

        // クリア処理
        if (isClear && isLast)
        {
            cameraCtrl_.LookForward();

            yield return new WaitForSeconds(1.0f);

            // UIを消す
            canvasGroup_.DOFade(0.0f, 0.25f).OnComplete(() => canvasGroup_.gameObject.SetActive(false));

            yield return new WaitForSeconds(1.0f);

            if (galleryCtrl_ != null) galleryCtrl_.Show();

            yield return new WaitForSeconds(1.5f);

            // ランキングボタン、ホームボタンを出す
            resultCanvasGroup_.gameObject.SetActive(true);
            resultCanvasGroup_.DOFade(1.0f, 0.25f);
        }
        else
        {
            // テクスチャ情報をリセットする
            leftTexture_.Reset();
            rightTexture_.Reset(clearNum_);

            // テキストを消す
            text_.DOFade(0.0f, 0.25f);

            // 鍵を戻す
            leftParent_.DOMoveY(textureDefaultPos_.y, moveDuration_);
            rightParent_.DOMoveY(textureDefaultPos_.y, moveDuration_);

            if (isClear)
            {
                // レベルテキスト
                // 座標を変更
                bool isFinal = (clearNum_ == (doorNum_ + bossDoorNum_ - 1));
                string level = (isFinal) ? "FINAL" : (clearNum_ + 1).ToString();
                levelText_.rectTransform.position = levelTextDefaultPos_;
                levelText_.text = "LEVEL " + level;
                levelText_.DOFade(1.0f, 0.5f);
                levelText_.rectTransform.DOMoveY(levelTextDefaultPos_.y + 50f, 0.75f).SetEase(Ease.OutSine);
            }

            yield return new WaitForSeconds(moveDuration_);

            if(isClear)
                levelText_.DOFade(0.0f, 0.5f);

            if (isClear) prevTime_ = Time.time;

            isCheking_ = false;
        }
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