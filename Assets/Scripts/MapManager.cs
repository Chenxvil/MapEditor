using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MapManager : MonoBehaviour
{
    //单实例类
    public static MapManager Instance { get; private set; }

    //UI控件的引用
    public Dropdown m_dropdownSize;
    public Dropdown m_dropdownTexture;

    //地图类数据
    public Map m_map { get; private set; } = null;

    //地图块的预设体
    public GameObject m_prefabMapCube;
    //地图块的父物体
    public GameObject m_parentMapCube;
    //地图块类数据列表
    public List<GameObject> m_objCubes = new List<GameObject>();

    //地图网格的父物体
    public GameObject m_parentMapMesh;
    //网格数据列表
    public List<GameObject> m_objLines = new List<GameObject>();

    //当前选中的地图块
    private GameObject m_currMapCube;
    
    //纹理资源的根目录
    private readonly string m_textureDirPath = "MapTexture/"; 

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            bool isExist = Physics.Raycast(ray, out RaycastHit raycastHit, 1000, LayerMask.GetMask("MapCube"));
            if (isExist)
            {
                m_currMapCube = raycastHit.collider.gameObject;
                if (m_currMapCube.GetComponent<MapCube>().m_isEmpty)
                {
                    //if(EntityManager.Instance.m_isBuild)
                    if (EditorManager.CurrES == EditState.ENTITYEDIT)
                    {
                        m_currMapCube.GetComponent<MapCube>().m_isEmpty = false;
                        MapCube mapCube = m_currMapCube.GetComponent<MapCube>();
                        EntityManager.Instance.CreateEntity(mapCube.m_position, mapCube.m_index);
                    }
                    //else if(PathManager.Instance.m_isEdit)
                    else if (EditorManager.CurrES == EditState.PATHEDIT)
                    {
                        PathManager.Instance.CreateKeyPoint(m_currMapCube.GetComponent<MapCube>().m_position);
                    }
                }
            }
        }
    }

    public void CreateMap()
    {
        Clear();
        EntityManager.Instance.Clear();
        PathManager.Instance.Clear();
        MusicManager.Instance.Clear(); 

        m_map = new Map(); 
        string text = m_dropdownSize.captionText.text;
        string[] data = text.Split();
        int len = int.Parse(data[0]);
        int wid = int.Parse(data[2]);
        m_map.m_length = len;
        m_map.m_width = wid;

        text = m_dropdownTexture.captionText.text;
        string texturePath = m_textureDirPath + text;
        m_map.m_textureName = text;
         
        Texture texture = Resources.Load<Texture>(texturePath);
        m_prefabMapCube.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = texture;

        GenerateMapCube(len, wid);
        GenerateMapMesh(len, wid);
        LogManager.Instance.Log("创建地图成功!");
    }

    public void CreateMap(int len, int wid, string textureName)
    {
        Clear();

        m_map = new Map();
        m_map.m_length = len;
        m_map.m_width = wid;

        string texturePath = m_textureDirPath + textureName;
        m_map.m_textureName = textureName;

        Texture texture = Resources.Load<Texture>(texturePath);
        m_prefabMapCube.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = texture;

        GenerateMapCube(len, wid);
        GenerateMapMesh(len, wid);
    }

    public void GenerateMapCube(int len, int wid)
    {
        int midLen = len / 2;
        int midWid = wid / 2;
        int count = 0;
        for(int i = 0; i< wid;i++)
        {
            for(int j = 0;j< len;j++)
            {
                float xValue = (j - midLen) + 0.5f;
                float zValue = (i - midWid) + 0.5f; 
                Vector3 pos = new Vector3(xValue, 0, zValue);
                GameObject gameObject = GameObject.Instantiate(m_prefabMapCube, pos, Quaternion.identity, m_parentMapCube.transform);
                gameObject.GetComponent<MapCube>().m_index = count;
                gameObject.GetComponent<MapCube>().m_position = pos;
                m_objCubes.Add(gameObject);
                count++;
            }
        }
    }

    public void GenerateMapMesh(int len, int wid)
    {
        int midLen = len / 2;
        int midWid = wid / 2;
        float yOffset = 0.0001f;
        int count = 0;
        for (int i = 0; i <= wid; i++)
        {
            int xValue = midLen;
            int zValue = (i - midWid);
            m_objLines.Add(new GameObject("LineRow" + i));
            m_objLines[count].transform.parent = m_parentMapMesh.transform;
            LineRenderer lineRenderer = m_objLines[count].AddComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Particles/Standard Unlit"));
            lineRenderer.useWorldSpace = true;
            lineRenderer.startColor = Color.white;
            lineRenderer.endColor = Color.white;
            lineRenderer.startWidth = 0.05f;
            lineRenderer.endWidth = 0.05f;
            lineRenderer.SetPosition(0, new Vector3(xValue, yOffset, zValue));
            lineRenderer.SetPosition(1, new Vector3(-xValue, yOffset, zValue));
            count++;
        }
        for (int i = 0; i <= len; i++)
        {
            int xValue = (i - midLen);
            int zValue = midWid;
            m_objLines.Add(new GameObject("LineColum" + i));
            m_objLines[count].transform.parent = m_parentMapMesh.transform;
            LineRenderer lineRenderer = m_objLines[count].AddComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Particles/Standard Unlit"));
            lineRenderer.useWorldSpace = true;
            lineRenderer.startColor = Color.white;
            lineRenderer.endColor = Color.white;
            lineRenderer.startWidth = 0.05f;
            lineRenderer.endWidth = 0.05f;
            lineRenderer.SetPosition(0, new Vector3(xValue, yOffset, zValue));
            lineRenderer.SetPosition(1, new Vector3(xValue, yOffset, -zValue));
            count++;
        }
    }

    public GameObject GetCurrMapCube()
    { 
        return m_currMapCube;
    }

    public GameObject GetObjMapCube(int index)
    {
        return m_objCubes[index];
    }

    public MapCube GetMapCube(int index)
    {
        return m_objCubes[index].GetComponent<MapCube>();
    }

    public void Clear()
    {
        m_map = null;
        m_currMapCube = null;

        foreach(GameObject objCube in m_objCubes)
        {
            Destroy(objCube);
        }
        m_objCubes.Clear();

        foreach(GameObject objLine in m_objLines)
        {
            Destroy(objLine);
        }
        m_objLines.Clear();
        LogManager.Instance.Log("清除所有地图信息成功!");
    }
}
