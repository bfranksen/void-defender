using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class BuildReset : IPostprocessBuildWithReport {

    public int callbackOrder => 1;

    public void OnPostprocessBuild(BuildReport report) {
        RunPostBuildProcess();
    }

    private void RunPostBuildProcess() {
        Debug.Log("post newBuildNumber: " + BuildIncrementor.newBuildNumber);
        PlayerSettings.Android.bundleVersionCode = BuildIncrementor.newBuildNumber;
        BuildScriptableObject buildScriptableObject = ScriptableObject.CreateInstance<BuildScriptableObject>();
        buildScriptableObject.BuildNumber = PlayerSettings.Android.bundleVersionCode.ToString();

        AssetDatabase.DeleteAsset("Assets/Resources/Build.asset"); // delete any old build.asset
        AssetDatabase.CreateAsset(buildScriptableObject, "Assets/Resources/Build.asset"); // create the new one with correct build number before build starts
        AssetDatabase.SaveAssets();
    }
}