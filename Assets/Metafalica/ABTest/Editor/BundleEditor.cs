using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;

public class BundleEditor
{
    private const string _abConfigName = "abconfig";
    private static string _abFileName = "myAB";
    private static string m_BundleTargetPath = Application.streamingAssetsPath + "/" + _abFileName;
    private static string _xmlPath = Application.dataPath + "/AssetBundleConfig.xml";
    private static string _bytePath = "Assets/Metafalica/res/abCfg/AssetBundleConfig.bytes";

    private static string ABCONFIGPATH = "Assets/Metafalica/ABTest/ABConfig.asset";

    //key为ab包名，value为路径，所有文件夹ab包dic
    private static Dictionary<string, string> m_AllFileDir = new Dictionary<string, string>();

    //过滤list
    private static List<string> m_AllFileAB = new List<string>();

    //单个prefab的ab包
    private static Dictionary<string, List<string>> m_AllPrefabDir = new Dictionary<string, List<string>>();

    //存储所有有效路径
    private static List<string> m_ConfigFil = new List<string>();

    [MenuItem("Tools/打包")]
    public static void Build()
    {
        m_AllFileAB.Clear();
        m_AllFileDir.Clear();
        m_AllPrefabDir.Clear();
        m_ConfigFil.Clear();
        ABConfig _abConfig = AssetDatabase.LoadAssetAtPath<ABConfig>(ABCONFIGPATH);
        foreach (ABConfig.FileDirABName fileDir in _abConfig.m_AllFileDirAB)
        {
            if (m_AllFileDir.ContainsKey(fileDir.ABName))
            {
                Debug.LogError("AB包配置名字重复，请检查");
            }
            else
            {
                m_AllFileDir.Add(fileDir.ABName, fileDir.Path);
                m_AllFileAB.Add(fileDir.Path);
                m_ConfigFil.Add(fileDir.Path);
            }
        }

        string[] allStr = AssetDatabase.FindAssets("t:Prefab", _abConfig.m_AllPrefabPath.ToArray());
        for (int i = 0; i < allStr.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(allStr[i]);
            EditorUtility.DisplayProgressBar("查找Prefab", "Prefab:" + path, i * 1f / allStr.Length);
            m_ConfigFil.Add(path);
            if (!ContainAllFileAB(path))
            {
                GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                string[] allDepend = AssetDatabase.GetDependencies(path);
                List<string> allDependPath = new List<string>();
                for (int j = 0; j < allDepend.Length; j++)
                {
                    if (!ContainAllFileAB(allDepend[j]) && !allDepend[j].EndsWith(".cs"))
                    {
                        m_AllFileAB.Add(allDepend[j]);
                        allDependPath.Add(allDepend[j]);
                    }
                }

                if (m_AllPrefabDir.ContainsKey(obj.name))
                {
                    Debug.LogError("存在相同名字的Prefab!名字：" + obj.name);
                }
                else
                {
                    m_AllPrefabDir.Add(obj.name, allDependPath);
                }
            }
        }

        foreach (string name in m_AllFileDir.Keys)
        {
            SetABName(name, m_AllFileDir[name]);
        }

        foreach (string name in m_AllPrefabDir.Keys)
        {
            SetABName(name, m_AllPrefabDir[name]);
        }

        BuildAssetBundle();

        string[] oldABNames = AssetDatabase.GetAllAssetBundleNames();
        for (int i = 0; i < oldABNames.Length; i++)
        {
            AssetDatabase.RemoveAssetBundleName(oldABNames[i], true);
            EditorUtility.DisplayProgressBar("清除AB包名", "名字：" + oldABNames[i], i * 1f / oldABNames.Length);
        }

        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
    }

    static void SetABName(string name, string path)
    {
        AssetImporter assetImporter = AssetImporter.GetAtPath(path);
        if (assetImporter == null)
        {
            Debug.LogError("不存在此路径文件：" + path);
        }
        else
        {
            assetImporter.assetBundleName = name;
        }
    }

    static void SetABName(string name, List<string> paths)
    {
        for (int i = 0; i < paths.Count; i++)
        {
            SetABName(name, paths[i]);
        }
    }

    static void BuildAssetBundle()
    {
        string[] allBundles = AssetDatabase.GetAllAssetBundleNames();
        //key为全路径,value为包名
        Dictionary<string, string> resPathDic = new Dictionary<string, string>();
        for (int i = 0; i < allBundles.Length; i++)
        {
            string[] allBundlePath = AssetDatabase.GetAssetPathsFromAssetBundle(allBundles[i]);
            for (int j = 0; j < allBundlePath.Length; j++)
            {
                if (allBundlePath[j].EndsWith(".cs"))
                    continue;
                // Debug.Log("此AB包："+allBundles[i]+"下包含的资源文件路径："+allBundlePath[j]);
                if (VaildPath(allBundlePath[j]))
                {
                    resPathDic.Add(allBundlePath[j], allBundles[i]);
                }
            }
        }

        DeleteAB();
        //生成自己的配置表
        WriteData(resPathDic);

        BuildPipeline.BuildAssetBundles(m_BundleTargetPath, BuildAssetBundleOptions.ChunkBasedCompression,
            EditorUserBuildSettings.activeBuildTarget);
    }

