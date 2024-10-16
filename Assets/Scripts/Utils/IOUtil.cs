using LitJson;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace MiniGames.Utils
{
    class IOUtil
    {
        public static string ANDROID_SA_PATH_TPL = "jar:file://" + Application.dataPath + "!/assets/{0}";

        /// <summary>
        /// 将对象序列化为 JSON 并保存到 StreamingAssets 目录
        /// </summary>
        /// <typeparam name="T">对象的类型</typeparam>
        /// <param name="data">要序列化的对象</param>
        /// <param name="fileName">保存的文件名</param>
        public static void ExportJsonToSA<T>(T data, string fileName)
        {
            // 序列化为 JSON 字符串
            string jsonData = JsonMapper.ToJson(data);

            // 保存路径
            string fullPath = Path.Combine(Application.streamingAssetsPath, fileName);

            // 调用导出文件的方法
            ExportFile(fullPath, jsonData);
        }

        /// <summary>
        /// 从 StreamingAssets 目录加载 JSON 文件并反序列化为对象
        /// </summary>
        /// <typeparam name="T">对象的类型</typeparam>
        /// <param name="fileName">文件名</param>
        /// <returns>反序列化后的对象</returns>
        public static T LoadJsonFromSA<T>(string fileName)
        {
            string fullPath = "";
            if (Application.platform == RuntimePlatform.Android)
            {
                // 在 Android 上使用 UnityWebRequest 获取文件内容
                fullPath = string.Format(ANDROID_SA_PATH_TPL, fileName);
            }
            else
            {
                // 其他平台使用标准的文件 I/O
                fullPath = Path.Combine(Application.streamingAssetsPath, fileName);
            }

            // 加载文件内容
            string jsonData = LoadFile(fullPath);
            return JsonMapper.ToObject<T>(jsonData);
        }

        /// <summary>
        /// 将数据导出到指定路径
        /// </summary>
        /// <param name="fullPath">保存的完整路径</param>
        /// <param name="data">要保存的数据</param>
        public static void ExportFile(string fullPath, string data)
        {
            try
            {
                // 确保目录存在
                string directory = Path.GetDirectoryName(fullPath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // 将数据写入文件
                File.WriteAllText(fullPath, data);
                Debug.Log($"File exported to {fullPath}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to export file to {fullPath}: {ex.Message}");
            }
        }

        /// <summary>
        /// 加载文件的内容
        /// </summary>
        /// <param name="fullPath">文件路径</param>
        /// <returns>文件的内容</returns>
        public static string LoadFile(string fullPath)
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                // 使用 UnityWebRequest 加载 Android 上的文件
                UnityWebRequest www = UnityWebRequest.Get(fullPath);
                var operation = www.SendWebRequest();

                while (!operation.isDone)
                {
                    // 等待请求完成
                }

                if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError($"Failed to load file from {fullPath}: {www.error}");
                    return null;
                }
                else
                {
                    return www.downloadHandler.text;
                }
            }
            else
            {
                // 使用标准的文件读取方式
                return File.ReadAllText(fullPath);
            }
        }
    }
}
