using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombEvent : Event
{
    public BombEvent()
    {
        this.m_delay = 0;
        this.m_hurt = 0;
        this.m_isDestory = false;
    }
    public BombEvent(float delay, float hurt, bool isDestroy)
    {
        this.m_delay = delay;

        this.m_hurt = hurt;
        this.m_isDestory = isDestroy; 
    }

    public float m_delay;
    public float m_hurt;
    public float m_range;
    public bool m_isDestory;
    public override string Type
    {
        get
        {
            return "Bomb";
        }
    }
}
