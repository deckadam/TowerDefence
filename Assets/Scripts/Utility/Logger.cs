using System.Collections.Generic;
using UnityEngine;

public class Logger : MonoSingleton<Logger>
{
    public bool enabledByHotkey;
    public KeyCode enablingKey;

    private Dictionary<string, string> debugTexts = new Dictionary<string, string>();

    private int width;
    private int height;
    private int fontSize;
    public int fontSizeMultiplier = 3;
    public Color textColor = Color.black;

    public void SetDebugText(string key, object value, bool showKey = true)
    {
        var displayText = showKey ? (key + ": " + value) : value.ToString();
        debugTexts[key] = displayText;
    }

    private bool visibilitySwap;

    private void Update()
    {
        if (Input.GetKeyDown(enablingKey))
        {
            visibilitySwap = !visibilitySwap;
        }
    }


    private void Start()
    {
        width = Screen.width;
        height = Screen.height;
        fontSize = height * fontSizeMultiplier / 100;
    }

    private void OnGUI()
    {
        if (enabledByHotkey && !visibilitySwap) return;
        var counter = 0;
        foreach (var debug in debugTexts.Keys)
        {
            var style = new GUIStyle();
            var rect = new Rect(0, fontSize * counter, width, fontSize * counter++);
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = fontSize;
            style.normal.textColor = textColor;
            GUI.Box(rect, debugTexts[debug], style);
            //GUI.Label(rect, debugTexts[debug], style);
        }
    }
}