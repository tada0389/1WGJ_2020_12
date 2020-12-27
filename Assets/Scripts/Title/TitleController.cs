using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TitleController : MonoBehaviour
{
    public static bool isPoseEffectEnabled = true;

    [SerializeField]
    private MessageManager messageManager_;

    [SerializeField]
    private CanvasGroup canvasGroup_;

    [SerializeField]
    private GameObject postEffectVolume_;

    [SerializeField]
    private RectTransform onButton_;

    [SerializeField]
    private RectTransform offButton_;

    [SerializeField]
    private GameObject ruleCloseButton_;

    [Multiline]
    [SerializeField]
    private string ruleExplonation_ = "ルール説明";

    public void OnPostEffectOnButtonClicked()
    {
        if (postEffectVolume_.activeSelf) return;

        onButton_.DOKill();
        offButton_.DOKill();
        onButton_.DOScale(1.3f, 0.25f);
        offButton_.DOScale(0.9f, 0.25f);

        postEffectVolume_.SetActive(true);
        isPoseEffectEnabled = true;
    }

    public void OnPostEffectOffButtonClicked()
    {
        if (!postEffectVolume_.activeSelf) return;

        onButton_.DOKill();
        offButton_.DOKill();
        onButton_.DOScale(0.9f, 0.25f);
        offButton_.DOScale(1.3f, 0.25f);

        postEffectVolume_.SetActive(false);
        isPoseEffectEnabled = false;
    }

    public void OnRuleButtonClicked()
    {
        ruleCloseButton_.SetActive(true);
        messageManager_.OpenKanbanWindow(ruleExplonation_);
    }

    public void OnRuleCloseButtonClicked()
    {
        ruleCloseButton_.SetActive(false);
        messageManager_.CloseKanbanWindow();
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
