using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Game Info", menuName = "Game Info")]
public class GameInfoScriptableObject : ScriptableObject
{
    public string path;
    public string gameName;
    public string creatorName;
    [TextArea(1, 64)]
    public string explanation;
    public Sprite backgroundTexture;
}
