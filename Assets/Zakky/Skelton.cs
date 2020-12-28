using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Skelton : MonoBehaviour
{
    [SerializeField]
    Transform mCameraTrans;
    [SerializeField]
    float mMovingTime = 100f;
    [SerializeField]
    float mRunningSpeed = 5f;
    [SerializeField]
    float mWalkingSpeed = 0.5f;
    [SerializeField]
    AudioClip[] mGetaSFX;
    [SerializeField]
    float mTimeTillRun = 20f;

    float mDisTanceToCamera;
    Vector3 IniPos;

    enum SkeletonState
    {
        Walk,
        Run,
        Idle
    }

    SkeletonState mSkeletonState;
    [SerializeField]
    SkeletonState mWatchingSkeletonState = SkeletonState.Walk;

    ZakkyLib.Timer[] mTimer = new ZakkyLib.Timer[System.Enum.GetNames(typeof(SkeletonState)).Length];
    ZakkyLib.Timer mGetaTimer;

    Animator mAnimator;
    AudioSource mAudioSource;

    // Start is called before the first frame update
    void Start()
    {
        {
            Vector3 vecCamera = mCameraTrans.position;
            vecCamera.y = transform.position.y;
            Vector3 vecToCamera = vecCamera - transform.position;

            var look = Quaternion.LookRotation(vecToCamera);
            transform.localRotation = look;

            mDisTanceToCamera = (vecToCamera).magnitude;

            IniPos = transform.position;
        }

        mSkeletonState = SkeletonState.Walk;
        for (int i = 0; i < System.Enum.GetNames(typeof(SkeletonState)).Length; i++)
        {
            float t;
            if (i == (int)SkeletonState.Walk) t = mTimeTillRun + Random.Range(0f, 5f);
            else t = 99999f;
            mTimer[i] = new ZakkyLib.Timer(t);
        }
        //mTimer[(int)SkeletonState.Walk] = new ZakkyLib.Timer(mTimeTillRun + Random.Range(0f, 5f));
        //mTimer[(int)SkeletonState.Run] = new ZakkyLib.Timer(99999f);
        //mTimer[(int)SkeletonState.Idle] = new ZakkyLib.Timer(99999f);

        mGetaTimer = new ZakkyLib.Timer(1f / SpeedCoff());

        mAnimator = GetComponent<Animator>();
        mAudioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        StateChange();

        SetStateIntoAnimator();

        SkeletonMove();

        PlayGetaSFX();
    }

    float SpeedCoff()
    {
        if (mSkeletonState == SkeletonState.Run) return Mathf.Max(0f, mRunningSpeed);
        if (mSkeletonState == SkeletonState.Walk) return Mathf.Max(0f, mWalkingSpeed);
        return 0.0001f;
    }

    void StateChange()
    {
        if (mSkeletonState == SkeletonState.Walk && mTimer[(int)SkeletonState.Walk].IsTimeout())
        {
            SetSkeletonState(SkeletonState.Run);
        }

        {
            float tmpRotY = mCameraTrans.rotation.eulerAngles.y;
            while (tmpRotY < 0f) tmpRotY += 360f;
            tmpRotY %= 360f;
            switch (mWatchingSkeletonState)
            {
                case SkeletonState.Walk:
                    if (mSkeletonState != SkeletonState.Walk && tmpRotY == 180f)
                    {
                        SetWalkState();
                    }
                    break;
                case SkeletonState.Idle:
                    if (mSkeletonState != SkeletonState.Idle && tmpRotY == 180f)
                    {
                        SetIdleState();
                    }
                    else if (mSkeletonState == SkeletonState.Idle && tmpRotY != 180f)
                    {
                        SetWalkState();
                    }
                    break;
            }
        }
    }

    void SetStateIntoAnimator()
    {
        mAnimator.SetBool("IsWalking", mSkeletonState == SkeletonState.Walk);
        mAnimator.SetBool("IsRunning", mSkeletonState == SkeletonState.Run);
        mAnimator.SetBool("IsIdle", mSkeletonState == SkeletonState.Idle);
    }

    private void SkeletonMove()
    {
        Vector3 vec = mCameraTrans.position - transform.position;
        vec.y = 0f;
        transform.position += vec.normalized * mDisTanceToCamera * SpeedCoff() * (Time.deltaTime / mMovingTime);
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
        Debug.Log(mTimer[(int)SkeletonState.Walk].GetLimitTime());
    }

    public void SetIdleState()
    {
        mSkeletonState = SkeletonState.Idle;
        mTimer[(int)SkeletonState.Idle].TimeReset();
        mGetaTimer = new ZakkyLib.Timer(1f / SpeedCoff());
        Debug.Log(mTimer[(int)SkeletonState.Walk].GetLimitTime());

    }

    void PlayGetaSFX()
    {
        if (mGetaTimer.IsTimeout())
        {
            mAudioSource.PlayOneShot(mGetaSFX[Random.Range(0, mGetaSFX.Length)]);
            mGetaTimer.TimeReset();
        }
    }

    public void SkeletonPosReset()
    {
        transform.position = IniPos;
    }

    public void SleketonPosReset(float z)
    {
        transform.position += new Vector3(0f, 0f, z);
    }
}