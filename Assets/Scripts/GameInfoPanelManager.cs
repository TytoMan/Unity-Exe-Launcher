using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameInfoPanelManager : MonoBehaviour
{
    [SerializeField] GameInfoScriptableObject gameInfo;

    [Header("Texts")]
    [SerializeField] TextMeshProUGUI gameNameTMP;
    [SerializeField] TextMeshProUGUI creatorNameTMP;
    [SerializeField] TextMeshProUGUI explanationTMP;
    [SerializeField] TextMeshProUGUI staticticsTMP;

    [Header("Path Edit")]
    [SerializeField] TMP_InputField pathInputField;
    [SerializeField] Button pathEditButton;
    [SerializeField] Button pathEditDoneButton;

    public CanvasGroup canvasGroup;

    public void SetGameInfo(GameInfoScriptableObject gameInfoScriptableObject, int count, float time)
    {
        gameInfo = gameInfoScriptableObject;
        pathInputField.text = PlayerPrefs.GetString(gameInfo.gameName + "_Path", gameInfo.path);
        gameNameTMP.text = gameInfo.gameName;
        creatorNameTMP.text = gameInfo.creatorName;
        explanationTMP.text = gameInfo.explanation;
        staticticsTMP.text = count.ToString() + string.Format(" ({00:00.00})", time);
    }

    public void FadeIn(float time)
    {
        StartCoroutine(CanvasGroupManager.current.FadeCanvas(canvasGroup, 0, 1, time));
    }

    public void FadeOut(float time)
    {
        StartCoroutine(CanvasGroupManager.current.FadeCanvas(canvasGroup, 1, 0, time));
    }

    public void OnPlayButtonPressed()
    {
        StartCoroutine(CanvasGroupManager.current.RunGame(gameInfo));
    }

    public void OnPahtEditButtonPressed()
    {
        pathEditButton.gameObject.SetActive(false);
        pathInputField.gameObject.SetActive(true);
    }

    public void OnPathEditDoneButtonPressed()
    {
        if (gameInfo == null) return;
        gameInfo.path = pathInputField.text;
        PlayerPrefs.SetString(gameInfo.gameName + "_Path", gameInfo.path);
        PlayerPrefs.Save();
        pathEditButton.gameObject.SetActive(true);
        pathInputField.gameObject.SetActive(false);
    }

    // public void OnPathInputFieldChange(string text)
    // {
    //     gameInfo.path = text;
    //     PlayerPrefs.SetString(gameInfo.name, text);
    // }

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        // pathInputField.onValueChanged.AddListener(OnPathInputFieldChange);
        pathInputField.gameObject.SetActive(false);
    }
}
