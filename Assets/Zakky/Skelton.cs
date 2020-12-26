using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Skelton : MonoBehaviour
{
    [SerializeField]
    Transform mCameraTrans;
    [SerializeField]
    float mMovingTime;

    enum SkeltonState
    {
        Walk,
        Run,
        Dead
    }

    [SerializeField]
    SkeltonState mSkeltonState = SkeltonState.Walk;

    ZakkyLib.Timer[] mTimer = new ZakkyLib.Timer[System.Enum.GetNames(typeof(SkeltonState)).Length];

    Animator mAnimator;
    // Start is called before the first frame update
    void Start()
    {
        Vector3 vec = mCameraTrans.position;
        vec.y = transform.position.y;
        var look = Quaternion.LookRotation(vec - transform.position);
        transform.localRotation = look;
        transform.DOMove(vec,mMovingTime);

        mTimer[(int)SkeltonState.Walk] = new ZakkyLib.Timer(3f);
        mTimer[(int)SkeltonState.Run] = new ZakkyLib.Timer(2f);

        mAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        StateChange();

        SetStateIntoAnimator();
    }

    void StateChange()
    {
        if (mSkeltonState == SkeltonState.Walk && mTimer[(int)SkeltonState.Walk].IsTimeout())
        {
            mSkeltonState = SkeltonState.Run;
            var t = mTimer[(int)SkeltonState.Run];
            t = new ZakkyLib.Timer(t.GetLimitTime());
        }
        else if (mSkeltonState == SkeltonState.Run && mTimer[(int)SkeltonState.Run].IsTimeout())
        {
            mSkeltonState = SkeltonState.Walk;
            var t = mTimer[(int)SkeltonState.Walk];
            t = new ZakkyLib.Timer(t.GetLimitTime());
        }
    }

    void SetStateIntoAnimator()
    {
        mAnimator.SetBool("IsWalking", mSkeltonState == SkeltonState.Walk);
        mAnimator.SetBool("IsDead", mSkeltonState == SkeltonState.Dead);
    }

    public void DaedSetter()
    {
        mSkeltonState = SkeltonState.Dead;
    }
}