    static void WriteData(Dictionary<string, string> resPathDic)
    {
        AssetBundleConfig config = new AssetBundleConfig();
        config.ABList = new List<ABBase>();
        foreach (string path in resPathDic.Keys)
        {
            ABBase abBase = new ABBase();
            abBase.Path = path;
            abBase.Crc = MyCrc32.GetCRC32(path);
            abBase.ABName = resPathDic[path];
            abBase.AssetName = path.Remove(0, path.LastIndexOf("/") + 1);
            abBase.ABDependce = new List<string>();
            string[] resDependce = AssetDatabase.GetDependencies(path);
            for (int i = 0; i < resDependce.Length; i++)
            {
                string tempPath = resDependce[i];
                if (tempPath == path || path.EndsWith(".cs"))
                    continue;
                string abName = "";
                if (resPathDic.TryGetValue(tempPath, out abName))
                {
                    if (abName == resPathDic[path])
                        continue;
                    if (!abBase.ABDependce.Contains(abName))
                    {
                        abBase.ABDependce.Add(abName);
                    }
                }
            }

            config.ABList.Add(abBase);
        }

        //写入xml
        if (File.Exists(_xmlPath)) File.Delete(_xmlPath);
        FileStream fileStream = new FileStream(_xmlPath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
        StreamWriter sw = new StreamWriter(fileStream, System.Text.Encoding.UTF8);
        XmlSerializer xs = new XmlSerializer(config.GetType());
        xs.Serialize(sw, config);
        sw.Close();
        fileStream.Close();
        //写入二进制
        foreach (ABBase abBase in config.ABList)
        {
            abBase.Path = "";
        }

        FileStream fs = new FileStream(_bytePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
        fs.Seek(0, SeekOrigin.Begin);
        fs.SetLength(0);
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(fs, config);
        fs.Close();
        AssetDatabase.Refresh();
        SetABName(_abConfigName, _bytePath);
    }

    /// <summary>
    /// 删除无用AB包
    /// </summary>
    static void DeleteAB()
    {
        string[] allBundlesName = AssetDatabase.GetAllAssetBundleNames();
        DirectoryInfo directory = new DirectoryInfo(m_BundleTargetPath);
        FileInfo[] files = directory.GetFiles("*", SearchOption.AllDirectories);
        for (int i = 0; i < files.Length; i++)
        {
            if (ContainABName(files[i].Name, allBundlesName) || files[i].Name.EndsWith(".meta") ||
                files[i].Name.EndsWith(_abConfigName) ||
                files[i].Name.EndsWith(_abFileName))
            {
                continue;
            }
            else
            {
                //*.manifest不删的话,文件更换位置无法修改ab包内信息
                if (!files[i].Name.EndsWith(".manifest"))
                    Debug.Log("此AB包已经被删除或改名：" + files[i].Name);
                if (File.Exists(files[i].FullName))
                {
                    File.Delete(files[i].FullName);
                    // File.Delete(files[i].FullName + ".manifest");
                    
                    //如果出了奇怪的问题就把下面这行删了
                    if (files[i].Name.EndsWith(".manifest"))
                        File.Delete(files[i].FullName + ".meta");
                }
            }
        }
    }

    /// <summary>
    /// 遍历文件夹里的文件名与设置的所有AB包进行检查判断
    /// </summary>
    /// <param name="name"></param>
    /// <param name="strs"></param>
    /// <returns></returns>
    static bool ContainABName(string name, string[] strs)
    {
        for (int i = 0; i < strs.Length; i++)
        {
            if (name == strs[i])
                return true;
        }

        return false;
    }

    /// <summary>
    /// 是否包含在已经有的AB包里，用来做AB包的冗余剔除
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    static bool ContainAllFileAB(string path)
    {
        for (int i = 0; i < m_AllFileAB.Count; i++)
        {
            if (path == m_AllFileAB[i] || (path.Contains(m_AllFileAB[i]) && path.Replace(m_AllFileAB[i], "")[0] == '/'))
                return true;
        }

        return false;
    }

    /// <summary>
    /// 是否为有效路径
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    static bool VaildPath(string path)
    {
        for (int i = 0; i < m_ConfigFil.Count; i++)
        {
            if (path.Contains(m_ConfigFil[i]))
            {
                return true;
            }
        }

        return false;
    }
}