using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class DataManager
{
    static public string Path = @"Setting\";

    static private void SetJsonData(string section, string data, string key = "Data")
    {
        if (string.IsNullOrEmpty(data))
        {
            Debug.LogWarning("" + section + " 데이터 없음. 저장 X");
            return;
        }

        if (Directory.Exists(Path) == false)
        {
            Directory.CreateDirectory(Path);
        }

        var backPath = @"Setting\Backup\" + System.DateTime.Now.ToString("yyMMdd_HHmm") + @"\";
        if (Directory.Exists(backPath) == false)
        {
            Directory.CreateDirectory(backPath);
        }



        string fileName = Path + section + "_" + key + ".txt";
        string back_fileName = backPath + section + "_" + key + ".txt";
        Debug.Log("[Save] " + fileName + " / " + data);





        //PlayerPrefs.SetString(section + "_" + key, data);

        using (StreamWriter outputFile = new StreamWriter(fileName, false))
        {
            outputFile.WriteLine(data);
        }
        using (StreamWriter outputFile = new StreamWriter(back_fileName, false))
        {
            outputFile.WriteLine(data);
        }
    }

    static private string GetJsonData(string section, string defaultData, string key = "Data")
    {
        string file = Path + section + "_" + key + ".txt";
        if (File.Exists(file))
        {
            return File.ReadAllText(file);
        }
        else
        {
            return defaultData;
        }
    }


    /// <summary>
    /// [System.Serializable] 사용된 데이터 클래스 사용
    /// </summary>
    static public void SetData<T>(string key, T data)
    {
        SetJsonData(key, JsonUtility.ToJson(data, true));
    }


    /// <summary>
    /// [System.Serializable] 사용된 데이터 클래스 사용
    /// </summary>
    static public T GetData<T>(string key)
    {
        var str = GetJsonData(key, "");
        if (string.IsNullOrEmpty(str))
        {
            return default(T);
        }
        try
        {
            T data = JsonUtility.FromJson<T>(str);
            return data;
        }
        catch (System.Exception e)
        {
            Debug.LogError("[Get Data Err] " + key + " / " + e.Message);
            return default(T);
        }
    }




    /// <summary>
    /// [System.Serializable] 사용된 데이터 클래스 사용
    /// </summary>
    static public void SetDataSecurity<T>(string key, T data)
    {
        SetJsonData(key, SecurityPlayerPrefs.Encrypt(JsonUtility.ToJson(data, true)));
    }

    /// <summary>
    /// [System.Serializable] 사용된 데이터 클래스 사용
    /// </summary>
    static public T GetDataSecurity<T>(string key)
    {
        var str = GetJsonData(key, "");
        if (string.IsNullOrEmpty(str))
        {
            return default(T);
        }
        try
        {
            T data = JsonUtility.FromJson<T>(SecurityPlayerPrefs.Decrypt(str));
            return data;
        }
        catch (System.Exception e)
        {
            Debug.LogError("[Get Data Err] " + key + " / " + e.Message);
            return default(T);
        }
    }
}







public class SecurityPlayerPrefs
{
    // http://www.codeproject.com/Articles/769741/Csharp-AES-bits-Encryption-Library-with-Salt
    // http://ikpil.com/1342

    private static string _saltForKey;

    private static byte[] _keys;
    private static byte[] _iv;
    private static int keySize = 256;
    private static int blockSize = 128;
    private static int _hashLen = 32;

    static SecurityPlayerPrefs()
    {
        // 8 바이트로 하고, 변경해서 쓸것
        byte[] saltBytes = new byte[] { 36, 36, 8, 51, 22, 14, 75, 12 };

        // 길이 상관 없고, 키를 만들기 위한 용도로 씀
        string randomSeedForKey = "5h34w5g4326hrtdgh45ktyhe5c";

        // 길이 상관 없고, aes에 쓸 key 와 iv 를 만들 용도
        string randomSeedForValue = "2345j7rt63wj645wf54w3d4";

        {
            Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(randomSeedForKey, saltBytes, 1000);
            _saltForKey = System.Convert.ToBase64String(key.GetBytes(blockSize / 8));
        }

        {
            Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(randomSeedForValue, saltBytes, 1000);
            _keys = key.GetBytes(keySize / 8);
            _iv = key.GetBytes(blockSize / 8);
        }
    }

    public static string MakeHash(string original)
    {
        using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(original);
            byte[] hashBytes = md5.ComputeHash(bytes);

            string hashToString = "";
            for (int i = 0; i < hashBytes.Length; ++i)
                hashToString += hashBytes[i].ToString("x2");

            return hashToString;
        }
    }

    public static byte[] Encrypt(byte[] bytesToBeEncrypted)
    {
        using (RijndaelManaged aes = new RijndaelManaged())
        {
            aes.KeySize = keySize;
            aes.BlockSize = blockSize;

            aes.Key = _keys;
            aes.IV = _iv;

            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using (ICryptoTransform ct = aes.CreateEncryptor())
            {
                return ct.TransformFinalBlock(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
            }
        }
    }

    public static byte[] Decrypt(byte[] bytesToBeDecrypted)
    {
        using (RijndaelManaged aes = new RijndaelManaged())
        {
            aes.KeySize = keySize;
            aes.BlockSize = blockSize;

            aes.Key = _keys;
            aes.IV = _iv;

            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using (ICryptoTransform ct = aes.CreateDecryptor())
            {
                return ct.TransformFinalBlock(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
            }
        }
    }

    public static string Encrypt(string input)
    {
        byte[] bytesToBeEncrypted = Encoding.UTF8.GetBytes(input);
        byte[] bytesEncrypted = Encrypt(bytesToBeEncrypted);

        return System.Convert.ToBase64String(bytesEncrypted);
    }

    public static string Decrypt(string input)
    {
        byte[] bytesToBeDecrypted = System.Convert.FromBase64String(input);
        byte[] bytesDecrypted = Decrypt(bytesToBeDecrypted);

        return Encoding.UTF8.GetString(bytesDecrypted);
    }
}