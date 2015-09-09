using UnityEngine;
using UnityEditor;
using System.Collections;

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


    public void DrawGUI(uBuildWindow window)
    {
        EditorGUILayout.BeginHorizontal();
        IsFoldOut = EditorGUILayout.Foldout( IsFoldOut, BuildName + " (" + target.ToString() + ')');
        GUILayout.FlexibleSpace();
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
        }
    }

    public string Build(string root, string[] SceneStr)
    {
        return BuildPipeline.BuildPlayer(SceneStr, root + System.IO.Path.DirectorySeparatorChar + BuildName + "-" + target.ToString() + System.IO.Path.DirectorySeparatorChar + Application.productName + (HasCustomExtension ? Extension : DeterminateExtension(target)), target, options);
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
}
