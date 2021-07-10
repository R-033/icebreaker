using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
 
public class WebglPostBuild
{
    [PostProcessBuild(1)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        if (target != BuildTarget.WebGL)
            return;
 
        Debug.Log(pathToBuiltProject);
 
        string[] filePaths = Directory.GetFiles(pathToBuiltProject, "*.js", SearchOption.AllDirectories);
 
        foreach(string file in filePaths)
        {
            if(file.ToLower().Contains("loader.js"))
            {
                string text = File.ReadAllText(file);
                text = text.Replace(@"Mac OS X (10[\.\_\d]+)", @"Mac OS X (1[\.\_\d][\.\_\d]+)");
                File.WriteAllText(file, text);
            }
        }
    }
}