using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum EntityType
{
    Zero,
    Building,
    Monster,
    NPC,
    
}

public class Entity : MonoBehaviour
{
    //游戏属性
    public float HP { get; set; } = 100;
    public float AtkValue { get; set; } = 10;
    public float DefValue { get; set; } = 5;
    public float AtkSpeed { get; set; } = 1;
    public float AtkRange { get; set; } = 1;
    public float MoveSpeed { get; set; } = 1;
    public bool IsMove { get; set; } = false;
    public bool IsAtk { get; set; } = false; 

    //实体属性
    public Vector3 m_position { get; set; }
    public Quaternion m_quaternion { get; set; }
    public EntityType m_type { get; set; }
    public string m_modelName { get; set; }
    public int m_mapCubeIndex { get; set; }
    public int m_pathIndex { get; set; }
    public int m_eventIndex { get; set; }
}
