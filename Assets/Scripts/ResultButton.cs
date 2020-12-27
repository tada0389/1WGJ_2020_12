using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultButton : MonoBehaviour
{ 
    public void OnRankingButtonClicked()
    {
        naichilab.RankingLoader.Instance.SendScoreAndShowRanking(ClearCheckController.curScore_);
    }

    public void OnHomeButtonClicked()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Title");
    }
}
