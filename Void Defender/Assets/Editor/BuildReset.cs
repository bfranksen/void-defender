using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class BuildReset : IPostprocessBuildWithReport {

    public int callbackOrder => 1;

    public void OnPostprocessBuild(BuildReport report) {
        Debug.Log("In Post - Dev Build: " + Debug.isDebugBuild);
        if (!Debug.isDebugBuild) {
            RunPostBuildProcess();
        }

        BuildScriptableObject buildScriptableObject = ScriptableObject.CreateInstance<BuildScriptableObject>();
        buildScriptableObject.BuildNumber = PlayerSettings.Android.bundleVersionCode.ToString();

        AssetDatabase.DeleteAsset("Assets/Resources/Build.asset"); // delete any old build.asset
        AssetDatabase.CreateAsset(buildScriptableObject, "Assets/Resources/Build.asset"); // create the new one with correct build number before build starts
        AssetDatabase.SaveAssets();
    }
    private void RunPostBuildProcess() {
        PlayerSettings.macOS.buildNumber = 0.ToString();
        PlayerSettings.iOS.buildNumber = 0.ToString();
        PlayerSettings.Android.bundleVersionCode = 0;
        // PlayerSettings.PS4.appVersion = 0.ToString();
        // PlayerSettings.XboxOne.Version = 0.ToString();
        // PlayerSettings.WSA.packageVersion = new System.Version(PlayerSettings.WSA.packageVersion.Major, PlayerSettings.WSA.packageVersion.Minor, 0);
    }
}