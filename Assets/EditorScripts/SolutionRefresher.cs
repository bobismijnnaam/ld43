// Simple Editor Script that lets you create / save quick notes
// Between Unity Sessions.
// Made by: Bob Rubbens
// http://www.plusminos.nl

using System.IO;
using UnityEngine;

#if UNITY_EDITOR 
using UnityEditor;

using UED = UnityEngine.Debug;

public class SolutionRefresher : EditorWindow
{

    public static string DEFAULT_APP_KEY = "kScriptsDefaultApp";
    public static string MONODEVELOP_PATH = "/usr/bin/monodevelop";

    [MenuItem("Extensions/Solution Refresher")]
    static void Init()
    {
        var window = (SolutionRefresher)EditorWindow.GetWindowWithRect(typeof(SolutionRefresher), new Rect(0, 0, 100, 100));
        window.Show();
    }

    void OnGUI()
    {
        if (GUILayout.Button("Print current editor")) UED.Log(EditorPrefs.GetString(DEFAULT_APP_KEY));
        if (GUILayout.Button("Set to monodevelop")) {
            EditorPrefs.SetString(DEFAULT_APP_KEY, MONODEVELOP_PATH);
        }
        if (GUILayout.Button("Set to gvim_wrapper.sh")) {
            EditorPrefs.SetString(DEFAULT_APP_KEY, "/home/bobe/Dropbox/Unity/gvim_wrapper.sh");
        }
        if (GUILayout.Button("Refresh C# Solution")) {
            UED.Log("Saving previous editor...");

            var previousEditor = EditorPrefs.GetString(DEFAULT_APP_KEY);

            UED.Log("Is: " + previousEditor);
            UED.Log("Setting editor: " + MONODEVELOP_PATH);

            EditorPrefs.SetString(DEFAULT_APP_KEY, MONODEVELOP_PATH);

            UED.Log("Starting \"Open C# Project\"...");
            EditorApplication.ExecuteMenuItem("Assets/Open C# Project");
            UED.Log("Started.");
            UED.Log("Setting editor to: " + previousEditor);
            EditorPrefs.SetString(DEFAULT_APP_KEY, previousEditor);
            UED.Log("All done!");
        }
    }

    public static string FindScriptRecursively(string basePath) {
        UED.Log("Looking in folder: " + basePath);
        var files = Directory.GetFiles(basePath);
        foreach (var file in files) {
            UED.Log(file);
            if (file.EndsWith(".cs")) {
                return file;
            }
        }
        foreach (var dir in Directory.GetDirectories(basePath)) {
            var candidate = FindScriptRecursively(dir);
            if (candidate != null) {
                return candidate;
            }
        }
        UED.Log("Ok");
        return null;
    }

    public static string GetDataPath()
    {
        //Get the path of the Game data folder
        string m_Path = Application.dataPath;

        //Output the Game data path to the console
        return m_Path;
    }
}
#endif
