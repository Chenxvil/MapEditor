using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirthEvent : Event
{
    public BirthEvent()
    {
        this.m_cd = 0;
        this.m_time = 0;
        this.m_isInfinite = false;
    }

    public BirthEvent(float cd, float time, bool isInfinite)
    {
        this.m_cd = cd;
        this.m_time = time;
        this.m_isInfinite = isInfinite;
    }
    public float m_cd;
    public float m_time;
    public bool m_isInfinite;

    public override string Type
    {
        get
        {
            return "Birth";
        }
    }
}
