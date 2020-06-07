using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LogManager : MonoBehaviour
{
    //单实例类
    public static LogManager Instance { get; private set; }


    public Text m_textLog;

    private static string m_path;
    private List<string> m_logs = new List<string>();

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
    void Start()
    {
        m_textLog.text = string.Empty;
        m_path = Application.dataPath + "/Log/log.txt";
    }

    /// <summary>
    /// 打印提示信息
    /// </summary>
    /// <param name="msg">需要打印的消息</param>
    public void Log(string msg)
    {
        m_textLog.color = Color.green;
        m_textLog.text = "Tip " + msg;
        DateTime NowTime = DateTime.Now.ToLocalTime();
        string time = NowTime.ToString("yyyy-MM-dd HH:mm:ss");
        m_logs.Add(time + "(Tip) : "+msg);
    }

    /// <summary>
    /// 打印错误信息
    /// </summary>
    /// <param name="msg"></param>
    public void LogError(string msg)
    {
        m_textLog.color = Color.red;
        m_textLog.text = "Error " + msg;
        DateTime NowTime = DateTime.Now.ToLocalTime();
        string time = NowTime.ToString("yyyy-MM-dd HH:mm:ss");
        m_logs.Add(time + "(Error) : " + msg);
    }

    /// <summary>
    /// 打印警告信息
    /// </summary>
    /// <param name="msg"></param>
    public void LogWarnning(string msg)
    {
        m_textLog.color = Color.yellow;
        m_textLog.text = "Warnning " + msg;
        DateTime NowTime = DateTime.Now.ToLocalTime();
        string time = NowTime.ToString("yyyy-MM-dd HH:mm:ss");
        m_logs.Add(time + "(Warnning) : " + msg);
    }

    private void OnDestroy()
    {
        if (StoreLogs())
            Debug.Log("保存日志文件成功！");
        else
            Debug.Log("保存日志文件失败！");
    }
     
    private bool StoreLogs()
    {
        try
        {
            StreamWriter streamWriter;
            FileInfo fileInfo = new FileInfo(m_path);
            streamWriter = fileInfo.CreateText();
            foreach (string log in m_logs)
            {
                streamWriter.WriteLine(log);
            }
            streamWriter.Close();
            streamWriter.Dispose();
            return true;
        }
        catch
        {
            return false;
        }
    }
}
