using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

namespace Metafalica.RPG
{
    #region JSON读写
    public class MyJsonUtils
    {
        public static void WriteToJson<T>(string path, T data)
        {
            string str_write = JsonConvert.SerializeObject(data, Formatting.Indented);
            FileInfo fileWrite = new FileInfo(path);
            StreamWriter sw = fileWrite.CreateText();
            sw.WriteLine(str_write);
            sw.Close();
        }
        
        public static void ReadFromJson<T>(string path , out T data)
        {
            FileStream fs = new FileStream(path, FileMode.Open);
            StreamReader fileStream = new StreamReader(fs);
            string str = fileStream.ReadToEnd();
            data = JsonConvert.DeserializeObject<T>(str);
            fs.Close();
            fileStream.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="textAsset">AB包中加载到的Json文件</param>
        /// <param name="data"></param>
        /// <typeparam name="T"></typeparam>
        public static void ReadFromJson<T>(TextAsset textAsset ,out T data)
        {
            MemoryStream stream = new MemoryStream(textAsset.bytes);
            StreamReader fileStream = new StreamReader(stream);
            string str = fileStream.ReadToEnd();
            data = JsonConvert.DeserializeObject<T>(str);
            stream.Close();
            fileStream.Close();
        }
        
    }
    #endregion
    
}
