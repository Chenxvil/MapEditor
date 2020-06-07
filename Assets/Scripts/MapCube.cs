using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapCube : MonoBehaviour
{
    public Vector3 m_position { get; set; }   
    public int m_index { get; set; } = -1;
    public bool m_isEmpty { get; set; } = true;
    public bool m_isBarrier { get; set; } = false;

    private Renderer m_renderer;

    private void Start()
    {
        m_renderer = GetComponent<Renderer>();
    }

    private void OnMouseEnter()
    {
        if(!EventSystem.current.IsPointerOverGameObject() && m_isEmpty)
        {
            if (EntityManager.Instance.m_isBuild || EditorManager.CurrES == EditState.ENTITYEDIT)
                ChangeColor(Color.green);
            else if (PathManager.Instance.m_isEdit || EditorManager.CurrES == EditState.ENTITYEDIT)
                ChangeColor(Color.blue);
            else
                ChangeColor(Color.red);
        }
    }

    private void OnMouseExit()
    {
        ChangeColor(Color.white);
    }

    public void ChangeColor(Color color)
    {
        m_renderer.material.color = color;
    }

}
