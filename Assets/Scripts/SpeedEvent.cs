using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedEvent : Event
{
    public SpeedEvent()
    {
        this.m_speed = 0;
        this.m_time = 0;
        this.m_isInfinite = false;
    }
    public SpeedEvent(float speed, float time, bool isInfinite)
    {
        this.m_speed = speed;
        this.m_time = time;
        this.m_isInfinite = isInfinite;
    }
    public float m_speed;
    public float m_time;
    public bool m_isInfinite;
    public override string Type
    {
        get
        {
            return "Speed";
        }
    }
} 