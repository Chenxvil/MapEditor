using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PathManager : MonoBehaviour
{
    public static PathManager Instance { get; private set; }

    //关键路径点预设体
    public GameObject m_keyPointArrowPrefab;
    //路径点预设体
    public GameObject m_pathArrowPrefab;
    //路径预设体
    public GameObject m_pathPrefeb;
    //路径的父物体
    public GameObject m_parentPath;

    //UI控件的引用
    public GameObject m_objPathAttribute;
    private Text[] m_textAttribute = new Text[3];

    //所有路径数据的列表
    public List<GameObject> m_objPaths { get; set; } = new List<GameObject>();
    //当前选中的路径
    private GameObject m_objCurrPath;

    //路径编辑状态
    public bool m_isEdit { get; set; } = false;
    //关键路径设置状态
    public bool m_isBuild { get; set; } = false;

    //寻路算法相关的数据
    //地图长度
    private int m_len;
    //地图宽度
    private int m_wid;
    //地图所有点的数据
    private Point[,] m_points;
    //寻路算法中的open list
    private List<Point> m_closeList = new List<Point>();
    //寻路算法中close list
    private List<Point> m_openList = new List<Point>();

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

    private void Start()
    {
        m_textAttribute[0] = m_objPathAttribute.transform.Find("Number").GetComponent<Text>();
        m_textAttribute[1] = m_objPathAttribute.transform.Find("KPCount").GetComponent<Text>();
        m_textAttribute[2] = m_objPathAttribute.transform.Find("PointCount").GetComponent<Text>();

        Button button = m_objPathAttribute.transform.Find("Btn_Edit").GetComponent<Button>();
        button.onClick.AddListener(delegate { EditPath(); });

        button = m_objPathAttribute.transform.Find("Btn_Refresh").GetComponent<Button>();
        button.onClick.AddListener(delegate { RefreshPath(); });

        button = m_objPathAttribute.transform.Find("Btn_Delete").GetComponent<Button>();
        button.onClick.AddListener(delegate { DeletePath(); });
    }

    void Update()
    {
        if(m_isEdit)
        {
            if(Input.GetMouseButtonDown(1) && !EventSystem.current.IsPointerOverGameObject())
            {
                m_isEdit = false;
                if(!m_objCurrPath.GetComponent<Path>().m_isEmpty)
                    m_objPaths.Add(m_objCurrPath);
                m_objCurrPath = null;
                EditorManager.CurrES = EditState.MAPEDIT;
            }
        }
        else
        {
            if(Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject() && m_isBuild)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                bool isExist = Physics.Raycast(ray, out RaycastHit raycastHit, 1000, LayerMask.GetMask("Path"));
                if (isExist)
                {
                    Transform kpTran = raycastHit.collider.transform;
                    m_objCurrPath = kpTran.parent.gameObject;
                    SetAttributeArea();
                }
            }
        }  
    }

    public void SetAttributeArea()
    {
        if(m_objCurrPath != null)
        {
            Path path = m_objCurrPath.GetComponent<Path>();
            m_textAttribute[0].text = path.m_index.ToString();
            m_textAttribute[1].text = path.m_keyPoints.Count.ToString();
            m_textAttribute[2].text = path.m_points.Count.ToString();
        }
        else
        {
            m_textAttribute[0].text = "";
            m_textAttribute[1].text = "";
            m_textAttribute[2].text = "";
        }
        
    }

    public List<Path> GetPaths()
    {
        List<Path> paths = new List<Path>();
        foreach(GameObject go in m_objPaths)
        {
            paths.Add(go.GetComponent<Path>());
        }
        return paths;
    }

    public void SetPaths(List<Path> paths)
    {
        foreach(Path path in paths)
        {
            //设置路径数据
            GameObject objPath = GameObject.Instantiate<GameObject>(m_pathPrefeb, m_parentPath.transform);
            Path tempPath = objPath.GetComponent<Path>();
            tempPath.m_isEmpty = path.m_isEmpty;
            tempPath.m_index = path.m_index;
            tempPath.m_keyPoints = path.m_keyPoints;
            tempPath.m_points = path.m_points;

            //设置路径物体
            m_objCurrPath = objPath;
            DrawKeyPoints(path);
            DrawPoints(path, 0);

            m_objPaths.Add(objPath);
        }
        m_objCurrPath = null;
    }

    public void AddPath(int index, bool isE, List<Vector2> tempKeyPoints, List<Vector2> tempPoints)
    {
        GameObject objPath = GameObject.Instantiate<GameObject>(m_pathPrefeb, m_parentPath.transform);
        Path tempPath = objPath.GetComponent<Path>();
        tempPath.m_index = index;
        tempPath.m_isEmpty = isE;
        foreach(Vector2 tempKP in tempKeyPoints)
        {
            tempPath.m_keyPoints.Add(m_points[(int)tempKP.x, (int)tempKP.y]);
        }
        foreach(Vector2 tempP in tempPoints)
        {
            tempPath.m_points.Add(m_points[(int)tempP.x, (int)tempP.y]);
        }

        //设置路径物体
        m_objCurrPath = objPath;
        DrawKeyPoints(tempPath);
        DrawPoints(tempPath, 0);

        m_objPaths.Add(objPath);
    }

    public void StartEdit()
    {
        if(MapManager.Instance.m_map == null)
        {
            LogManager.Instance.Log("无法编辑路径，请先创建地图！");
            return;
        }
        if (m_isBuild)
        {
            LogManager.Instance.Log("退出路径编辑!");
            m_isBuild = false;
            m_isEdit = false;
            m_objCurrPath = null;
            if(m_parentPath.activeSelf == true)
            {
                m_parentPath.SetActive(false);
            }
           if(m_objPathAttribute.activeSelf == true)
            {
                m_objPathAttribute.SetActive(false);
            }
        }
        else
        {
            LogManager.Instance.Log("进入路径编辑!");
            GeneratePointMsg();
            m_isBuild = true;
            m_parentPath.SetActive(true);
            m_objPathAttribute.SetActive(true);
            //EditorManager.CurrES = EditState.PATHEDIT;
        }
    }

    public void EditPath()
    {
        if (m_isEdit)
            return;
        m_isEdit = true;
        EditorManager.CurrES = EditState.PATHEDIT;
        if (m_objCurrPath==null)
        {
            m_objCurrPath = GameObject.Instantiate<GameObject>(m_pathPrefeb, m_parentPath.transform);
            m_objCurrPath.GetComponent<Path>().m_index = m_objPaths.Count + 1;
            LogManager.Instance.Log("创建路径成功!");
        }

    }

    public void RefreshPath()
    {
        //TODO
        //根据当前场景重新计算路径
    }

    public void DeletePath()
    {
        //当前路径的编号
        int index = m_objCurrPath.GetComponent<Path>().m_index;
        //删除当前路径
        m_objPaths.Remove(m_objCurrPath);
        Destroy(m_objCurrPath);
        m_objCurrPath = null;

        //对剩下的路径进行重新编号
        for (int i = index - 1; i < m_objPaths.Count; i++)
        {
            Path path = m_objPaths[i].GetComponent<Path>();
            path.m_index = i + 1;
            foreach (Transform transform in path.transform)
            {
                if (transform.tag == "KeyPoint")
                    transform.GetComponentInChildren<TextMesh>().text = (i + 1).ToString();
            }
        }
        SetAttributeArea();
        LogManager.Instance.Log("删除路径成功!");
    }

    public void CreateKeyPoint(Vector3 pos)
    {
        //创建关键点物体
        GameObject keyPoint = GameObject.Instantiate<GameObject>(m_keyPointArrowPrefab, pos, Quaternion.identity, m_objCurrPath.transform);
        keyPoint.GetComponentInChildren<TextMesh>().text = m_objCurrPath.GetComponent<Path>().m_index.ToString();

        //添加关键点到当前路径中
        Path path = m_objCurrPath.GetComponent<Path>();
        Point point = GetPointByPos(pos);
        path.m_keyPoints.Add(point);
                
        //根据关键点构建路径
        int index = path.m_points.Count - 1;
        if (path.m_isEmpty)
        {
            path.m_points.Add(point);
            path.m_isEmpty = false;
        }
        else
        {
            //如果路径不为空，则起始点设为当前路径的最后一个关键点，终点为当前关键点
            Point startPoint = path.m_points.Last();
            Point endPoint = point;

            //构建路径
            List<Point> pathPoints = GeneratePath(startPoint, endPoint);
            foreach (Point p in pathPoints)
            {
                path.m_points.Add(p);
            }
            //创建路径物体
            DrawPoints(path, index);
        }
        SetAttributeArea();
    }
    
    public void DrawKeyPoints(Path path)
    {
        List<Point> keyPoints = path.m_keyPoints;
        foreach(Point keyPoint in keyPoints)
        {
            Vector3 pos = GetPosByPoint(keyPoint.m_x, keyPoint.m_y);
            GameObject objKeyPoint = GameObject.Instantiate<GameObject>(m_keyPointArrowPrefab, pos, Quaternion.identity, m_objCurrPath.transform);
            objKeyPoint.GetComponentInChildren<TextMesh>().text = path.m_index.ToString();
        }
    }

    public void DrawPoints(Path path, int index)
    {
        List<Point> points = path.m_points;
        for (int i = index; i < points.Count - 1; i++)
        {
            Vector3 currPos = GetPosByPoint(points[i].m_x, points[i].m_y);
            Vector3 postPos = GetPosByPoint(points[i+1].m_x, points[i+1].m_y);


            Vector3 direction = postPos - currPos;
            Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, direction);
            float x = (currPos.x + postPos.x) / 2;
            float z = (currPos.z + postPos.z) / 2;
            Vector3 position = new Vector3(x, 0.2f, z);

            GameObject.Instantiate<GameObject>(m_pathArrowPrefab, position, rotation, m_objCurrPath.transform);
        }
    }

    public void GeneratePointMsg()
    {
        m_len = MapManager.Instance.m_map.m_length;
        m_wid = MapManager.Instance.m_map.m_width;

        m_points = new Point[m_wid, m_wid];

        //写入信息
        for (int x = 0; x < m_wid; x++)
        {
            for (int y = 0; y < m_len; y++)
            {
                MapCube tempMC = MapManager.Instance.m_objCubes[x * m_len + y].GetComponent<MapCube>();
                m_points[x, y] = new Point(x, y, !tempMC.m_isEmpty);
            }
        }
    }

    public void Clear()
    {
        //清除所有路径信息
        foreach (GameObject path in m_objPaths)
        {
            Destroy(path);
        }
        m_objPaths.Clear();

        //重置状态
        m_objCurrPath = null;
        m_isEdit = false;
        m_isBuild = false;
        m_len = 0;
        m_wid = 0;

        //清除用于寻路的数据
        m_points = null;
        m_closeList.Clear();
        m_openList.Clear();
        LogManager.Instance.Log("清除所有路径信息成功!");
    }
  
    public Point GetPointByPos(Vector3 pos)
    {
        float x = pos.z + m_wid / 2 - 0.5f;
        float y = pos.x + m_len / 2 - 0.5f;
        return m_points[(int)x, (int)y];
    }

    public Vector3 GetPosByPoint(int x, int y)
    {
        float posZ = (x + 0.5f) - m_wid / 2;
        float posX = (y + 0.5f) - m_len / 2;
        return new Vector3(posX, 0, posZ);
    }

    public List<Point> GeneratePath(Point startPos, Point desPos)
    {
        Point point = FindPath(startPos, desPos);
        List<Point> pathPoints = new List<Point>();
        while (point != startPos && point != null)
        {
            pathPoints.Add(point);
            point = point.m_parent;
        }
        pathPoints.Reverse();
        return pathPoints;
    }

    public Point FindPath(Point startPos, Point endPos)
    {
        m_openList.Clear();                                 //清除上一次寻路遗留（如果有）的数据，防止对本次寻路产生干扰
        m_closeList.Clear();
        m_openList.Add(startPos);                           //将起始点加入open list 
        while (m_openList.Count > 0)
        {   
            m_openList = m_openList.OrderBy(p => p.m_costFactor).ToList();  //对open list按F值排序
            Point currPoint = m_openList[0];                //获得F值最小的点
            m_openList.RemoveAt(0);                         //从open list中删除当前点
            m_closeList.Add(currPoint);                     //将当前点加入到close list中
            foreach (Point point in GetNeibourhood(currPoint))  //遍历当前点的所有“邻居”
            {
                if(m_openList.Contains(point))              //如果open list中已存在该点
                {
                    int tmpCost = CalcValueGiven(currPoint, point) + currPoint.m_valueGiven;    //计算由当前点到达该点的G值
                    if(tmpCost <point.m_valueGiven)         //如果计算的G值小于原本的G值
                    {
                        point.m_parent = currPoint;         //将父节点指向当前点
                        point.m_valueGiven = tmpCost;       //重新计算G值和F值
                        point.CalcCostFactor();
                    }
                }
                else                                        //如果open list中不存在点
                {
                    point.m_parent = currPoint;             //将父节点指向当前点
                    point.m_valueGiven =                    //计算G值、H值和F值
                        CalcValueGiven(currPoint, point) + currPoint.m_valueGiven; 
                    point.m_valueHeur = CalcValueHeur(point, endPos);
                    point.CalcCostFactor();
                    m_openList.Add(point);                  //将该点加入到open list中
                }
            }
            if (m_openList.Contains(endPos))                //如果open lsit中存在目标点，寻路结束，返回目标点
                return endPos;
        }
        return null;                                        //找不到路径，返回空
    }

    public List<Point> GetNeibourhood(Point point)
    {
        List<Point> tmpPoints = new List<Point>();
        for (int x = point.m_x - 1; x <= point.m_x + 1; x++)
        {
            for (int y = point.m_y - 1; y <= point.m_y + 1; y++)
            {
                if (x == point.m_x && y == point.m_y)
                    continue;
                if (0 <= x && x < m_len && 0 <= y && y < m_wid)
                {
                    if (!m_points[x, y].m_isBarrier && !m_closeList.Contains(m_points[x, y]))
                    {
                        tmpPoints.Add(m_points[x, y]);
                    }
                }
            }
        }
        return tmpPoints;
    }

    public int CalcValueGiven(Point start, Point point)
    {
        return (Math.Abs(point.m_x - start.m_x) + Math.Abs(point.m_y - start.m_y)) == 2 ? 14 : 10;
    }

    public int CalcValueHeur(Point start, Point point)
    {
        int xOffset = Math.Abs(start.m_x - point.m_x);
        int yOffset = Math.Abs(start.m_y - point.m_y);

        if(xOffset > yOffset)
        {
            return yOffset * 14 + (xOffset - yOffset) * 10;
        }
        else
        {
            return xOffset * 14 + (yOffset - xOffset) * 10;
        }
    }
}
