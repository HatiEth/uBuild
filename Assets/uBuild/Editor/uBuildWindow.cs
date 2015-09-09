using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class uBuildWindow : EditorWindow {

    static string Title = "uBuild";
    uBuildSettings db;
    string BuildRootLocation = "Builds/";
    Vector2 scroll = new Vector2();


    [MenuItem("uBuild/Settings")]
    static void CreateWindow()
    {
        uBuildWindow cb = (uBuildWindow)EditorWindow.GetWindow(typeof(uBuildWindow));
        cb.titleContent.text = Title;
        cb.titleContent.tooltip = Title;

        cb.Show();
    }
    GUIContent addBtnContent;
    public GUIContent rmBtnContent;
    public void OnEnable()
    {

        addBtnContent = new GUIContent();
        //addBtnContent.text = "Add Build";
        addBtnContent.image = Resources.Load("uBuild/Images/addBtn") as Texture2D;

        rmBtnContent = new GUIContent();
        //rmBtnContent.text = "Remove";
        rmBtnContent.image = Resources.Load("uBuild/Images/rmBtn") as Texture2D;
    }

    public void OnGUI()
    {
        if(AssetDatabase.LoadAssetAtPath<uBuildSettings>("Assets/uBuildSettings.asset") == null)
        {
            CreateBuildSettingsFile();
        }
        db = AssetDatabase.LoadAssetAtPath<uBuildSettings>("Assets/uBuildSettings.asset");

        EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));

        EditorGUILayout.BeginHorizontal();
        BuildRootLocation = EditorGUILayout.TextField("Build root location", BuildRootLocation);
        if(GUILayout.Button("...", GUILayout.MaxWidth(32f), GUILayout.MinWidth(32f)))
        {
            BuildRootLocation = EditorUtility.OpenFolderPanel("Locate your build root directory", "", BuildRootLocation);
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();

        GUILayout.FlexibleSpace();

        if(GUILayout.Button(addBtnContent))
        {
            db.settings.Add(new uBuildSetting());
            AssetDatabase.SaveAssets();
        }
        EditorGUILayout.EndHorizontal();

        scroll = EditorGUILayout.BeginScrollView(scroll, false, false);
        GUILayout.Space(10);


        for (int i = 0; i < db.settings.Count;++i)
        {
            db.settings[i].DrawGUI(this);
        }

        EditorGUILayout.EndScrollView();

        GUI.enabled = HasActiveScenes() && HasActiveBuilds();
        string ButtonText = !HasActiveScenes() ? " No enabled scenes" : "";
        ButtonText += !HasActiveBuilds() ? HasActiveScenes() ? " No enabled builds" : " and no enabled builds" : "";
        bool BuildBtnPressed = GUILayout.Button(GUI.enabled ? "Build" : "Build (Disabled:"+ButtonText+"!)");
        GUI.enabled = true;

        EditorGUILayout.EndVertical();
        EditorUtility.SetDirty(db);

        if(BuildBtnPressed)
        {
            BuildCommand();
        }

        /*
        */
    }

    public void BuildCommand()
    {
        EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
        string[] SceneStr = new string[scenes.Length];
        for (int i = 0; i < scenes.Length;++i)
        {
            if(scenes[i].enabled)
            {
                SceneStr[i] = scenes[i].path;
            }
        }

        for(int i=0;i<db.settings.Count;++i)
        {
            if(db.settings[i].IsEnabled)
            {
                string err = db.settings[i].Build(BuildRootLocation, SceneStr);
                if(err != "")
                {
                    Debug.LogError("uBuild Error: "+err);
                }
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

    private bool HasActiveBuilds()
    {
        for (int i = 0; i < db.settings.Count; ++i)
        {
            if (db.settings[i].IsEnabled)
            {
                return true;
            }
        }
        return false;
    }


    private static void CreateBuildSettingsFile()
    {
        uBuildSettings db = ScriptableObject.CreateInstance<uBuildSettings>();
        string dbPath = AssetDatabase.GenerateUniqueAssetPath("Assets/uBuildSettings.asset");

        AssetDatabase.CreateAsset(db, dbPath);
        AssetDatabase.SaveAssets();
    }

    public void RemoveBuildSetting(uBuildSetting setting)
    {
        db.settings.Remove(setting);
        AssetDatabase.SaveAssets();
    }
}
