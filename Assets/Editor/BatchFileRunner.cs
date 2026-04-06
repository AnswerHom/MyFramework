using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEditor;
using Debug = UnityEngine.Debug;

namespace Editor
{
    /// <summary>
    /// Utility class for running batch files in Unity Editor
    /// </summary>
    public static class BatchFileRunner
    {
        /// <summary>
        /// Run a batch file and capture its output
        /// </summary>
        /// <param name="batFilePath">Full path to the batch file</param>
        /// <param name="errorTag"></param>
        /// <returns>True if execution was successful</returns>
        public static bool Run(string batFilePath, string errorTag = null)
        {
            if (string.IsNullOrEmpty(batFilePath) || !File.Exists(batFilePath))
            {
                EditorUtility.DisplayDialog("Error", "Batch file does not exist!", "OK");
                return false;
            }

            Process process = new Process();
            process.StartInfo.FileName = batFilePath;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;

            // 替换为 UTF-8 编码
            process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
            process.StartInfo.StandardErrorEncoding = Encoding.UTF8;

            process.OutputDataReceived += (sender, args) =>
            {
                if (args == null || string.IsNullOrEmpty(args.Data)) return;
                if (errorTag != null && args.Data.Contains(errorTag)) Debug.LogError(args.Data);
                else Debug.Log(args.Data);
            };
            process.ErrorDataReceived += (sender, args) =>
            {
                if (args == null || string.IsNullOrEmpty(args.Data)) return;
                Debug.Log(args.Data);
            };

            process.Start();

            // Start asynchronous output reading
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            // Wait for the process to exit
            process.WaitForExit();

            return process.ExitCode == 0;
        }
    }
}