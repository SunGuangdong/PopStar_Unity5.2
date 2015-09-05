
using System.IO;
using System.Text;
using UnityEngine;
using JsonFx.Json;
using System.Text.RegularExpressions;

/// <summary>
/// 	JSON读取操作
/// 孙广东
/// </summary>
public class JsonReadFileTool
{

    /// <summary>
    /// 根据一个JSON内容，得到一个结构
    /// </summary>
    static public T JsonToClass<T>(string json) where T : class
    {
        T t = JsonReader.Deserialize<T>(json);
        Debug.Log("实际上:"+json);
        return t;
    }

    /// <summary>
    /// 根据一个JSON的文件地址，得到一个结构
    /// 传入如："Assets/Textures/texture.jpg"
    /// </summary>
    //static public T AddressToClass<T>(string txtAddress) where T : class
    //{
    //    //TextAsset jsonData = Resources.Load(txtAddress) as TextAsset;     // 不一定在Resources下  
    //    TextAsset jsonData = AssetDatabase.LoadAssetAtPath(txtAddress, typeof(TextAsset)) as TextAsset;
    //    return JsonToClass<T>(jsonData.text);
    //}

    /// <summary>
    /// 根据一个JSON的文件地址，得到一个结构
    /// 传入如："Assets/Textures/texture.jpg"
    /// </summary>
    static public T DataPathToClass<T>(string txtAddress) where T : class
    {
        string contentText = File.ReadAllText(txtAddress, Encoding.UTF8);     // 不一定在Resources下  

        return JsonToClass<T>(contentText);
    }

    /// <summary>
    /// 将JSON转换为一个结构数组
    /// </summary>
    static public T JsonToClasses<T>(string json) where T : class
    {
        //Debug.Log(json);
        T list = JsonReader.Deserialize<T>(json);
        return list;
    }

    /// <summary>
    /// 给Json文件的地址。转换为一个结构数组
    /// </summary>
    static public T AddressToClasses<T>(string txtAddress) where T : class
    {
        TextAsset jsonData = Resources.Load(txtAddress) as TextAsset;
        return JsonToClasses<T>(jsonData.text);
    }

    /// <summary>
    /// Addresses to classes.
    /// </summary>
    static public string ClassesToString<T>(T txtContent) where T : class
    {
        //		TextAsset jsonData = Resources.Load(txtContent) as TextAsset; 
        StringBuilder sb = new StringBuilder();
        JsonWriterSettings settings = new JsonWriterSettings();
        settings.PrettyPrint = true;
        using (JsonWriter writer = new JsonWriter(sb, settings))
        {
            writer.Write(txtContent);
        }
        return Regex.Unescape(sb.ToString());
    }
}
