using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [HideInInspector]
    public Inventory Invent;
    public enum Movement { Run, Walk, Crouch, Sprint}
    [Header("Передвижение")]
    [Tooltip("Walk = ходьба, Crouch = скрытность, Run = бег(базовый), Sprint = спринт")]
    public Movement CurrentMovement;
    [Tooltip("Базовая скорость перемещения. Используеться при режиме CurrentMovement = Run")]
    public float Speed = 5;
    [Range(0.2f,0.4f)]
    [Tooltip("Множитель скорости при скрытом перемещении. Скорость при скрытом перемещении равняется базовой скорости(Speed) * [этот множитель]")]
    public float CrouchMultiple = 0.3f;
    [Range(0.4f, 0.7f)]
    [Tooltip("Множитель скорости при ходьбе. Скорость при ходьбе равняется базовой скорости(Speed) * [этот множитель]")]
    public float WalkMultiple = 0.45f;
    [Range(1.2f, 2f)]
    [Tooltip("Множитель скорости при спринте. Скорость при спринте равняется базовой скорости(Speed) * [этот множитель]")]
    public float SprintMultiple = 1.3f;

    [Header("Общее здоровье")]
    public float Health = 100;
    public float CurHealth = 100;

    //----Здоровье частей тела---- от 0 до 100-----//
    [Header("Здоровья разных частей тела")]
    public float HeadHealth = 100;
    public float TorsoHealth = 100;
    public float RightHandHealth = 100;
    public float LeftHeandHealth = 100;
    public float RightLegHealth = 100;
    public float LeftLegHealth = 100;

    //----Защита частей тела---- от 0 до 85-----//
    [Header("Броня для разных частей тела")]
    [Range(0, 85)]
    public int HeadArmor = 0;
    [Range(0, 85)]
    public int TorsoArmor = 0;
    [Range(0, 85)]
    public int RightHandArmor = 0;
    [Range(0, 85)]
    public int LeftHandArmor = 0;
    [Range(0, 85)]
    public int RightLegArmor = 0;
    [Range(0, 85)]
    public int LeftLegArmor = 0;

    [Header("Параметры анимации")]
    public bool isAIM = false;

    //Static function
    public static Vector3 Direction(Vector3 target, Vector3 hero)
    {
        Vector3 temp = target - hero;
        float ftemp = temp.magnitude;
        return temp / ftemp;
    }

}
