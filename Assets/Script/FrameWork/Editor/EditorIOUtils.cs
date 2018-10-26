using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class EditorIOUtils : Editor {

    /// <summary>
    /// 写文件
    /// </summary>
    /// <param name="content"></param>
    /// <param name="path"></param>
    public static void SaveFile(string content, string path)
    {
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
    }
}
