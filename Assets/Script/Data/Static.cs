using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using System.IO;
using System;


public class CachingRegion
{

    //储存区全局变量
    static Dic<string, object> CachingField = new Dic<string, object>();

    static CachingRegion()
    {
        Add("huiyuan_id", "B52740810ptxz2018a04efa5ff9017fc25a81a140c0f369231");
    }

    public static string Get(string key)
    {
        bool Result = false;
        object obj = CachingField.Get(key, out Result);
        if (!Result)
        {
            Debug.LogError("查找的字段不在缓存中:" + key);
            return null;
        }
        return obj.ToString();
    }

    public static T Get<T>(string key)
    {
        bool Result = false;
        object obj = CachingField.Get(key, out Result);
        if (!Result)
        {
            Debug.LogError("查找的字段不在缓存中:" + key);
        }
        return (T)obj;
    }

    public static void Add(string key, object obj)
    {
        CachingField.Add(key, obj);
    }
}

public class Static
{
    private static Static instance;
    public static Static Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new Static();
                instance.Init();
            }
            return instance;
        }
    }


    //初始化程序集
    public void Init()
    {

    }


    public bool MusicSwich = false;

    public bool MusicSwichButton = true;

    public string URL = "http://192.168.89.89/YZC/";//"http://www.maidada.vip/";

    public string LocalURL = "http://47.94.99.18:19011/";//"http://192.168.88.60:19003/";

    public string SaveName = "GYKY";

    public void CreateFile(string path, string name, string info)
    {
        //文件流信息
        StreamWriter sw;
        FileInfo t = new FileInfo(path + "//" + SaveName + name);
        if (!t.Exists)
        {
            //如果此文件不存在则创建
            sw = t.CreateText();
        }
        else
        {
            //如果此文件存在则打开
            sw = t.AppendText();
        }
        //以行的形式写入信息
        sw.WriteLine(info);
        //关闭流
        sw.Close();
        //销毁流
        sw.Dispose();
    }

    /**
     * path：读取文件的路径
     * name：读取文件的名称
     */
    public ArrayList LoadFile(string path, string name)
    {
        //使用流的形式读取
        StreamReader sr = null;
        try
        {
            sr = File.OpenText(path + "//" + SaveName + name);
        }
        catch (Exception e)
        {
            //路径与名称未找到文件则直接返回空
            return null;
        }
        string line;
        ArrayList arrlist = new ArrayList();
        while ((line = sr.ReadLine()) != null)
        {
            //一行一行的读取
            //将每一行的内容存入数组链表容器中
            arrlist.Add(line);
        }
        //关闭流
        sr.Close();
        //销毁流
        sr.Dispose();
        //将数组链表容器返回
        return arrlist;
    }

    public string GetLoadFile(string name)
    {
        ArrayList infoall = Static.Instance.LoadFile(Application.persistentDataPath, name);
        String sr = null;
        if (infoall == null)
            return null;
        foreach (string str in infoall)
        {
            sr += str;
        }
        return sr;
    }

    /**
     * path：删除文件的路径
     * name：删除文件的名称
     */
    public void DeleteFile(string path, string name)
    {
        File.Delete(path + "//" + SaveName + name);
    }

    public class aciton<T>
    {
        public delegate void ACTION(T OBJ);
        public ACTION EVENT;
    }
}
