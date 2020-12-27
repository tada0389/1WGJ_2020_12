using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GameOver : MonoBehaviour
{
    [SerializeField]
    private float sceneTransitionTime_ = 2.0f;

    [SerializeField]
    private UnityEngine.UI.Image antenImage_;

    // Start is called before the first frame update
    private IEnumerator Start()
    {

        yield return new WaitForSeconds(sceneTransitionTime_ / 2.0f);

        antenImage_.DOFade(1.0f, sceneTransitionTime_ / 2.0f);

        yield return new WaitForSeconds(sceneTransitionTime_ / 2.0f);

        UnityEngine.SceneManagement.SceneManager.LoadScene("Title");
    }
}
