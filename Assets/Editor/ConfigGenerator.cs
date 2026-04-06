using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class ConfigGenerator
    {
        [MenuItem("Tools/生成数值 %F1")]
        public static void GenerateConfig()
        {
            Clear("Assets/Scripts/Game/Config/Gen/");
            Clear("Assets/Res/Data/Config/");
            string assetsPath = Application.dataPath;
            string batFilePath = Path.Combine(assetsPath, "../../MyFramework-Data/Data/gen.bat");
            EditorUtility.DisplayProgressBar("正在导出","正在导出",0);
            try
            {
                BatchFileRunner.Run(Path.GetFullPath(batFilePath), "|ERROR|");
            }
            catch (Exception e){throw e;}
            finally{EditorUtility.ClearProgressBar();}
            AssetDatabase.Refresh();
        }
        private static void Clear(string path)
        {
            if (Directory.Exists(path))
                Directory.Delete(path, true);
        }
    }
}