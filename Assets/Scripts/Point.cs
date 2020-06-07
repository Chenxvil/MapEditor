using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point
{
    //地图块位置信息
    public int m_x { get; set; }
    public int m_y { get; set; }
    //标志位
    public bool m_isBarrier { get; set; }
    //指向父节点
    public Point m_parent { get; set; }
    //G值
    public int m_valueGiven { get; set; }
    //H值
    public int m_valueHeur { get; set; }
    //F值，代价因子
    public int m_costFactor { get; set; }

    public Point(int x, int y, bool isBarrier)
    {
        this.m_x = x;
        this.m_y = y;
        this.m_isBarrier = isBarrier;
        this.m_valueGiven = 0;
        this.m_valueHeur = 0;
    }

    public Point(int x, int y, bool isBarrier, int index)
    {
        this.m_x = x;
        this.m_y = y;
        this.m_isBarrier = isBarrier;
        this.m_valueGiven = 0;
        this.m_valueHeur = 0;
    }

    public Point(Point p)
    {
        this.m_x = p.m_x;
        this.m_y = p.m_y;
        this.m_isBarrier = p.m_isBarrier;
        this.m_parent = p.m_parent;
        this.m_valueGiven = p.m_valueGiven;
        this.m_valueHeur = p.m_valueHeur;
        this.m_costFactor = p.m_costFactor;
    }

    public void CalcCostFactor()
    {
        this.m_costFactor = m_valueGiven + m_valueHeur;
    }

    public void Clear()
    {
        this.m_parent = null;
        this.m_valueGiven = 0;
        this.m_valueHeur = 0;
        this.m_costFactor = 0;
    }
} 
