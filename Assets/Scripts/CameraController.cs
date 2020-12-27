using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class CameraController : MonoBehaviour
{
    private enum eDir
    {
        Forward,
        Backward,
    }

    private eDir curDir_;

    [SerializeField]
    private TextMeshProUGUI lookText_;

    [SerializeField]
    private float duration_ = 0.5f;

    [SerializeField]
    private Ease turnEase_ = Ease.Linear;

    [SerializeField]
    private Skelton skeltonCtrl_;

    private float time_;

    // Start is called before the first frame update
    void Start()
    {
        curDir_ = eDir.Forward;
    }

    public void OnTurnAroundButtonClicked()
    {
        float prevDif = Time.time - time_;
        float newDuration = duration_ - Mathf.Max(0f, duration_ - prevDif);

        if(curDir_ == eDir.Forward)
        {
            curDir_ = eDir.Backward;

            // 先頭を向く
            transform.DOKill();
            transform.DORotate(new Vector3(0f, 180f, 0f), newDuration).SetEase(turnEase_); 

            lookText_.text = "TURN";
        }
        else
        {
            curDir_ = eDir.Forward;

            // 後ろを見る
            transform.DOKill();
            transform.DORotate(new Vector3(0f, 0f, 0f), newDuration).SetEase(turnEase_);

            lookText_.text = "BACK";
        }

        time_ = Time.time;
    }
}
