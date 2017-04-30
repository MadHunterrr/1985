using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class EnvironmentEditorScript : EditorWindow
{

    [MenuItem("Window/Environment")]

    public static void ShowWindow()
    {
        GetWindow(typeof(EnvironmentEditorScript));
    }

    void OnGUI()
    {
        try
        {
            Environment en = FindObjectOfType<Environment>();
            EditorGUILayout.LabelField("Время");
            en.timeHour = (byte)EditorGUILayout.IntSlider(new GUIContent("Часов: ", "Время игры в игровых часах"), en.timeHour, 0, 23);
            en.timeMinute = (byte)EditorGUILayout.IntSlider(new GUIContent("Минут: ", "Время игры в игровых часах(минутах)"), en.timeMinute, 0, 59);
            en.TimeMultiple = EditorGUILayout.Slider(new GUIContent("Множитель времени: ", "Если установить значение в 1 - то 1 игровая минута будет равняться 1 секунде реального времени"), en.TimeMultiple, 1, 60);
            EditorGUILayout.EnumPopup(new GUIContent("Текущее время дня: ", "Текущее время дня(только для чтения)"),en.TimeOfDay);
            en.SunAngle();
        }
        catch
        {
            Debug.Log("Нет менеджера окружения");
        }
    }

}
