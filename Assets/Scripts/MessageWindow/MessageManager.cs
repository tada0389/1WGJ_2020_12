using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TadaLib;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MessageManager : MonoBehaviour
{
    [SerializeField]
    private MessageWindow messageWindow;

    [SerializeField]
    private MessageWindow kanbanWindow;

    private void Start()
    {
        //シーンが切り替わったときに閉じる
        SceneManager.sceneLoaded += Close;
    }

    public void OpenMessageWindow(string textStr)
    {
        messageWindow.gameObject.SetActive(true);
        messageWindow.WindowOpen(textStr);
        // Time.timeScale = 0.0f; // 変更 tada
    }

    public void OpenMessageWindow(string textStr, Sprite sprite)
    {
        messageWindow.gameObject.SetActive(true);
        messageWindow.WindowOpen(textStr, sprite);
        // Time.timeScale = 0.0f; // 変更 tada
    }

    public void CloseMessageWindow()
    {
        if (messageWindow == null) return;
        messageWindow.WindowClose(true);
    }

    public void OpenKanbanWindow(string textStr)
    {
        kanbanWindow.gameObject.SetActive(true);
        kanbanWindow.WindowOpen(textStr);
    }

    public void CloseKanbanWindow()
    {
        kanbanWindow.WindowClose();
    }

    public void InitMessage(string textStr)
    {
        messageWindow.MessageInit(textStr);
    }

    public bool isSending()
    {
        return messageWindow.isSending;
    }

    public void FinishMessage()
    {
        messageWindow.MessageFinish();
    }

    void Close(Scene nextScene, LoadSceneMode mode)
    {
        CloseMessageWindow();
        CloseKanbanWindow();
    }
}
