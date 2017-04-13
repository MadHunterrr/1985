using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DamageableParts))]
public class DamageablePartsEditor : Editor {

    public override void OnInspectorGUI()
    {
        DamageableParts src = (DamageableParts)target;

        src.BodyPart = (DamageableParts.BodyType)EditorGUILayout.EnumPopup(new GUIContent("Тип конечнсти: ", "Указывает на тип конечности персонажа"), src.BodyPart);
        switch(src.BodyPart)
        {
            case DamageableParts.BodyType.LeftHand:
            case DamageableParts.BodyType.RightHand:
                src.HandMultiply = EditorGUILayout.Slider(new GUIContent("Множитель урона: ", "Множитель урона при попадаии в руку"), src.HandMultiply, 0.5f, 3f);
                break;
            case DamageableParts.BodyType.LeftLeg:
            case DamageableParts.BodyType.RightLeg:
                src.LegsMultiply = EditorGUILayout.Slider(new GUIContent("Множитель урона: ", "Множитель урона при попадаии в ногу"), src.LegsMultiply, 0.5f, 3f);
                break;
            case DamageableParts.BodyType.Head:
                src.HeadMultiply = EditorGUILayout.Slider(new GUIContent("Множитель урона: ", "Множитель урона при попадаии в голову"), src.HeadMultiply, 0.5f, 3f);
                break;
            case DamageableParts.BodyType.Torso:
                src.TorsoMultiply = EditorGUILayout.Slider(new GUIContent("Множитель урона: ", "Множитель урона при попадаии в туловище"), src.TorsoMultiply, 0.5f, 3f);
                break;
        }

    }

}
