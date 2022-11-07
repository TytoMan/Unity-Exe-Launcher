using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CanvasGroupManager : MonoBehaviour
{
    public GameInfoScriptableObject[] gameInfos;

    public Image background;
    public LayoutGroup gameSelectLayout;
    public GameObject gameNamePrafab;
    public GameInfoPanelManager gameInfoPanelManagerPrefab;
    public CanvasGroup menu;
    public CanvasGroup note;
    public float fadeTime = .1f;
    public AnimationCurve fontTweenCurve;
    public TextMeshProUGUI noteTMP;
    [TextArea(1, 16)]
    public string noteText;

    public static CanvasGroupManager current;

    private Process process;
    private GameButtonManager[] gameButtonManagers;
    private GameInfoPanelManager[] gameInfoPanelManagers;
    private int currentGBMIndex;
    private int currentGIPMIndex;
    private float initialGBMFontSize;


    public void ChangeGameInfoPanel(GameInfoScriptableObject gameInfo, int index)
    {
        var startCount = PlayerPrefs.GetInt(gameInfo.gameName + "_Count", 0);
        var playTime = PlayerPrefs.GetFloat(gameInfo.gameName + "_Time", 0f);
        background.sprite = gameInfo.backgroundTexture;

        var oldTMP = gameButtonManagers[currentGBMIndex].textMesh;
        var newTMP = gameButtonManagers[index].textMesh;
        StartCoroutine(TweenText(oldTMP, oldTMP.fontSize, initialGBMFontSize, fadeTime));
        StartCoroutine(TweenText(newTMP, initialGBMFontSize, initialGBMFontSize * 1.5f, fadeTime));
        currentGBMIndex = index;

        var nextIndex = currentGIPMIndex == 0 ? 1 : 0;

        gameInfoPanelManagers[currentGIPMIndex].OnPathEditDoneButtonPressed();
        gameInfoPanelManagers[nextIndex].OnPathEditDoneButtonPressed();

        gameInfoPanelManagers[currentGIPMIndex].canvasGroup.interactable = false;
        gameInfoPanelManagers[currentGIPMIndex].canvasGroup.blocksRaycasts = false;
        gameInfoPanelManagers[nextIndex].canvasGroup.interactable = true;
        gameInfoPanelManagers[nextIndex].canvasGroup.blocksRaycasts = true;
        gameInfoPanelManagers[nextIndex].SetGameInfo(gameInfo, startCount, playTime);
        gameInfoPanelManagers[currentGIPMIndex].FadeOut(fadeTime);
        gameInfoPanelManagers[nextIndex].FadeIn(fadeTime);
        currentGIPMIndex = nextIndex;
    }

    public IEnumerator RunGame(GameInfoScriptableObject gameInfo)
    {
        var startCount = PlayerPrefs.GetInt(gameInfo.gameName + "_Count", 0);
        var playTime = PlayerPrefs.GetFloat(gameInfo.gameName + "_Time", 0f);

        noteTMP.text = string.Format(noteText, gameInfo.gameName);

        menu.interactable = false;
        process = new Process();
        process.StartInfo.FileName = gameInfo.path;

        yield return FadeCanvas(menu, 1, 0, fadeTime);
        yield return FadeCanvas(note, 0, 1, fadeTime);

        yield return new WaitForSeconds(2f);

        var successStart = true;
        var beginTime = Time.realtimeSinceStartup;

        try
        {
            process.Start();
        }
        catch
        {
            successStart = false;
        }
        if (successStart)
        {
            startCount++;
            process.WaitForExit();
        }

        PlayerPrefs.SetInt(gameInfo.gameName + "_Count", startCount);
        PlayerPrefs.SetFloat(gameInfo.gameName + "_Time", playTime + Time.realtimeSinceStartup - beginTime);
        PlayerPrefs.Save();

        yield return new WaitForSeconds(1f);

        yield return FadeCanvas(menu, 0, 1, fadeTime);
        yield return FadeCanvas(note, 1, 0, fadeTime);

        menu.interactable = true;
    }

    public IEnumerator TweenText(TextMeshProUGUI tmp, float from, float to, float time)
    {
        var timer = 0f;
        yield return new WaitWhile(() =>
        {
            var t = fontTweenCurve.Evaluate(timer / time);
            var value = Mathf.Lerp(from, to, t);
            tmp.fontSize = value;
            var rect = tmp.transform as RectTransform;
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, value);
            if (timer > time)
            {
                return false;
            }
            else
            {
                timer += Time.deltaTime;
                return true;
            }
        });
    }

    public IEnumerator FadeCanvas(CanvasGroup canvasGroup, float from, float to, float time)
    {
        var timer = 0f;
        yield return new WaitWhile(() =>
        {
            canvasGroup.alpha = Mathf.Lerp(from, to, timer / time);
            if (timer > time)
            {
                return false;
            }
            else
            {
                timer += Time.deltaTime;
                return true;
            }
        });
    }

    private void Initialize()
    {
        gameInfoPanelManagers = new GameInfoPanelManager[2];

        for (int i = 0; i < 2; i++)
        {
            var go = Instantiate(gameInfoPanelManagerPrefab, menu.transform);
            gameInfoPanelManagers[i] = go.GetComponent<GameInfoPanelManager>();
        }

        currentGIPMIndex = 0;
        gameInfoPanelManagers[0].canvasGroup.alpha = 1;
        gameInfoPanelManagers[0].canvasGroup.interactable = true;
        gameInfoPanelManagers[0].canvasGroup.blocksRaycasts = true;
        gameInfoPanelManagers[1].canvasGroup.alpha = 0;
        gameInfoPanelManagers[1].canvasGroup.interactable = false;
        gameInfoPanelManagers[1].canvasGroup.blocksRaycasts = false;

        var gameInfoLength = gameInfos.Length;
        gameButtonManagers = new GameButtonManager[gameInfoLength];

        for (var i = 0; i < gameInfoLength; i++)
        {
            var go = Instantiate(gameNamePrafab, gameSelectLayout.transform);
            gameButtonManagers[i] = go.GetComponent<GameButtonManager>();
            gameButtonManagers[i].gameInfo = gameInfos[i];
            gameButtonManagers[i].textMesh.text = gameInfos[i].gameName;
            gameButtonManagers[i].index = i;
            go.SetActive(false);
            go.SetActive(true);
        }

        initialGBMFontSize = gameButtonManagers[0].textMesh.fontSize;

        CanvasGroupManager.current.ChangeGameInfoPanel(gameInfos[0], 0);
    }

    private void Awake()
    {
        if (current == null)
        {
            current = this;
        }
        else
        {
            Destroy(this);
        }

        Initialize();
    }
}
