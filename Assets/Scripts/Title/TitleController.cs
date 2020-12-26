using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TitleController : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup canvasGroup_;

    [SerializeField]
    private GameObject postEffectVolume_;

    [SerializeField]
    private RectTransform onButton_;

    [SerializeField]
    private RectTransform offButton_;


    private void Start()
    {
        DontDestroyOnLoad(postEffectVolume_);
    }

    public void OnPostEffectOnButtonClicked()
    {
        if (postEffectVolume_.activeSelf) return;

        onButton_.DOKill();
        offButton_.DOKill();
        onButton_.DOScale(1.3f, 0.25f);
        offButton_.DOScale(0.9f, 0.25f);

        postEffectVolume_.SetActive(true);
    }

    public void OnPostEffectOffButtonClicked()
    {
        if (!postEffectVolume_.activeSelf) return;

        onButton_.DOKill();
        offButton_.DOKill();
        onButton_.DOScale(0.9f, 0.25f);
        offButton_.DOScale(1.3f, 0.25f);

        postEffectVolume_.SetActive(false);
    }

    public void OnRuleButtonClicked()
    {
        MessageManager.OpenKanbanWindow("ルール説明");
    }

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
