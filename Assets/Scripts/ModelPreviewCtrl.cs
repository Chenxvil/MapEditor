using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ModelPreviewCtrl : MonoBehaviour
{
    public Image imageSprite;
    public Dropdown dropdownEntityModel;

    private readonly string m_spriteRootPath = "Assets/Resources/";
    private readonly string m_spriteSceondPath = "ModelSprite/";
    private List<string> m_spriteNames = new List<string>();

    void Start()
    {
        string spriteDirPath = m_spriteRootPath + m_spriteSceondPath;
        if(Directory.Exists(spriteDirPath))
        {
            foreach (string path in Directory.GetFiles(spriteDirPath))
            {
                if (path.EndsWith(".png") || path.EndsWith(".bmp") || path.EndsWith(".jpg"))
                {
                    string[] temp = path.Split('/');
                    string tmpStr = temp[temp.Length - 1].Split('.')[0];
                    m_spriteNames.Add(tmpStr);
                }
            }
            dropdownEntityModel.ClearOptions();
            dropdownEntityModel.AddOptions(m_spriteNames);
        }
        else
        {
            Debug.Log("Can't Find Sprite Dir Path " + spriteDirPath);
        }

    }

    public void ChangeSprite(int index)
    {
        
        string spritePath = m_spriteSceondPath + m_spriteNames[index];
        imageSprite.sprite = Resources.Load<Sprite>(spritePath);
    }
}
