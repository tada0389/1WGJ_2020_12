using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostProcessController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (!TitleController.isPoseEffectEnabled) gameObject.SetActive(false);  
    }
}
