using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

public enum EditState
{
    INITIAL,
    MAPEDIT,
    ENTITYEDIT,
    PATHEDIT,
    EVENTEDIT,
    MUSICEDIT,
}

public class EditorManager : MonoBehaviour
{
    public static EditorManager Instance { get; private set; }
    /// <summary>
    /// 当前编辑状态
    /// </summary>
    public static EditState CurrES { get; set; }

    public GameObject m_objLoadMap;
    public GameObject m_objStoreMap;

    private static readonly string m_mapPath = "/Maps/";
    private List<string> m_mapNames = new List<string>();
    private string m_mapName = "";

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
        //绑定m_objStoreMap的事件
        InputField inputField = m_objStoreMap.transform.Find("MapName").GetComponentInChildren<InputField>();
        inputField.onEndEdit.AddListener(delegate { SetMapName(inputField.text); });

        Button btn = m_objStoreMap.transform.Find("Btn_Confirm").GetComponent<Button>();
        btn.onClick.AddListener(delegate { StoreMap(); });

        //绑定m_objLoadMap的事件
        Dropdown dropdown = m_objLoadMap.transform.Find("MapName").GetComponentInChildren<Dropdown>();
        string mapDirPath = Application.dataPath + m_mapPath;
        if (Directory.Exists(mapDirPath))
        {
            //List<string> m_mapNames = new List<string>();
            foreach (string path in Directory.GetFiles(mapDirPath))
            {
                if (path.EndsWith(".xml"))
                {
                    string[] temp = path.Split('/');
                    string tmpStr = temp[temp.Length - 1].Split('.')[0];
                    m_mapNames.Add(tmpStr);
                }
            }
            m_mapName = m_mapNames[0];
            dropdown.ClearOptions();
            dropdown.AddOptions(m_mapNames);
        }
        else
        {
            Debug.Log("Can't Find Sprite Dir Path " + mapDirPath);
        }
        dropdown.onValueChanged.AddListener(delegate { SetMapName(dropdown.captionText.text); });

