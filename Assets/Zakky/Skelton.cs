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

    // Start is called before the first frame update
    void Start()
    {
        //vec = mCameraTrans.position - transform.position;
        Vector3 vec = mCameraTrans.position;
        vec.y = transform.position.y;
        var look = Quaternion.LookRotation(vec);
        transform.localRotation = look;
        transform.DOMove(vec,mMovingTime);
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position += vec * (Time.deltaTime / mMovingTime);
    }
}
