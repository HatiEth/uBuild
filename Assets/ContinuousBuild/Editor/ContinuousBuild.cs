using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class ContinuousBuild : EditorWindow {

    static string Title = "ContinuousBuild";
    List<ContinuousBuildSetting> settings;
    string BuildRootLocation = "Builds/";


    [MenuItem("ContinuousBuild/Settings")]
    static void CreateWindow()
    {
        ContinuousBuild cb = (ContinuousBuild)EditorWindow.GetWindow(typeof(ContinuousBuild));
        cb.titleContent.text = Title;
        cb.titleContent.tooltip = Title;
        

        cb.Show();
    }

    public void OnEnable()
    {
        settings = new List<ContinuousBuildSetting>();
        settings.Add(new ContinuousBuildSetting());
    }

    public void OnGUI()
    {
        EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));

        EditorGUILayout.BeginHorizontal();
        BuildRootLocation = EditorGUILayout.TextField("Build root location", BuildRootLocation);
        if(GUILayout.Button("...", GUILayout.MaxWidth(32f), GUILayout.MinWidth(32f)))
        {
            BuildRootLocation = EditorUtility.OpenFolderPanel("Locate your build root directory", "", BuildRootLocation);
        }
        EditorGUILayout.EndHorizontal();

        for (int i = 0; i < settings.Count;++i)
        {
            settings[i].OnGUI();
        }

        GUI.enabled = HasActiveScenes();
        bool BuildBtnPressed = GUILayout.Button(GUI.enabled ? "Build" : "Build (Disabled: No enabled scenes!)");
        GUI.enabled = true;

        EditorGUILayout.EndVertical();

        if(BuildBtnPressed)
        {
            OnBuild();
        }
    }

    public void OnBuild()
    {
        // Build Cmd
        EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
        string[] SceneStr = new string[scenes.Length];
        for (int i = 0; i < scenes.Length;++i)
        {
            if(scenes[i].enabled)
            {
                SceneStr[i] = scenes[i].path;
            }
        }

        for(int i=0;i<settings.Count;++i)
        {
            string err = settings[i].Build(BuildRootLocation, SceneStr);
            if(err != "")
            {
                Debug.LogError(err);
            }
        }
    }


    private bool HasActiveScenes()
    {
        EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
        for (int i = 0; i < scenes.Length;++i)
        {
            if(scenes[i].enabled)
            {
                return true;
            }
        }
        return false;
    }
}