        btn = m_objLoadMap.transform.Find("Btn_Confirm").GetComponent<Button>();
        btn.onClick.AddListener(delegate { LoadMap(); });

    }

    public void StoreMap()
    {
        if (m_mapName == "")
            return;
        
        string path = Application.dataPath + m_mapPath + m_mapName + ".xml";
        if(File.Exists(path))
        {
            LogManager.Instance.Log(path + " 覆盖保存!");
        }
        if(CreateXml(path))
        {
            List<string> temp = new List<string>() { m_mapName };
            Dropdown dropdown = m_objLoadMap.transform.Find("MapName").GetComponentInChildren<Dropdown>();
            dropdown.AddOptions(temp);
            LogManager.Instance.Log("当前地图已保存!");
        }
    }

    public void LoadMap() 
    {
        if (m_mapName == "")
            return;
        string path = Application.dataPath + m_mapPath + m_mapName + ".xml";
        if (!File.Exists(path))
        {
            LogManager.Instance.Log(path + "不存在！");
        }
        MapManager.Instance.Clear();
        EntityManager.Instance.Clear();
        PathManager.Instance.Clear();
        MusicManager.Instance.Clear();
        if(ParseXml(path))
        {
            LogManager.Instance.Log("加载地图完成！");
        }

    }

    private bool CreateXml(string filePath)
    {
        try
        {
            XmlDocument xmlDocument = new XmlDocument();
            XmlDeclaration xmlDeclaration = xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);
            xmlDocument.AppendChild(xmlDeclaration);

            XmlElement eRoot = xmlDocument.CreateElement("MapData");
            xmlDocument.AppendChild(eRoot);

            //保存地图信息
            XmlElement eMap = xmlDocument.CreateElement("Map");
            Map map = MapManager.Instance.m_map;

            EncryptAttribute(eMap, "TexturePath", map.m_textureName.ToString());
            EncryptAttribute(eMap, "wid", map.m_width.ToString());
            EncryptAttribute(eMap, "len", map.m_length.ToString());

            //eMap.SetAttribute("TexturePath", map.m_textureName.ToString());
            //eMap.SetAttribute("wid", map.m_width.ToString());
            //eMap.SetAttribute("len", map.m_length.ToString());

            eRoot.AppendChild(eMap);

            //保存实体信息
            XmlElement eEntitiesList = xmlDocument.CreateElement("EntitiesList");
            foreach (Entity entity in EntityManager.Instance.GetEntities())
            {
                XmlElement eEntity = xmlDocument.CreateElement("Entity");
                eEntitiesList.AppendChild(eEntity);

                //实体属性（逆序设置，顺序输出）
                XmlElement eEntityAtr = xmlDocument.CreateElement("EntityAtr");

                EncryptAttribute(eEntityAtr, "EventIndex", entity.m_eventIndex.ToString());
                EncryptAttribute(eEntityAtr, "PathIndex", entity.m_pathIndex.ToString());
                EncryptAttribute(eEntityAtr, "MapCubeIndex", entity.m_mapCubeIndex.ToString());
                EncryptAttribute(eEntityAtr, "Type", entity.m_type.ToString());
                EncryptAttribute(eEntityAtr, "Model", entity.m_modelName);

                EncryptAttribute(eEntityAtr, "quaW", entity.m_quaternion.w.ToString());
                EncryptAttribute(eEntityAtr, "quaZ", entity.m_quaternion.z.ToString());
                EncryptAttribute(eEntityAtr, "quaY", entity.m_quaternion.y.ToString());
                EncryptAttribute(eEntityAtr, "quaX", entity.m_quaternion.x.ToString());

                EncryptAttribute(eEntityAtr, "posZ", entity.m_position.z.ToString());
                EncryptAttribute(eEntityAtr, "posY", entity.m_position.y.ToString());
                EncryptAttribute(eEntityAtr, "posX", entity.m_position.x.ToString());

                //eEntityAtr.SetAttribute("PathIndex", entity.m_pathIndex.ToString());
                //eEntityAtr.SetAttribute("MapCubeIndex", entity.m_mapCubeIndex.ToString());
                //eEntityAtr.SetAttribute("Type", entity.m_type.ToString());
                //eEntityAtr.SetAttribute("Model", entity.m_modelName);

                //eEntityAtr.SetAttribute("quaW", entity.m_quaternion.w.ToString());
                //eEntityAtr.SetAttribute("quaZ", entity.m_quaternion.z.ToString());
                //eEntityAtr.SetAttribute("quaY", entity.m_quaternion.y.ToString());
                //eEntityAtr.SetAttribute("quaX", entity.m_quaternion.x.ToString());

                //eEntityAtr.SetAttribute("posZ", entity.m_position.z.ToString());
                //eEntityAtr.SetAttribute("posY", entity.m_position.y.ToString());
                //eEntityAtr.SetAttribute("posX", entity.m_position.x.ToString());
                  
                //游戏属性
                XmlElement eGameAtr = xmlDocument.CreateElement("GameAtr");

                EncryptAttribute(eGameAtr, "IsAtk", entity.IsAtk.ToString());
                EncryptAttribute(eGameAtr, "IsMove", entity.IsMove.ToString());
                EncryptAttribute(eGameAtr, "MoveSpeed", entity.MoveSpeed.ToString());
                EncryptAttribute(eGameAtr, "AtkRange", entity.AtkRange.ToString());
                EncryptAttribute(eGameAtr, "AtkSpeed", entity.AtkSpeed.ToString());
                EncryptAttribute(eGameAtr, "DefValue", entity.DefValue.ToString());
                EncryptAttribute(eGameAtr, "AtkValue", entity.HP.ToString());
                EncryptAttribute(eGameAtr, "HP", entity.HP.ToString());

                //eGameAtr.SetAttribute("IsAtk", entity.IsAtk.ToString());
                //eGameAtr.SetAttribute("IsMove", entity.IsMove.ToString());
                //eGameAtr.SetAttribute("MoveSpeed", entity.MoveSpeed.ToString());
                //eGameAtr.SetAttribute("AtkRange", entity.AtkRange.ToString());
                //eGameAtr.SetAttribute("AtkSpeed", entity.AtkSpeed.ToString());
                //eGameAtr.SetAttribute("DefValue", entity.DefValue.ToString());
                //eGameAtr.SetAttribute("AtkValue", entity.HP.ToString());
                //eGameAtr.SetAttribute("HP", entity.HP.ToString());

                eEntity.AppendChild(eEntityAtr);
                eEntity.AppendChild(eGameAtr);
            }
            eRoot.AppendChild(eEntitiesList);

            //保存路径信息
            XmlElement ePathList = xmlDocument.CreateElement("PathList");
            foreach (Path path in PathManager.Instance.GetPaths())
            {
                //单个路径节点
                XmlElement ePath = xmlDocument.CreateElement("Path");

                //路径的状态信息
                EncryptAttribute(ePath, "IsEmpty", path.m_isEmpty.ToString());
                EncryptAttribute(ePath, "Index", path.m_index.ToString());

                //ePath.SetAttribute("IsEmpty", path.m_isEmpty.ToString());
                //ePath.SetAttribute("Index", path.m_index.ToString());


                //路径的关键点信息
                XmlElement eKeyPointList = xmlDocument.CreateElement("KeyPointList");
                foreach (Point keyPoint in path.m_keyPoints)
                {
                    XmlElement eKeyPoint = xmlDocument.CreateElement("KeyPoint");
                    EncryptAttribute(eKeyPoint, "y", keyPoint.m_y.ToString());
                    EncryptAttribute(eKeyPoint, "x", keyPoint.m_x.ToString());

                    //eKeyPoint.SetAttribute("y", keyPoint.m_y.ToString());
                    //eKeyPoint.SetAttribute("x", keyPoint.m_x.ToString());

                    eKeyPointList.AppendChild(eKeyPoint);
                }
                ePath.AppendChild(eKeyPointList);

                //路径的点信息
                XmlElement ePointList = xmlDocument.CreateElement("PointList");
                foreach (Point point in path.m_points)
                {
                    XmlElement ePoint = xmlDocument.CreateElement("Point");
                    EncryptAttribute(ePoint, "y", point.m_y.ToString());
                    EncryptAttribute(ePoint, "x", point.m_x.ToString());

                    //ePoint.SetAttribute("y", point.m_y.ToString());
                    //ePoint.SetAttribute("x", point.m_x.ToString());

                    ePointList.AppendChild(ePoint);
                }
                ePath.AppendChild(ePointList);

                ePathList.AppendChild(ePath);
            }
            eRoot.AppendChild(ePathList);

            //保存事件信息
            XmlElement eEventList = xmlDocument.CreateElement("EventList");
            foreach(Event @event in EventManager.Instance.GetEvents())
            {
                XmlElement eEvent = xmlDocument.CreateElement("Event");

                EncryptAttribute(eEvent, "Index", @event.m_index.ToString());
                string type = @event.Type;
                //Debug.Log("事件的类型是： " + type);
                EncryptAttribute(eEvent, "Type", type); 
               
                if (type == "Birth")
                {
                    BirthEvent birthEvent = (BirthEvent)@event;
                    EncryptAttribute(eEvent, "CD", birthEvent.m_cd.ToString());
                    EncryptAttribute(eEvent, "Time", birthEvent.m_time.ToString());
                    EncryptAttribute(eEvent, "IsInfinite", birthEvent.m_isInfinite.ToString());
                }
                else if (type == "Bomb")
                {
                    BombEvent bombEvent = (BombEvent)@event;
                    EncryptAttribute(eEvent, "Delay", bombEvent.m_delay.ToString());
                    EncryptAttribute(eEvent, "Hurt", bombEvent.m_hurt.ToString());
                    EncryptAttribute(eEvent, "IsDestroy", bombEvent.m_isDestory.ToString());
                }
                else if (type == "Speed")
                {
                    SpeedEvent speedEvent = (SpeedEvent)@event;
                    EncryptAttribute(eEvent, "Speed", speedEvent.m_speed.ToString());
                    EncryptAttribute(eEvent, "Time", speedEvent.m_time.ToString());
                    EncryptAttribute(eEvent, "IsInfinite", speedEvent.m_isInfinite.ToString());
                }
                else if (type == "Teleport")
                {
                    TeleportEvent teleportEvent = (TeleportEvent)@event;
                    EncryptAttribute(eEvent, "TeleportID", teleportEvent.m_teleportID.ToString());
                    EncryptAttribute(eEvent, "CD", teleportEvent.m_cd.ToString());
                    EncryptAttribute(eEvent, "Time", teleportEvent.m_time.ToString());
                    EncryptAttribute(eEvent, "IsInfinite", teleportEvent.m_isInfinite.ToString());
                }
                eEventList.AppendChild(eEvent);
            }
            eRoot.AppendChild(eEventList); 

            //保存音乐信息
            XmlElement eMusic = xmlDocument.CreateElement("Music");
            Music music = MusicManager.Instance.m_music;
            if (music != null)
            {
                EncryptAttribute(eMusic, "Volume", music.m_volume.ToString());
                EncryptAttribute(eMusic, "Name", music.m_name);

                //eMusic.SetAttribute("Volume", music.m_volume.ToString());
                //eMusic.SetAttribute("Name", music.m_name);
            }  
            eRoot.AppendChild(eMusic);

            //保存XML文件
            xmlDocument.Save(filePath);
            return true;
        }
        catch
        {
            LogManager.Instance.Log("保存地图失败！");
            return false;
        }

    }

    private bool ParseXml(string filePath)
    {
        try
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(filePath);
            XmlElement eRoot = (XmlElement)xmlDocument.SelectSingleNode("MapData");

            foreach (XmlElement element in eRoot.ChildNodes)
            {
                //处理地图信息
                if (element.Name == "Map")
                {
                    int len = int.Parse(DecryptAttribute(element, "len"));
                    int wid = int.Parse(DecryptAttribute(element, "wid"));
                    string textureName = (DecryptAttribute(element, "TexturePath"));

                    //int len = int.Parse(element.GetAttribute("len"));
                    //int wid = int.Parse(element.GetAttribute("wid"));
                    //string textureName = element.GetAttribute("TexturePath");
                    MapManager.Instance.CreateMap(len, wid, textureName);
                }
                else if (element.Name == "EntitiesList")
                {
                    List<Entity> entities = new List<Entity>();
                    foreach (XmlElement eEntity in element.ChildNodes)
                    {
                        //实体属性
                        XmlElement eEntityAtr = (XmlElement)eEntity.SelectSingleNode("EntityAtr");

                        //位置
                        float x = float.Parse(DecryptAttribute(eEntityAtr, "posX"));
                        float y = float.Parse(DecryptAttribute(eEntityAtr, "posY"));
                        float z = float.Parse(DecryptAttribute(eEntityAtr, "posZ"));

                        //float x = float.Parse(eEntityAtr.GetAttribute("posX"));
                        //float y = float.Parse(eEntityAtr.GetAttribute("posY"));
                        //float z = float.Parse(eEntityAtr.GetAttribute("posZ"));

                        Vector3 pos = new Vector3(x, y, z);

                        //旋转
                        x = float.Parse(DecryptAttribute(eEntityAtr, "quaX"));
                        y = float.Parse(DecryptAttribute(eEntityAtr, "quaY"));
                        z = float.Parse(DecryptAttribute(eEntityAtr, "quaZ"));
                        float w = float.Parse(DecryptAttribute(eEntityAtr, "quaW"));

                        //x = float.Parse(eEntityAtr.GetAttribute("quaX"));
                        //y = float.Parse(eEntityAtr.GetAttribute("quaY"));
                        //z = float.Parse(eEntityAtr.GetAttribute("quaZ"));
                        //float w = float.Parse(eEntityAtr.GetAttribute("quaW"));

                        Quaternion qua = new Quaternion(x, y, z, w);

                        //模型
                        string modelName = DecryptAttribute(eEntityAtr, "Model");
                        //string modelName = eEntityAtr.GetAttribute("Model");

                        //类型
                        string temp = DecryptAttribute(eEntityAtr, "Type");
                        //string temp = eEntityAtr.GetAttribute("Type");
                        EntityType entityType = EntityType.Zero;
                        if (temp == "NPC")
                            entityType = EntityType.NPC;
                        else if (temp == "Monster")
                            entityType = EntityType.Monster;
                        else if (temp == "Building")
                            entityType = EntityType.Building;
                        //地图块索引
                        int mapCubeIndex = int.Parse(DecryptAttribute(eEntityAtr, "MapCubeIndex"));
                        //int mapCubeIndex = int.Parse(eEntityAtr.GetAttribute("MapCubeIndex"));
                        //路径索引
                        int pathIndex = int.Parse(DecryptAttribute(eEntityAtr, "PathIndex"));
                        //int pathIndex = int.Parse(eEntityAtr.GetAttribute("PathIndex"));

                        int eventIndex = int.Parse(DecryptAttribute(eEntityAtr, "EventIndex"));

                        //游戏属性
                        XmlElement eGameAtr = (XmlElement)eEntity.SelectSingleNode("GameAtr");
                        float HP = int.Parse(DecryptAttribute(eGameAtr, "HP"));
                        float AtkValue = float.Parse(DecryptAttribute(eGameAtr, "AtkValue"));
                        float DefValue = float.Parse(DecryptAttribute(eGameAtr, "DefValue"));
                        float AtkSpeed = float.Parse(DecryptAttribute(eGameAtr, "AtkSpeed"));
                        float AtkRange = float.Parse(DecryptAttribute(eGameAtr, "AtkRange"));
                        float MoveSpeed = float.Parse(DecryptAttribute(eGameAtr, "MoveSpeed"));
                        bool IsMove = bool.Parse(DecryptAttribute(eGameAtr, "IsMove"));
                        bool IsAtk = bool.Parse(DecryptAttribute(eGameAtr, "IsAtk"));

                        //float HP = int.Parse(eGameAtr.GetAttribute("HP"));
                        //float AtkValue = float.Parse(eGameAtr.GetAttribute("AtkValue"));
                        //float DefValue = float.Parse(eGameAtr.GetAttribute("DefValue"));
                        //float AtkSpeed = float.Parse(eGameAtr.GetAttribute("AtkSpeed"));
                        //float AtkRange = float.Parse(eGameAtr.GetAttribute("AtkRange"));
                        //float MoveSpeed = float.Parse(eGameAtr.GetAttribute("MoveSpeed")); 
                        //bool IsMove = bool.Parse(eGameAtr.GetAttribute("IsMove"));
                        //bool IsAtk = bool.Parse(eGameAtr.GetAttribute("IsAtk"));

                        //添加到当前实体管理类
                        EntityManager.Instance.AddEntity(pos, qua, entityType, modelName, mapCubeIndex, pathIndex, eventIndex, HP, AtkValue, DefValue, AtkSpeed, AtkRange, MoveSpeed, IsMove, IsAtk);
                        //更新地图块的状态
                        MapManager.Instance.GetMapCube(mapCubeIndex).m_isEmpty = false;
                    }
                }
                else if (element.Name == "PathList")
                {
                    //根据当前地图信息生成寻路数据
                    PathManager.Instance.GeneratePointMsg();

                    //读取并加载存档中的路径信息
                    foreach (XmlElement ePath in element.ChildNodes)
                    {
                        //状态信息
                        int index = int.Parse(DecryptAttribute(ePath, "Index"));
                        bool isEmpty = bool.Parse(DecryptAttribute(ePath, "IsEmpty"));

                        //int index = int.Parse(ePath.GetAttribute("Index"));
                        //bool isEmpty = bool.Parse(ePath.GetAttribute("IsEmpty"));

                        //关键点信息
                        XmlElement eKeyPointList = (XmlElement)ePath.SelectSingleNode("KeyPointList");
                        List<Vector2> tempKeyPointList = new List<Vector2>();
                        foreach (XmlElement eKeyPoint in eKeyPointList.ChildNodes)
                        {
                            Vector2 tempKeyPoint = new Vector2
                            {
                                x = int.Parse(DecryptAttribute(eKeyPoint, "x")),
                                y = int.Parse(DecryptAttribute(eKeyPoint, "y"))
                                //x = int.Parse(eKeyPoint.GetAttribute("x")),
                                //y = int.Parse(eKeyPoint.GetAttribute("y"))
                            };
                            tempKeyPointList.Add(tempKeyPoint);
                        }

                        //点信息
                        XmlElement ePointList = (XmlElement)ePath.SelectSingleNode("PointList");
                        List<Vector2> tempPointList = new List<Vector2>();
                        foreach (XmlElement ePoint in ePointList.ChildNodes)
                        {
                            Vector2 tempPoint = new Vector2
                            {
                                x = int.Parse(DecryptAttribute(ePoint, "x")),
                                y = int.Parse(DecryptAttribute(ePoint, "y"))
                                //x = int.Parse(ePoint.GetAttribute("x")),
                                //y = int.Parse(ePoint.GetAttribute("y"))
                            };
                            tempPointList.Add(tempPoint);
                        }
                        //添加到当前路径管理类
                        PathManager.Instance.AddPath(index, isEmpty, tempKeyPointList, tempPointList);
                    }
                }
                else if(element.Name == "EventList")
                {
                    foreach(XmlElement eEvent in element.ChildNodes)
                    {
                        int index = int.Parse(DecryptAttribute(eEvent, "Index"));
                        string type = DecryptAttribute(eEvent, "Type");
                        if (type.CompareTo("Birth") == 0)
                        {
                            Debug.Log("BIRTHEVENT: " + type);
                            float cd = float.Parse(DecryptAttribute(eEvent, "CD"));
                            float time = float.Parse(DecryptAttribute(eEvent, "Time"));
                            bool isInfinite = bool.Parse(DecryptAttribute(eEvent, "IsInfinite"));
                            BirthEvent birthEvent = new BirthEvent(cd, time, isInfinite);
                            EventManager.Instance.AddBirthEvent(birthEvent);
                        }
                        else if (type.CompareTo("Bomb") == 0)
                        {
                            float delay = float.Parse(DecryptAttribute(eEvent, "Delay"));
                            float hurt = float.Parse(DecryptAttribute(eEvent, "Hurt"));
                            bool isDetroy = bool.Parse(DecryptAttribute(eEvent, "IsDestroy"));
                            BombEvent bombEvent = new BombEvent(delay, hurt, isDetroy);
                            EventManager.Instance.AddBombEvent(bombEvent);
                        }
                        else if (type.CompareTo("Speed") == 0)
                        {
                            float speed = float.Parse(DecryptAttribute(eEvent, "Speed"));
                            float time = float.Parse(DecryptAttribute(eEvent, "Time"));
                            bool isInfinite = bool.Parse(DecryptAttribute(eEvent, "IsInfinite"));
                            SpeedEvent speedEvent = new SpeedEvent(speed, time, isInfinite);
                            EventManager.Instance.AddSpeedEvent(speedEvent);
                        }
                        else if (type.CompareTo("Telepo") == 0)
                        {
                            int teleportID = int.Parse(DecryptAttribute(eEvent, "TeleportID"));
                            float cd = float.Parse(DecryptAttribute(eEvent, "CD"));
                            float time = float.Parse(DecryptAttribute(eEvent, "Time"));
                            bool isInfinite = bool.Parse(DecryptAttribute(eEvent, "IsInfinite"));
                            TeleportEvent teleportEvent = new TeleportEvent(teleportID, cd, time, isInfinite);
                            EventManager.Instance.AddTeleportEvent(teleportEvent);
                        }
                    }
                }
                else if (element.Name == "Music")
                {
                    if (element.HasAttributes)
                    {
                        string name = DecryptAttribute(element, "Name");
                        float volume = float.Parse(DecryptAttribute(element, "Volume"));
                        //string name = element.GetAttribute("Name");
                        //float volume = float.Parse(element.GetAttribute("Volume"));
                        MusicManager.Instance.SetMusic(name, volume);
                    }
                }
            }
            return true;
        }
        catch
        {
            LogManager.Instance.Log("加载地图失败！");
            return false;
        }
    }

    public void SetMapName(string name)
    {
        this.m_mapName = name;
    }

    public void EncryptAttribute(XmlElement element, string name, string vale)
    {
        string data = DESEncryptor.Encrypt(vale);
        element.SetAttribute(name, data);
    }

    public string DecryptAttribute(XmlElement element, string name)
    {
        string data = element.GetAttribute(name);
        return DESEncryptor.Decrypt(data);
    }
}
