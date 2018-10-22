﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class IOUtils {

    // 读取streamassets目录中的文件
    public static byte[] ReadFile(string path)
    {
        byte[] b = null;

        if (Application.platform == RuntimePlatform.Android && path.Contains(Application.streamingAssetsPath))
        {
            WWW www = new WWW(path);
            while (!www.isDone)
            if (string.IsNullOrEmpty(www.error))
            {
                b = www.bytes;
            }
            www.Dispose();
        }
        else
        {
            b = ReadFileStream(path);
        }

        return b;
    }

    public static string ReadFileString(string path)
    {
        byte[] data = ReadFile(path);
        return System.Text.Encoding.UTF8.GetString(data);
    }

    public static byte[] ReadFileStream(string path)
    {
        byte[] b = null;
        using (Stream file = File.OpenRead(path))
        {
            b = new byte[(int)file.Length];
            file.Read(b, 0, b.Length);
            file.Close();
            file.Dispose();
        }
        return b;
    }

    public static void SaveFile(string content, string path)
    {
        string folder = Path.GetDirectoryName(path);
		Directory.CreateDirectory(folder);


        var utf8WithoutBom = new System.Text.UTF8Encoding(false);

        StreamWriter write = new StreamWriter(path, false, utf8WithoutBom); // Unity's TextAsset.text borks when encoding used is UTF8 :(
        write.Write(content);
		write.Flush();
		write.Close();
		write.Dispose();
    }
}