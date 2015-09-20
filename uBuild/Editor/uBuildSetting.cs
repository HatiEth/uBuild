using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class uBuildSetting {
    public BuildTarget target = BuildTarget.StandaloneWindows64;
    public BuildOptions options = BuildOptions.None;

    public bool IsFoldOut = true;
    public string BuildName = "Build";
    public  bool isEnabled = true;

    public bool IsEnabled
    {
        get { return isEnabled; }
    }

    public bool HasCustomExtension;
    public string Extension;

    List<string> Errors = new List<string>();
    Vector2 errorScroll = new Vector2();

    public void DrawGUI(uBuildWindow window)
    {
        EditorGUILayout.BeginHorizontal();
        IsFoldOut = EditorGUILayout.Foldout( IsFoldOut, BuildName + " (" + target.ToString() + ')');
        GUILayout.FlexibleSpace();
        if(Errors.Count > 0)
        {
            GUILayout.Label(window.errBtnContent);
        }
        if(GUILayout.Button(window.rmBtnContent))
        {
            window.RemoveBuildSetting(this);
        }
        EditorGUILayout.EndHorizontal();

        if(IsFoldOut)
        {
            isEnabled = EditorGUILayout.Toggle("Enabled?", isEnabled);
            GUI.enabled = isEnabled;
            BuildName = EditorGUILayout.TextField("BuildName: ", BuildName);

            BuildTarget newtarget = (BuildTarget)EditorGUILayout.EnumPopup("Target: ", target);
            if(newtarget != target)
            {
                Extension = Extension == DeterminateExtension(target) ? DeterminateExtension(newtarget) : Extension;
                target = newtarget;
            }
            options = (BuildOptions)EditorGUILayout.EnumMaskField("Options: ", options);

            HasCustomExtension = EditorGUILayout.ToggleLeft("Custom Extension?", HasCustomExtension);

            if(HasCustomExtension)
            {
                EditorGUILayout.BeginHorizontal();
                Extension = EditorGUILayout.TextField("Custom Extension: ", Extension ?? DeterminateExtension(target));
                EditorGUILayout.EndHorizontal();
            }
            GUI.enabled = true;

            if(Errors.Count > 0)
            {
                //EditorGUILayout.BeginScrollView(errorScroll, false, false);
                GUILayout.Space(10);
                EditorGUILayout.BeginHorizontal();
                for (int i = 0; i < Errors.Count;++i)
                {

                    Vector2 h = GUI.skin.button.CalcSize(new GUIContent(Errors[i]));
                    EditorGUILayout.SelectableLabel(Errors[i], GUILayout.Height(h.y), GUILayout.Width(h.x));
                }
                EditorGUILayout.EndHorizontal();
                //EditorGUILayout.EndScrollView();
            }
        }
    }

    public string Build(string root, string[] SceneStr)
    {
        string Path = root + System.IO.Path.DirectorySeparatorChar + BuildName + "-" + target.ToString() + System.IO.Path.DirectorySeparatorChar ;
        if(!System.IO.Directory.Exists(Path))
        {
            System.IO.Directory.CreateDirectory(Path);
        }

        string AppFile = Application.productName + (HasCustomExtension ? Extension : DeterminateExtension(target));
        return BuildPipeline.BuildPlayer(SceneStr, Path+AppFile, target, options);
    }

    private static string DeterminateExtension(BuildTarget target)
    {
        switch(target)
        {
            case BuildTarget.Android:
                return ".apk";
            case BuildTarget.StandaloneWindows:
                return ".exe";
            case BuildTarget.StandaloneWindows64:
                return ".exe";
            default:
                return "";
        }
    }

    public void ClearErrors()
    {
        Errors.Clear();
    }

    public void PushError(string err)
    {
        Errors.Add(err);
    }
}
