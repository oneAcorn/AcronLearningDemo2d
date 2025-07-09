using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 快捷调试timeScale,其原本位置在Edit > Project Settings > Time中设置,比较麻烦.
/// </summary>
public class TimeScaleTool : EditorWindow
{
    private float timeScale;

    [MenuItem("Tools/Time Scale Controller")]
    public static void ShowWindow()
    {
        GetWindow<TimeScaleTool>("Time Scale");
    }

    private void OnEnable()
    {
        // 在窗口启用时加载之前保存的时间缩放比例
        timeScale = EditorPrefs.GetFloat("TimeScale", 1.0f);
    }

    private void OnDisable()
    {
        // 当窗口关闭时保存当前的时间缩放比例
        EditorPrefs.SetFloat("TimeScale", timeScale);
    }

    void OnGUI()
    {
        GUILayout.Label("调整游戏速度", EditorStyles.boldLabel);
        timeScale = EditorGUILayout.Slider("Time Scale", timeScale, 0f, 5f);

        if (GUILayout.Button("应用"))
        {
            Time.timeScale = timeScale;
        }

        if (GUILayout.Button("恢复 1x"))
        {
            Time.timeScale = 1f;
            timeScale = 1f;
        }
    }
}
