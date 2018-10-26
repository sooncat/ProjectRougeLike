using System;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

public class IOUtils {

    /// <summary>
    /// 从Resources文件夹中读取资源
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="relativePath">相对Resources目录的路径，不带文件扩展名</param>
    /// <returns></returns>
    public static T GetAssetFromResources<T>(string relativePath) where T : Object
    {
        return Resources.Load<T>(relativePath);
    }

    /// <summary>
    /// 从StreamingAssets文件夹中读取，需要全路径及文件扩展名
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static byte[] ReadFileFromStreamingAssets(string path)
    {
        byte[] result = {};
        if(!path.StartsWith(Application.streamingAssetsPath))
        {
            path = Application.streamingAssetsPath + Path.DirectorySeparatorChar + path;    
        }
        
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
                WWW www = new WWW(path);
                while (!www.isDone)
                if (string.IsNullOrEmpty(www.error))
                {
                    result = www.bytes;
                }
                else
                {
                    throw new Exception(www.error);
                }
                www.Dispose();
                break;
            case RuntimePlatform.WindowsPlayer:
            case RuntimePlatform.IPhonePlayer:
            case RuntimePlatform.WindowsEditor:
                result = ReadFileStream(path);
                break;
        }
        return result;
    }
//#if UNITY_EDITOR
//    public static byte[] ReadFile(string path)
//    {
//        return ReadFileStream(path);
//    }
//#endif
    public static byte[] ReadFileFromPersistent(string path)
    {
        if (!path.StartsWith(Application.persistentDataPath))
        {
            path = Application.persistentDataPath + Path.DirectorySeparatorChar + path;
        }
        return ReadFileStream(path);
    }

    public static string Byte2String(byte[] data)
    {
        return System.Text.Encoding.UTF8.GetString(data);
    }

    static byte[] ReadFileStream(string path)
    {
        byte[] b;
        using (Stream file = File.OpenRead(path))
        {
            b = new byte[(int)file.Length];
            file.Read(b, 0, b.Length);
            file.Close();
            file.Dispose();
        }
        return b;
    }

    /// <summary>
    /// 需要全路径和扩展名，覆盖模式
    /// Runtime时只能向persistant文件夹中写
    /// Editor模式下可以向任意文件夹写
    /// </summary>
    /// <param name="content"></param>
    /// <param name="path"></param>
    public static void SaveFile(string content, string path)
    {
        if(!path.StartsWith(Application.persistentDataPath))
        {
            path = Application.persistentDataPath + Path.DirectorySeparatorChar + path;
        }   
        string folder = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(folder) && !Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }
        var utf8WithoutBom = new System.Text.UTF8Encoding(false);

        StreamWriter write = new StreamWriter(path, false, utf8WithoutBom); // Unity's TextAsset.text borks when encoding used is UTF8 :(
        write.Write(content);
		write.Flush();
		write.Close();
		write.Dispose();

        Debug.Log("SaveFile = " + path);
    }

    /// <summary>
    /// 只能删除persistentPath目录下的文件，需要全路径和扩展名
    /// </summary>
    /// <param name="path"></param>
    public static void DelFile(string path)
    {
        if(!path.StartsWith(Application.persistentDataPath))
        {
            path = Application.persistentDataPath + Path.DirectorySeparatorChar + path;
        }
        File.Delete(path);
    }

}
