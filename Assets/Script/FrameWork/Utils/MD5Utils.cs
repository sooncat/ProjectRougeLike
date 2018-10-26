using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Security.Cryptography;

public class MD5Utils {

    /// <summary>
    /// 计算字符串的MD5值
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string GetStringMd5(string source)
    {
        MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
        byte[] data = Encoding.UTF8.GetBytes(source);
        byte[] md5Data = md5.ComputeHash(data, 0, data.Length);
        md5.Clear();

        string result = BitConverter.ToString(md5Data);//.Replace("-","");
        return result;   
    }

    /// <summary>
    /// 计算文件的MD5值
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static string GetFileMd5(string filePath)
    {
        FileStream fs = new FileStream(filePath, FileMode.Open);
        return GetFileMd5(fs);   
    }

    /// <summary>
    /// 计算文件的MD5值
    /// </summary>
    /// <param name="fs"></param>
    /// <returns></returns>
    public static string GetFileMd5(FileStream fs)
    {
        MD5 md5 = new MD5CryptoServiceProvider();
        byte[] retVal = md5.ComputeHash(fs);
        fs.Close();
        string result = BitConverter.ToString(retVal);//.Replace("-","");
        return result;
    }

}
