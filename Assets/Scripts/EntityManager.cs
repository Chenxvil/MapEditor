using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EntityManager : MonoBehaviour
{
    public static EntityManager Instance { get; private set; }

    //二级界面UI控件的引用
    public Dropdown m_dropdownType;
    public Dropdown m_dropdownModel;

    //属性设置区的UI控件的引用
    public GameObject m_objEntityAttribute;
    private Text m_text;
    private Dropdown m_dropdown;
    private InputField[] m_inputFields = new InputField[8];
    private Toggle[] m_toggles = new Toggle[2];

    //实体的父物体
    public GameObject m_parentEntity;
    //当前实体预设体
    private GameObject m_currPrefab;
    //当前实体
    private GameObject m_currEntity;
    //实体列表
    private List<GameObject> m_objEntities = new List<GameObject>();

    //是否建造模式
    public bool m_isBuild { get; set; } = false;

    private readonly string m_prefabDirPath = "Prefabs/";

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
        Transform entityAttrTF = m_objEntityAttribute.transform;

        m_text = entityAttrTF.Find("Name").GetComponentInChildren<Text>();

        m_dropdown = entityAttrTF.Find("Type").GetComponentInChildren<Dropdown>();
        m_dropdown.onValueChanged.AddListener(delegate { SetType(m_dropdown.captionText.text); });

        m_inputFields[0] = entityAttrTF.Find("HP").GetComponentInChildren<InputField>();
        m_inputFields[0].onEndEdit.AddListener(delegate { SetHP(m_inputFields[0].text); });

        m_inputFields[1] = entityAttrTF.Find("AtkValue").GetComponentInChildren<InputField>();
        m_inputFields[1].onEndEdit.AddListener(delegate { SetAtkValue(m_inputFields[1].text); });

        m_inputFields[2] = entityAttrTF.Find("DefValue").GetComponentInChildren<InputField>();
        m_inputFields[2].onEndEdit.AddListener(delegate { SetDefValue(m_inputFields[2].text); });

        m_inputFields[3] = entityAttrTF.Find("AtkSpeed").GetComponentInChildren<InputField>();
        m_inputFields[3].onEndEdit.AddListener(delegate { SetAtkSpeed(m_inputFields[3].text); });

        m_inputFields[4] = entityAttrTF.Find("AtkRange").GetComponentInChildren<InputField>();
        m_inputFields[4].onEndEdit.AddListener(delegate { SetAtkRange(m_inputFields[4].text); });

        m_inputFields[5] = entityAttrTF.Find("MoveSpeed").GetComponentInChildren<InputField>();
        m_inputFields[5].onEndEdit.AddListener(delegate { SetMoveSpeed(m_inputFields[5].text); });

        m_inputFields[6] = entityAttrTF.Find("PathIndex").GetComponentInChildren<InputField>();
        m_inputFields[6].onEndEdit.AddListener(delegate { SetPathIndex(m_inputFields[6].text); });

        m_inputFields[7] = entityAttrTF.Find("EventIndex").GetComponentInChildren<InputField>();
        m_inputFields[7].onEndEdit.AddListener(delegate { SetEventIndex(m_inputFields[7].text); });

        m_toggles[0] = entityAttrTF.Find("IsMove").GetComponentInChildren<Toggle>();
        m_toggles[0].onValueChanged.AddListener(delegate { SetIsMove(m_toggles[0].isOn); });

        m_toggles[1] = entityAttrTF.Find("IsAtk").GetComponentInChildren<Toggle>();
        m_toggles[1].onValueChanged.AddListener(delegate { SetIsAtk(m_toggles[1].isOn); });

        Button Btn_Rotate = entityAttrTF.Find("Btn_Rotate").GetComponentInChildren<Button>();
        Btn_Rotate.onClick.AddListener(delegate { RotateEntity(45.0f); });

        Button Btn_Delete = entityAttrTF.Find("Btn_Delete").GetComponentInChildren<Button>();
        Btn_Delete.onClick.AddListener(delegate { DeleteEntity(); });

    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject() && !m_isBuild && !PathManager.Instance.m_isBuild)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            bool isExist = Physics.Raycast(ray, out RaycastHit raycastHit, 1000, LayerMask.GetMask("Entity"));
            if (isExist)
            {
                GameObject entity = raycastHit.collider.gameObject;
                SatrtEdit(entity);
            }
        }
        if(Input.GetMouseButtonDown(1)&&!EventSystem.current.IsPointerOverGameObject() && !m_isBuild)
        {
            if(m_currEntity!=null)
            {
                m_currEntity.GetComponentInChildren<SkinnedMeshRenderer>().material.color = Color.white;
                m_objEntityAttribute.SetActive(false);
                m_currEntity = null;
            }
        }
    }

    public void CreateEntityPrefab()
    {
        if(MapManager.Instance.m_map == null)
        {
            LogManager.Instance.LogWarnning("创建物体失败，请先创建地图!");
            return;
        }
        string text = m_dropdownModel.captionText.text;
        string path = m_prefabDirPath + text;
        m_currPrefab = Resources.Load<GameObject>(path);

        //m_isBuild = true;
        EditorManager.CurrES = EditState.ENTITYEDIT;
        LogManager.Instance.Log("创建物体预设体成功，请选择放置位置!");
    }

    public void CreateEntity(Vector3 pos, int mapCubeIndex)
    {
        GameObject objEntity = GameObject.Instantiate<GameObject>(m_currPrefab, pos, Quaternion.identity, m_parentEntity.transform);
        objEntity.GetComponent<Entity>().m_position = pos;
        objEntity.GetComponent<Entity>().m_quaternion = Quaternion.identity;
        objEntity.GetComponent<Entity>().m_mapCubeIndex = mapCubeIndex;

        string text = m_dropdownType.captionText.text;
        EntityType entityType = EntityType.Zero;
        if (text == "NPC")
        {
            entityType = EntityType.NPC;
        }
        if (text == "Monster")
        {
            entityType = EntityType.Monster;
        }
        if(text == "Building")
        {
            entityType = EntityType.Building;
        }
        objEntity.GetComponent<Entity>().m_type = entityType;

        text = m_dropdownModel.captionText.text;
        objEntity.GetComponent<Entity>().m_modelName = text;

        m_objEntities.Add(objEntity);
        m_isBuild = false;
        EditorManager.CurrES = EditState.MAPEDIT;
        LogManager.Instance.Log("创建物体成功!");
    }

    public List<Entity> GetEntities()
    {
        List<Entity> entities = new List<Entity>();
        foreach(GameObject objEntity in m_objEntities)
        {
            Entity entity = objEntity.GetComponent<Entity>();
            entities.Add(entity);
        }
        return entities;
    }

    public void SatrtEdit(GameObject entity)
    {
        if(m_currEntity!=null)
        {
            m_currEntity.GetComponentInChildren<SkinnedMeshRenderer>().material.color = Color.white;
        }
        m_currEntity = entity;
        m_currEntity.GetComponentInChildren<SkinnedMeshRenderer>().material.color = Color.green;

        m_objEntityAttribute.SetActive(true);

        m_text.text = entity.GetComponent<Entity>().m_modelName;

        EntityType type = entity.GetComponent<Entity>().m_type;
        if(type == EntityType.NPC)
        {
            m_dropdown.captionText.text = "NPC";
        }
        else if(type==EntityType.Monster )
        {
            m_dropdown.captionText.text = "Monster";
        }
        else if(type==EntityType.Building)
        {
            m_dropdown.captionText.text = "Building";
        }
        else
        {
            m_dropdown.captionText.text = "Zero";
        }
           
        m_inputFields[0].text = entity.GetComponent<Entity>().HP.ToString();
        m_inputFields[1].text = entity.GetComponent<Entity>().AtkValue.ToString();
        m_inputFields[2].text = entity.GetComponent<Entity>().DefValue.ToString();
        m_inputFields[3].text = entity.GetComponent<Entity>().AtkSpeed.ToString();
        m_inputFields[4].text = entity.GetComponent<Entity>().AtkRange.ToString();
        m_inputFields[5].text = entity.GetComponent<Entity>().MoveSpeed.ToString();
        m_inputFields[6].text = entity.GetComponent<Entity>().m_pathIndex.ToString();
        m_inputFields[7].text = entity.GetComponent<Entity>().m_eventIndex.ToString();

        m_toggles[0].isOn = entity.GetComponent<Entity>().IsMove;
        m_toggles[1].isOn = entity.GetComponent<Entity>().IsAtk;
    }

    public void SetType(string s)
    {
        if (s == "NPC")
        {
            m_currEntity.GetComponent<Entity>().m_type = EntityType.NPC;
        }
        else if (s == "Monster")
        {
            m_currEntity.GetComponent<Entity>().m_type = EntityType.Monster;
        }
        else if (s == "Building")
        {
            m_currEntity.GetComponent<Entity>().m_type = EntityType.Building;
        }
    }

    public void SetHP(string s)
    {
        m_currEntity.GetComponent<Entity>().HP = int.Parse(s);
    }

    public void SetAtkValue(string s)
    {
        m_currEntity.GetComponent<Entity>().AtkValue = int.Parse(s);
    }

    public void SetDefValue(string s)
    {
        m_currEntity.GetComponent<Entity>().DefValue = int.Parse(s);
    }

    public void SetAtkSpeed(string s)
    {
        m_currEntity.GetComponent<Entity>().AtkSpeed = int.Parse(s);
    }

    public void SetAtkRange(string s)
    {
        m_currEntity.GetComponent<Entity>().AtkRange = int.Parse(s);
    }

    public void SetMoveSpeed(string s)
    {
        m_currEntity.GetComponent<Entity>().MoveSpeed = int.Parse(s);
    }

    public void SetPathIndex(string s)
    {
        m_currEntity.GetComponent<Entity>().m_pathIndex = int.Parse(s);
    }

    public void SetEventIndex(string s)
    {
        m_currEntity.GetComponent<Entity>().m_eventIndex = int.Parse(s);
    }

    public void SetIsMove(bool isMove)
    {
        m_currEntity.GetComponent<Entity>().IsMove = isMove;
    }

    public void SetIsAtk(bool isAtk)
    {
        m_currEntity.GetComponent<Entity>().IsAtk = isAtk;
    }

    public void RotateEntity(float angle)
    {
        m_currEntity.transform.Rotate(Vector3.up, angle);
        m_currEntity.GetComponent<Entity>().m_quaternion = m_currEntity.transform.rotation;
    }

    public void DeleteEntity()
    {
        GameObject mapCube = MapManager.Instance.GetObjMapCube(m_currEntity.GetComponent<Entity>().m_mapCubeIndex);
        mapCube.GetComponent<MapCube>().m_isEmpty = true;
        m_objEntities.Remove(m_currEntity);
        Destroy(m_currEntity);
        m_currEntity = null;
        ExitEdit();
        LogManager.Instance.Log("删除物体成功!");
    }

    public void ExitEdit()
    {
        m_objEntityAttribute.SetActive(false);
    }

    public void SetEntities(List<Entity> entities)
    {
        foreach(Entity entity in entities)
        {
            //创建物体
            string name = entity.m_modelName;
            string path = m_prefabDirPath + name;
            Debug.Log(path);
            m_currPrefab = Resources.Load<GameObject>(path);
            GameObject objEntity = GameObject.Instantiate<GameObject>(m_currPrefab, entity.m_position, Quaternion.identity, m_parentEntity.transform);

            //设置物体信息
            Entity tempEntity = objEntity.GetComponent<Entity>();
            tempEntity.m_position = entity.m_position;
            tempEntity.m_modelName = entity.m_modelName;
            tempEntity.m_type = entity.m_type;
            tempEntity.m_mapCubeIndex = entity.m_mapCubeIndex;
            tempEntity.m_pathIndex = entity.m_pathIndex;

            tempEntity.HP = entity.HP;
            tempEntity.AtkValue = entity.AtkValue;
            tempEntity.DefValue = entity.DefValue;
            tempEntity.AtkSpeed = entity.AtkSpeed;
            tempEntity.AtkRange = entity.AtkRange;
            tempEntity.MoveSpeed = entity.MoveSpeed;
            tempEntity.IsMove = entity.IsMove;
            tempEntity.IsAtk = entity.IsAtk;

            m_objEntities.Add(objEntity);
        }
    }

    public void AddEntity(Vector3 pos, Quaternion qua, EntityType type, string name, int mapCubeIndex, int pathIndex, int eventIndex,
        float hp, float atkV, float defV, float atkS, float atkR, float moveS, bool isM, bool isA)
    {
        //创建物体
        string path = m_prefabDirPath + name;
        Debug.Log(path);
        m_currPrefab = Resources.Load<GameObject>(path);
        GameObject objEntity = GameObject.Instantiate<GameObject>(m_currPrefab, pos, qua, m_parentEntity.transform);

        //设置物体信息
        Entity tempEntity = objEntity.GetComponent<Entity>();
        tempEntity.m_position = pos;
        tempEntity.m_quaternion = qua;
        tempEntity.m_modelName = name;
        tempEntity.m_type = type;
        tempEntity.m_mapCubeIndex = mapCubeIndex;
        tempEntity.m_pathIndex = pathIndex;
        tempEntity.m_eventIndex = eventIndex;

        tempEntity.HP = hp;
        tempEntity.AtkValue = atkV;
        tempEntity.DefValue = defV;
        tempEntity.AtkSpeed = atkS;
        tempEntity.AtkRange = atkR;
        tempEntity.MoveSpeed = moveS;
        tempEntity.IsMove = isM;
        tempEntity.IsAtk = isA;

        m_objEntities.Add(objEntity);
    }

    public void Clear()
    {
        //清除所有实体信息
        foreach(GameObject objEntity in m_objEntities)
        {
            Destroy(objEntity);
        }
        m_objEntities.Clear();

        //清除状态
        m_currEntity = null;
        m_isBuild = false;
        LogManager.Instance.Log("清除所有实体信息成功!");
    }
}
