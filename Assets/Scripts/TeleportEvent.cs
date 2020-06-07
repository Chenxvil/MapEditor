using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportEvent :Event
{
    public TeleportEvent()
    {
        this.m_teleportID = 0;
        this.m_cd = 0;
        this.m_time = 0;
        this.m_isInfinite = false;
    }

    public TeleportEvent(int teleportID, float cd, float time, bool isInfinite)
    {
        this.m_teleportID = teleportID;
        this.m_cd = cd;
        this.m_time = time;
        this.m_isInfinite = isInfinite;
    }
    public int m_teleportID;
    public float m_cd;
    public float m_time;
    public bool m_isInfinite;
    public override string Type
    {
        get
        {
            return "Teleport";
        }
    }
}