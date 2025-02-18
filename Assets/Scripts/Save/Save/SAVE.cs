using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class SAVE
{
    // 截图保存路径
    public static string shotPath = $"{Application.persistentDataPath}/Shot";

    static string GetPath(string fileName)
    {
        return Path.Combine(Application.persistentDataPath, fileName);
    }

    #region PlayerPrefs存储
    public static void PlayerPrefsSave(string key, object data)
    {
        // 将对象数据序列化为JSON字符串保存
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(key, json);
        PlayerPrefs.Save();
    }

    public static string PlayerPrefsLoad(string key)
    {
        // 从PlayerPrefs读取字符串，默认值为null
        return PlayerPrefs.GetString(key, null);
    }
    #endregion

    #region JSON存储
    public static void JsonSave(string fileName, object data)
    {
        string json = JsonUtility.ToJson(data);

        File.WriteAllText(GetPath(fileName), json);
        Debug.Log($"保存到 {GetPath(fileName)}");
    }

    public static T JsonLoad<T>(string fileName)
    {
        string path = GetPath(fileName);
        // 如果文件存在则读取
        if (File.Exists(path))
        {
            string json = File.ReadAllText(GetPath(fileName));
            var data = JsonUtility.FromJson<T>(json);
            return data;
        }
        // 对象类型返回null，值类型返回默认值
        return default;
    }

    public static void JsonDelete(string fileName)
    {
        File.Delete(GetPath(fileName));
    }

    public static string FindAuto()
    {
        // 确认路径存在
        if (Directory.Exists(Application.persistentDataPath))
        {
            // 获取所有存档文件
            FileInfo[] fileInfos = new DirectoryInfo(Application.persistentDataPath).GetFiles("*");
            for (int i = 0; i < fileInfos.Length; i++)
            {
                // 查找自动存档文件
                if (fileInfos[i].Name.EndsWith(".auto"))
                {
                    return fileInfos[i].Name;
                }
            }
        }
        return "";
    }
    #endregion

    #region 截图功能
    /* 截取整个屏幕或UI
       可直接指定路径，默认保存到项目文件夹下。
       如果路径已经存在则覆盖。
       使用 ScreenCapture.CaptureScreenshot(path);
    */

    /* 截取指定相机的指定范围*/
    public static void CameraCapture(int i, Camera camera, Rect rect)
    {
        // 如果截图文件夹不存在则创建
        if (!Directory.Exists(SAVE.shotPath))
            Directory.CreateDirectory(SAVE.shotPath);
        string path = Path.Combine(SAVE.shotPath, $"{i}.png");

        int w = (int)rect.width;
        int h = (int)rect.height;

        RenderTexture rt = new RenderTexture(w, h, 0);
        // 将相机的渲染结果保存到指定的RenderTexture
        camera.targetTexture = rt;
        camera.Render();

        // 激活指定的RenderTexture
        RenderTexture.active = rt;

        // 创建Texture2D
        Texture2D t2D = new Texture2D(w, h, TextureFormat.RGB24, true);

        // 将RenderTexture的数据读取到Texture2D中
        t2D.ReadPixels(rect, 0, 0);
        t2D.Apply();

        // 保存为PNG格式
        byte[] bytes = t2D.EncodeToPNG();
        File.WriteAllBytes(path, bytes);

        // 释放资源
        camera.targetTexture = null;
        RenderTexture.active = null;
        GameObject.Destroy(rt);
    }

    public static Sprite LoadShot(int i)
    {
        var path = Path.Combine(shotPath, $"{i}.png");

        Texture2D t = new Texture2D(640, 360);
        t.LoadImage(GetImgByte(path));
        return Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(0.5f, 0.5f));
    }

    static byte[] GetImgByte(string path)
    {
        FileStream s = new FileStream(path, FileMode.Open);
        byte[] imgByte = new byte[s.Length];
        s.Read(imgByte, 0, imgByte.Length);
        s.Close();
        return imgByte;
    }

    public static void DeleteShot(int i)
    {
        var path = Path.Combine(shotPath, $"{i}.png");
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log($"删除截图 {i}");
        }
    }
    #endregion

    #region 清理功能
    #if UNITY_EDITOR
    [UnityEditor.MenuItem("Delete/Records List")]
    public static void DeleteRecord()
    {
        UnityEngine.PlayerPrefs.DeleteAll();
        Debug.Log("已清空存档列表");
    }

    [UnityEditor.MenuItem("Delete/Player Data")]
    public static void DeletePlayerData()
    {
        ClearDirectory(Application.persistentDataPath);
        Debug.Log("已清空玩家数据");
    }

    static void ClearDirectory(string path)
    {
        if (Directory.Exists(path))
        {
            FileInfo[] f = new DirectoryInfo(path).GetFiles("*");
            for (int i = 0; i < f.Length; i++)
            {
                Debug.Log($"删除 {f[i].Name}");
                File.Delete(f[i].FullName);
            }
        }
    }

    [UnityEditor.MenuItem("Delete/Shot")]
    public static void DeleteScreenShot()
    {
        ClearDirectory(shotPath);
        Debug.Log("已清空截图");
    }

    [UnityEditor.MenuItem("Delete/All")]
    public static void DeleteAll()
    {
        DeletePlayerData();
        DeleteRecord();
        DeleteScreenShot();
    }
    #endif
    #endregion
}
