using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TitleController : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup canvasGroup_;

    public void OnOpenButtonClicked()
    {
        StartCoroutine(OpenFlow());
    }

    private IEnumerator OpenFlow()
    {
        canvasGroup_.DOFade(0.0f, 0.1f);

        yield return new WaitForSeconds(0.1f);

        UnityEngine.SceneManagement.SceneManager.LoadScene("MainGame");
    }
}
