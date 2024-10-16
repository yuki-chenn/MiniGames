using UnityEngine;
using System.Collections.Generic;
using System;

public class DebugDisplay : MonoBehaviour
{
    /// <summary>一页展示多少行</summary>
    private const int maxLines = 50;
    /// <summary>一行有多少个字</summary>
    private const int maxLineLength = 80;
    /// <summary>输出的字体大小</summary>
    private const int fontSize = 25;
    /// <summary>字体颜色</summary>
    private readonly Color fontColor = Color.red;

    private string _logStr = "";
    private readonly List<string> _lines = new List<string>();

    void OnEnable() { Application.logMessageReceived += Log; }
    void OnDisable() { Application.logMessageReceived -= Log; }

    public void Log(string logString, string stackTrace, LogType type)
    {
        foreach (var line in logString.Split('\n'))
        {
            if (line.Length <= maxLineLength)
            {
                _lines.Add(line);
                continue;
            }
            var lineCount = line.Length / maxLineLength + 1;
            for (int i = 0; i < lineCount; i++)
            {
                if ((i + 1) * maxLineLength <= line.Length)
                {
                    _lines.Add(line.Substring(i * maxLineLength, maxLineLength));
                }
                else
                {
                    _lines.Add(line.Substring(i * maxLineLength, line.Length - i * maxLineLength));
                }
            }
        }
        if (_lines.Count > maxLines)
        {
            _lines.RemoveRange(0, _lines.Count - maxLines);
        }
        _logStr = string.Join("\n", _lines);
    }

    private void OnGUI()
    {
        GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity,
           new Vector3(1f, 1f, 1.0f));
        GUI.Label(new Rect(100, 100, 600, 370), _logStr, new GUIStyle() { fontSize = Math.Max(10, fontSize), normal = new GUIStyleState() { textColor = fontColor } });

#if UNITY_EDITOR
        // 添加清除按钮
        if (GUI.Button(new Rect(400, 400, 200, 60), "Clear"))
        {
            _lines.Clear();
            _logStr = "";
        }
#endif
    }
}
