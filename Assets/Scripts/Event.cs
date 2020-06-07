  using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Event
{
    public Event()
    {
        m_index = Index;
         
        Index++;
    }
    private static int Index = 1;
    public int m_index { get; private set; }

    public abstract string Type { get; }
}
