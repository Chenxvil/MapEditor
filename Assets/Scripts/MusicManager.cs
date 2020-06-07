using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
 
public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    public Dropdown m_dropdownBGM;
    public Slider m_sliderVolume;
    public AudioSource m_audioSource;

    public Music m_music { get; private set; } = null;

    private readonly string m_rootPath = "Assets/Resources/";
    private readonly string m_musicRootPath = "Music/";

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
        string musicDirPath = m_rootPath + m_musicRootPath;
        List<string> musicNames = new List<string>();
        if (Directory.Exists(musicDirPath))
        {
            foreach (string path in Directory.GetFiles(musicDirPath))
            {
                if (path.EndsWith(".mp3") || path.EndsWith(".wav") || path.EndsWith(".ogg"))
                {
                    string[] temp = path.Split('/');
                    string tmpStr = temp[temp.Length - 1].Split('.')[0];
                    musicNames.Add(tmpStr);
                }
            }
        }
        else
        {
            Debug.Log("Can't Find Music Dir Path " + musicDirPath);
        }
        m_dropdownBGM.ClearOptions();
        m_dropdownBGM.AddOptions(musicNames);

        m_sliderVolume.onValueChanged.AddListener(delegate { SetVolume(m_sliderVolume.value); });
    }
    
    public void Edit()
    {
        if(m_music!=null)
        {
            string name = m_music.m_name;
            int value = int.Parse(name.Substring(3));
            m_dropdownBGM.value = value - 1;
            m_sliderVolume.value = m_music.m_volume;
        }
    }

    public void SetMusic()
    {
        m_music = new Music();
        string name = m_dropdownBGM.captionText.text;
        m_music.m_name = name;

        float volume = m_sliderVolume.value;
        m_music.m_volume = volume;
        StopPlay();
        LogManager.Instance.Log("设置音乐成功!");
    }

    public void SetMusic(string name, float volume)
    {
        m_music = new Music();
        m_music.m_name = name;
        m_music.m_volume = volume;
        LogManager.Instance.Log("设置音乐成功!");
    }

    public void PlayMusic()
    {
        StopPlay();
        string path = m_musicRootPath + m_dropdownBGM.captionText.text;
        AudioClip audioClip = Resources.Load<AudioClip>(path);
        if(audioClip != null)
        {
            m_audioSource.clip = audioClip;
        }
   
        SetVolume(m_sliderVolume.value);
        m_audioSource.Play();
    }

    public void StopPlay()
    {
        if (m_audioSource.isPlaying)
        {
            m_audioSource.Stop();
        }
    }

    public void SetVolume(float value)
    {
        if(value>=0 && value <=1)
        {
            m_audioSource.volume = value;
        }   
    }

    public void Clear()
    {
        m_music = null;
        LogManager.Instance.Log("清除所有音乐信息成功!");
    }
}
