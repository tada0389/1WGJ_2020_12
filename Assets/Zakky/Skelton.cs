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
    [SerializeField]
    float mRunningSpeed = 5f;
    [SerializeField]
    AudioClip[] mGetaSFX;

    float mDisTanceToCamera;
    Vector3 IniPos;

    enum SkeletonState
    {
        Walk,
        Run
    }

    SkeletonState mSkeletonState;

    ZakkyLib.Timer[] mTimer = new ZakkyLib.Timer[System.Enum.GetNames(typeof(SkeletonState)).Length];
    ZakkyLib.Timer mGetaTimer;

    Animator mAnimator;
    AudioSource mAudioSource;
    // Start is called before the first frame update
    void Start()
    {
        //mCameraTrans = GameObject.Find("MainCamera").transform;

        Vector3 vec = mCameraTrans.position;
        vec.y = transform.position.y;
        var look = Quaternion.LookRotation(vec - transform.position);
        transform.localRotation = look;
        IniPos = transform.position;

        mDisTanceToCamera = (vec - transform.position).magnitude;

        mSkeletonState = SkeletonState.Walk;
        mTimer[(int)SkeletonState.Walk] = new ZakkyLib.Timer(3f + Random.Range(0f, 5f));
        mTimer[(int)SkeletonState.Run] = new ZakkyLib.Timer(99999f);

        mGetaTimer = new ZakkyLib.Timer(1f / SpeedCoff());

        mAnimator = GetComponent<Animator>();
        mAudioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SetWalkState();
        }

        StateChange();

        SetStateIntoAnimator();

        SkeletonMove();

        if (mGetaTimer.IsTimeout())
        {
            mAudioSource.PlayOneShot(mGetaSFX[Random.Range(0, mGetaSFX.Length)]);
            mGetaTimer.TimeReset();
        }
    }

    float SpeedCoff()
    {
        if (mSkeletonState == SkeletonState.Run) return Mathf.Max(1f, mRunningSpeed);
        if (mSkeletonState == SkeletonState.Walk) return 1f;
        return 0f;
    }

    void StateChange()
    {
        if (mSkeletonState == SkeletonState.Walk && mTimer[(int)SkeletonState.Walk].IsTimeout())
        {
            SetSkeletonState(SkeletonState.Run);
        }
    }

    void SetStateIntoAnimator()
    {
        mAnimator.SetBool("IsWalking", mSkeletonState == SkeletonState.Walk);
    }

    private void SkeletonMove()
    {
        Vector3 vec = mCameraTrans.position - transform.position;
        vec.y = 0f;
        transform.position += vec.normalized * mDisTanceToCamera * SpeedCoff() * (Time.deltaTime / mMovingTime);
        Debug.Log(mDisTanceToCamera);
    }

    void SetSkeletonState(SkeletonState s)
    {
        mSkeletonState = s;
        mTimer[(int)s].TimeReset();
        mGetaTimer = new ZakkyLib.Timer(1f / SpeedCoff());
    }
    public void SetWalkState()
    {
        mSkeletonState = SkeletonState.Walk;
        mTimer[(int)SkeletonState.Walk].TimeReset();
        mGetaTimer = new ZakkyLib.Timer(1f / SpeedCoff());
    }

    public void SkeltonPosReset()
    {
        transform.position = IniPos;
    }
}