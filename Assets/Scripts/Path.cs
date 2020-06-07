using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{
    //路径点列表
    public List<Point> m_points = new List<Point>();
    //关键路径点列表
    public List<Point> m_keyPoints = new List<Point>();
    //路径点列表是否为空
    public bool m_isEmpty { get; set; } = true;
    //路径编号
    public int m_index;
}
