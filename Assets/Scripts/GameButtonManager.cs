using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameButtonManager : MonoBehaviour
{
    public GameInfoScriptableObject gameInfo;

    public Button button;
    public TextMeshProUGUI textMesh;
    public int index;

    public void OnPressed()
    {
        CanvasGroupManager.current.ChangeGameInfoPanel(gameInfo, index);
    }
}
