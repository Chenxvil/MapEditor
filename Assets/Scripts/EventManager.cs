using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }

    public GameObject m_objNewEvent;
    public GameObject m_objEventAttribute;
    public GameObject m_objBirthEvent;
    public GameObject m_objBombEvent;
    public GameObject m_objSpeedEvent;
    public GameObject m_objTeleportEvent;
    public Dropdown m_dpdEventList;

    public bool m_isEdit { get; set; } = false;

    private Event m_currEvent;
    private List<Event> m_events = new List<Event>();
    private List<string> m_eventNames = new List<string>();
     
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
        m_dpdEventList.ClearOptions();
        m_dpdEventList.onValueChanged.AddListener(delegate { SelectEvent(m_dpdEventList.value); });

        //绑定添加事件的监听
        Button buttonNewEvent = m_objNewEvent.transform.Find("Btn_confirm").GetComponent<Button>();
        Dropdown dropdown = m_objNewEvent.transform.Find("EventType").GetComponentInChildren<Dropdown>();
        buttonNewEvent.onClick.AddListener(delegate { AddEvent(dropdown.captionText.text); });

        //添加删除事件的监听
        if(m_objEventAttribute.transform.Find("Btn_Delete") != null)
        {
            Button buttonDeleteEvent = m_objEventAttribute.transform.Find("Btn_Delete").GetComponent<Button>();
            buttonDeleteEvent.onClick.AddListener(delegate { DeleteEvent(m_dpdEventList.value); });
        }

        //绑定出生事件的各个控件监听
        InputField inputField00 = m_objBirthEvent.transform.Find("CD").GetComponentInChildren<InputField>();
        inputField00.onEndEdit.AddListener(delegate { SetBirthCD(inputField00.text); });
        InputField inputField01 = m_objBirthEvent.transform.Find("Time").GetComponentInChildren<InputField>();
        inputField01.onEndEdit.AddListener(delegate { SetBirthTime(inputField01.text); });
        Toggle toggle0 = m_objBirthEvent.transform.Find("IsInfinite").GetComponentInChildren<Toggle>();
        toggle0.onValueChanged.AddListener(delegate { SetBirthISF(toggle0.isOn); });

        //绑定爆炸事件的各个控件监听
        InputField inputField10 = m_objBombEvent.transform.Find("Delay").GetComponentInChildren<InputField>();
        inputField10.onEndEdit.AddListener(delegate { SetBombDelay(inputField10.text); });
        InputField inputField11 = m_objBombEvent.transform.Find("Hurt").GetComponentInChildren<InputField>();
        inputField11.onEndEdit.AddListener(delegate { SetBombHurt(inputField11.text); });
        Toggle toggle1 = m_objBombEvent.transform.Find("IsDestroy").GetComponentInChildren<Toggle>();
        toggle1.onValueChanged.AddListener(delegate { SetBombISD(toggle1.isOn); });

        //绑定速度事件的各个控件监听
        InputField inputField20 = m_objSpeedEvent.transform.Find("Speed").GetComponentInChildren<InputField>();
        inputField20.onEndEdit.AddListener(delegate { SetSpeedSpeed(inputField20.text); });
        InputField inputField21 = m_objSpeedEvent.transform.Find("Time").GetComponentInChildren<InputField>();
        inputField21.onEndEdit.AddListener(delegate { SetSpeedTime(inputField21.text); });
        Toggle toggle2 = m_objSpeedEvent.transform.Find("IsInfinite").GetComponentInChildren<Toggle>();
        toggle2.onValueChanged.AddListener(delegate { SetSpeedISF(toggle2.isOn); });

        //绑定传送事件的各个控件监听
        InputField inputField30 = m_objTeleportEvent.transform.Find("TeleportID").GetComponentInChildren<InputField>();
        inputField30.onEndEdit.AddListener(delegate { SetTeleportID(inputField30.text); });
        InputField inputField31 = m_objTeleportEvent.transform.Find("CD").GetComponentInChildren<InputField>();
        inputField31.onEndEdit.AddListener(delegate { SetTeleportCD(inputField31.text); });
        InputField inputField32 = m_objTeleportEvent.transform.Find("Time").GetComponentInChildren<InputField>();
        inputField32.onEndEdit.AddListener(delegate { SetTeleportTime(inputField32.text); });
        Toggle toggle3 = m_objTeleportEvent.transform.Find("IsInfinite").GetComponentInChildren<Toggle>();
        toggle3.onValueChanged.AddListener(delegate { SetTeleportISF(toggle3.isOn); });
    }
     
    public List<Event> GetEvents()
    {
        return m_events;
    }

    public void Edit()
    {
        m_isEdit = !m_isEdit;
        m_objEventAttribute.SetActive(m_isEdit);
        if(m_isEdit)
        {
            LogManager.Instance.Log("进入事件编辑");
        }
        else
        {
            LogManager.Instance.Log("退出事件编辑");
        }
    }



    public void SelectEvent(int index)
    {
        if (index >= m_events.Count)
            return;
        Debug.Log(index);
        m_currEvent = m_events[index];
        DisplayEvent();

    }

    //添加事件
    public void AddEvent(string eventType)
    {
        if(eventType == "出生")
        {
            m_currEvent = new BirthEvent();
            m_events.Add(m_currEvent);
        }
        else if (eventType == "爆炸")
        {
            m_currEvent = new BombEvent();
            m_events.Add(m_currEvent);
        } 
        else if(eventType == "传送")
        {
            m_currEvent = new TeleportEvent();
            m_events.Add(m_currEvent);
        }
        else if(eventType == "速度")
        {
            m_currEvent = new SpeedEvent();
            m_events.Add(m_currEvent);
        }
        else
        {
            LogManager.Instance.Log("事件类型不存在！");
            return;
        }
        string name = "事件" + m_events.Count;
        m_eventNames.Add(name);
        m_dpdEventList.ClearOptions();
        m_dpdEventList.AddOptions(m_eventNames);
        m_dpdEventList.value = m_events.Count;
        DisplayEvent();
        LogManager.Instance.Log("添加事件成功！");
    }
    public void AddBirthEvent(BirthEvent birthEvent)
    {
        Debug.Log("EVENT: ");
        m_events.Add(birthEvent);
        string name = "事件" + m_events.Count;
        m_eventNames.Add(name);
        m_dpdEventList.ClearOptions();
        m_dpdEventList.AddOptions(m_eventNames);
        m_dpdEventList.value = m_events.Count;
    }

    public void AddBombEvent(BombEvent bombEvent)
    {
        m_events.Add(bombEvent);
        string name = "事件" + m_events.Count;
        m_eventNames.Add(name);
        m_dpdEventList.ClearOptions();
        m_dpdEventList.AddOptions(m_eventNames);
        m_dpdEventList.value = m_events.Count;
    }

    public void AddSpeedEvent(SpeedEvent speedEvent)
    {
        m_events.Add(speedEvent);
        string name = "事件" + m_events.Count;
        m_eventNames.Add(name);
        m_dpdEventList.ClearOptions();
        m_dpdEventList.AddOptions(m_eventNames);
        m_dpdEventList.value = m_events.Count;
    }

    public void AddTeleportEvent(TeleportEvent teleportEvent)
    {
        m_events.Add(teleportEvent);
        string name = "事件" + m_events.Count;
        m_eventNames.Add(name);
        m_dpdEventList.ClearOptions();
        m_dpdEventList.AddOptions(m_eventNames);
        m_dpdEventList.value = m_events.Count;
    }

    //显示事件属性
    private void DisplayEvent()
    {
        m_objBirthEvent.SetActive(false);
        m_objBombEvent.SetActive(false);
        m_objSpeedEvent.SetActive(false);
        m_objTeleportEvent.SetActive(false);

        string eventType = m_currEvent.Type;
        Transform tempTransform;
        if (eventType == "Birth")
        {
            m_objBirthEvent.SetActive(true);
            BirthEvent birthEvent = (BirthEvent)m_currEvent;
            tempTransform = m_objBirthEvent.transform.Find("Index");
            tempTransform.Find("Text").GetComponent<Text>().text = birthEvent.m_index.ToString();
            tempTransform = m_objBirthEvent.transform.Find("CD");
            tempTransform.GetComponentInChildren<InputField>().text = birthEvent.m_cd.ToString();
            tempTransform = m_objBirthEvent.transform.Find("Time");
            tempTransform.GetComponentInChildren<InputField>().text = birthEvent.m_time.ToString();
            tempTransform = m_objBirthEvent.transform.Find("IsInfinite");
            tempTransform.GetComponentInChildren<Toggle>().isOn = birthEvent.m_isInfinite;
        }
        else if (eventType == "Bomb")
        {
            m_objBombEvent.SetActive(true);
            BombEvent bombEvent = (BombEvent)m_currEvent;
            tempTransform = m_objBombEvent.transform.Find("Index");
            tempTransform.Find("Text").GetComponent<Text>().text = bombEvent.m_index.ToString();
            tempTransform = m_objBombEvent.transform.Find("Delay");
            tempTransform.GetComponentInChildren<InputField>().text = bombEvent.m_delay.ToString();
            tempTransform = m_objBombEvent.transform.Find("Hurt");
            tempTransform.GetComponentInChildren<InputField>().text = bombEvent.m_hurt.ToString();
            tempTransform = m_objBombEvent.transform.Find("IsDestroy");
            tempTransform.GetComponentInChildren<Toggle>().isOn = bombEvent.m_isDestory;
        }
        else if (eventType == "Speed")
        {
            m_objSpeedEvent.SetActive(true);
            SpeedEvent speedEvent = (SpeedEvent)m_currEvent;
            tempTransform = m_objSpeedEvent.transform.Find("Index");
            tempTransform.Find("Text").GetComponent<Text>().text = speedEvent.m_index.ToString();
            tempTransform = m_objSpeedEvent.transform.Find("Speed");
            tempTransform.GetComponentInChildren<InputField>().text = speedEvent.m_speed.ToString();
            tempTransform = m_objSpeedEvent.transform.Find("Time");
            tempTransform.GetComponentInChildren<InputField>().text = speedEvent.m_time.ToString();
            tempTransform = m_objSpeedEvent.transform.Find("IsInfinite");
            tempTransform.GetComponentInChildren<Toggle>().isOn = speedEvent.m_isInfinite;
        }
        else if (eventType == "Teleport")
        {
            m_objTeleportEvent.SetActive(true);
            TeleportEvent teleportEvent = (TeleportEvent)m_currEvent;
            tempTransform = m_objTeleportEvent.transform.Find("Index");
            tempTransform.Find("Text").GetComponent<Text>().text = teleportEvent.m_index.ToString();
            tempTransform = m_objTeleportEvent.transform.Find("TeleportID");
            tempTransform.GetComponentInChildren<InputField>().text = teleportEvent.m_teleportID.ToString();
            tempTransform = m_objTeleportEvent.transform.Find("CD");
            tempTransform.GetComponentInChildren<InputField>().text = teleportEvent.m_cd.ToString();
            tempTransform = m_objTeleportEvent.transform.Find("Time");
            tempTransform.GetComponentInChildren<InputField>().text = teleportEvent.m_time.ToString();
            tempTransform = m_objTeleportEvent.transform.Find("IsInfinite");
            tempTransform.GetComponentInChildren<Toggle>().isOn = teleportEvent.m_isInfinite;
        }
        else
        {
            LogManager.Instance.Log("事件类型不存在！");
            return;
        }

    }

    //删除事件
    public void DeleteEvent(int index)
    {
        m_events.RemoveAt(index);
        m_eventNames.RemoveAt(index);
        m_dpdEventList.ClearOptions();
        m_dpdEventList.AddOptions(m_eventNames);
    }

    //出生事件
    private void SetBirthCD(string value)
    {
        ((BirthEvent)m_currEvent).m_cd = float.Parse(value);
    }
    private void SetBirthTime(string value)
    {
        ((BirthEvent)m_currEvent).m_time = float.Parse(value);
    }
    private void SetBirthISF(bool value)
    {
        ((BirthEvent)m_currEvent).m_isInfinite = value;
    }

    //爆炸事件
    private void SetBombDelay(string value)
    {
        ((BombEvent)m_currEvent).m_delay = float.Parse(value);
    }
    private void SetBombHurt(string value)
    {
        ((BombEvent)m_currEvent).m_hurt = float.Parse(value);
    }
    private void SetBombISD(bool value)
    {
        ((BombEvent)m_currEvent).m_isDestory = value;
    }

    //速度事件
    private void SetSpeedISF(bool value)
    {
        ((SpeedEvent)m_currEvent).m_isInfinite = value;
    }
    private void SetSpeedTime(string value)
    {
        ((SpeedEvent)m_currEvent).m_time = float.Parse(value);
    }
    private void SetSpeedSpeed(string value)
    {
        ((SpeedEvent)m_currEvent).m_speed = float.Parse(value);
    }

    //传送事件
    private void SetTeleportISF(bool value)
    {
        ((TeleportEvent)m_currEvent).m_isInfinite = value;
    }
    private void SetTeleportTime(string value)
    {
        ((TeleportEvent)m_currEvent).m_time =float.Parse(value);
    }
    private void SetTeleportCD(string value)
    {
        ((TeleportEvent)m_currEvent).m_cd = float.Parse(value);
    }
    private void SetTeleportID(string value)
    {
        ((TeleportEvent)m_currEvent).m_teleportID = int.Parse(value);
    }
}
