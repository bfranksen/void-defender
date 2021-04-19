using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class BuildIncrementor : IPreprocessBuildWithReport {
    public int callbackOrder => 1;

    public void OnPreprocessBuild(BuildReport report) {
        // PlayerSettings.macOS.buildNumber = IncrementBuildNumber(PlayerSettings.macOS.buildNumber);
        // PlayerSettings.iOS.buildNumber = IncrementBuildNumber(PlayerSettings.iOS.buildNumber);
        // PlayerSettings.Android.bundleVersionCode++;
        // PlayerSettings.PS4.appVersion = IncrementBuildNumber(PlayerSettings.PS4.appVersion);
        // PlayerSettings.XboxOne.Version = IncrementBuildNumber(PlayerSettings.XboxOne.Version);
        // PlayerSettings.WSA.packageVersion = new System.Version(PlayerSettings.WSA.packageVersion.Major, PlayerSettings.WSA.packageVersion.Minor, PlayerSettings.WSA.packageVersion.Build + 1);

        // BuildScriptableObject buildScriptableObject = ScriptableObject.CreateInstance<BuildScriptableObject>();
        // buildScriptableObject.BuildNumber = PlayerSettings.macOS.buildNumber;

        // AssetDatabase.DeleteAsset("Assets/Resources/Build.asset"); // delete any old build.asset
        // AssetDatabase.CreateAsset(buildScriptableObject, "Assets/Resources/Build.asset"); // create the new one with correct build number before build starts
        // AssetDatabase.SaveAssets();
    }

    private string IncrementBuildNumber(string buildNumber) {
        int.TryParse(buildNumber, out int outputBuildNumber);

        return (outputBuildNumber + 1).ToString();
    }
}