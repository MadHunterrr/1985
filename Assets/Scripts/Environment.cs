using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Environment : MonoBehaviour
{
    [Range(0, 23)]
    public byte timeHour;
    [Range(0, 23)]
    public byte timeMinute;
    public const float AnglePerMinute = 0.25f;
    public const float AnglePerHour = 15f;
    [Range(1, 60)]
    public float TimeMultiple = 1; //(60-realtime)(1 game minyte = 1 real secounds
    public int DayCount = 0;



    public Light Sun
    {
        get
        {
            return RenderSettings.sun;
        }
    }


    public void SunAngle()
    {

        float angle = (timeHour * AnglePerHour) + (timeMinute * AnglePerMinute);
        Sun.transform.rotation = Quaternion.Euler(angle - 90, -90, 30);
    }


    public float lastCheckTime = 0;
    void ChangeTime()
    {
        if (timeMinute < 59)
            timeMinute++;
        else
        {
            if (timeHour < 23)
                timeHour++;
            else
                timeHour = 0;
            timeMinute = 0;
        }
        SunAngle();
    }
    // Update is called once per frame
    void Update()
    {
        if (Time.time >= lastCheckTime + TimeMultiple)
        {
            ChangeTime();
            lastCheckTime = Time.time;
        }
    }


}
