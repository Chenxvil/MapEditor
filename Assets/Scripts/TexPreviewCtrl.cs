using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class TexPreviewCtrl : MonoBehaviour
{
    public Image imageSprite;
    public Dropdown dropdownMapTexture;

    private readonly string m_rootPath = "Assets/Resources/";
    private readonly string m_spriteRootPath = "MapSprite/";
    private List<string> m_spriteNames = new List<string>(); 

    void Start()
    {
        string spriteDirPath = m_rootPath + m_spriteRootPath;
        if(Directory.Exists(spriteDirPath))
        {
            foreach (string path in Directory.GetFiles(spriteDirPath))
            {
                if(path.EndsWith(".png") || path.EndsWith(".bmp") || path.EndsWith(".jpg"))
                {
                    string[] temp = path.Split('/');
                    string tmpStr = temp[temp.Length - 1].Split('.')[0];
                    m_spriteNames.Add(tmpStr);
                }
            }
            dropdownMapTexture.ClearOptions();
            dropdownMapTexture.AddOptions(m_spriteNames);
        }
        else
        {
            Debug.Log("Can't Find Sprite Dir Path " + spriteDirPath);
        }
        
    }

    public void ChangeSprite(int index)
    {
        string spritePath = m_spriteRootPath + m_spriteNames[index];
        imageSprite.sprite = Resources.Load<Sprite>(spritePath);
    }
}
