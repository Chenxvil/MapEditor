using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class DESEncryptor
{
    private static string m_key = "Map_Edit";

    ///<summary>
    ///使用DES算法进行加密
    ///</summary>
    public static string Encrypt(string data)
    {
        try
        {
            MemoryStream mStream = new MemoryStream();

            DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
            byte[] key = Encoding.UTF8.GetBytes(m_key.Substring(0, 8));
            byte[] iv = key;

            CryptoStream cStream = new CryptoStream(mStream, provider.CreateEncryptor(key, iv), CryptoStreamMode.Write);

            byte[] toEncrypt = Encoding.UTF8.GetBytes(data);

            cStream.Write(toEncrypt, 0, toEncrypt.Length);
            cStream.FlushFinalBlock();

            byte[] result = mStream.ToArray();

            cStream.Close();
            mStream.Close();

            return Convert.ToBase64String(result);
        }
        catch (CryptographicException)
        {
            LogManager.Instance.Log("加密失败！");
            throw;
        }
    }

    ///<summary>
    ///使用DES算法进行解密
    ///</summary>
    public static string Decrypt(string data)
    {
        try
        {
            byte[] tempData = Convert.FromBase64String(data);
            MemoryStream mStream = new MemoryStream(tempData);

            DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
            byte[] key = Encoding.UTF8.GetBytes(m_key.Substring(0, 8));
            byte[] iv = key;

            CryptoStream cStream = new CryptoStream(mStream, provider.CreateDecryptor(key, iv), CryptoStreamMode.Read);

            byte[] fromEncrypt = new byte[tempData.Length];

            cStream.Read(fromEncrypt, 0, fromEncrypt.Length);

            mStream.Close();
            cStream.Close();

            return Encoding.UTF8.GetString(fromEncrypt);
        }
        catch (CryptographicException)
        {
            LogManager.Instance.Log("解密失败！");
            throw;
        }
    }

}
