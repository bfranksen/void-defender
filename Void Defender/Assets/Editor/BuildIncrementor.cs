using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class BuildIncrementor : IPreprocessBuildWithReport {

    public int callbackOrder => 1;

    public static int newBuildNumber;

    public void OnPreprocessBuild(BuildReport report) {
        Debug.Log("pre bef newBuildNumber: " + newBuildNumber);
        newBuildNumber = ++PlayerSettings.Android.bundleVersionCode;
        Debug.Log("pre aft newBuildNumber: " + newBuildNumber);
        UpdateAllPlatformVersions();
#if DEVELOPMENT_BUILD
        Debug.Log("Pre Dev Build");
#else
        Debug.Log("Pre Prod Build");
        RunProdBuildProcess();
#endif

        BuildScriptableObject buildScriptableObject = ScriptableObject.CreateInstance<BuildScriptableObject>();
        buildScriptableObject.BuildNumber = PlayerSettings.Android.bundleVersionCode.ToString();

        AssetDatabase.DeleteAsset("Assets/Resources/Build.asset"); // delete any old build.asset
        AssetDatabase.CreateAsset(buildScriptableObject, "Assets/Resources/Build.asset"); // create the new one with correct build number before build starts
        AssetDatabase.SaveAssets();
    }

    private void UpdateAllPlatformVersions() {
        PlayerSettings.macOS.buildNumber = newBuildNumber.ToString();
        PlayerSettings.iOS.buildNumber = newBuildNumber.ToString();
    }

    private void RunProdBuildProcess() {
        Debug.Log("Pre VC: " + PlayerSettings.Android.bundleVersionCode);
        int major, minor, build, version = -1;
        bool majorSuccess = int.TryParse(Application.version.Split('.')[0], out major);
        bool minorSuccess = int.TryParse(Application.version.Split('.')[1], out minor);
        build = PlayerSettings.Android.bundleVersionCode;
        if (majorSuccess && minorSuccess) {
            version = major * 1000 + minor * 100 + build;
        }
        if (version != -1) {
            PlayerSettings.Android.bundleVersionCode = version;
        }
        Debug.Log("Post VC: " + PlayerSettings.Android.bundleVersionCode);
    }



    // Keepsakes for a time where build numbers aren't the same between platforms
    // PlayerSettings.macOS.buildNumber = IncrementBuildNumber(PlayerSettings.macOS.buildNumber);
    // PlayerSettings.WSA.packageVersion = new System.Version(PlayerSettings.WSA.packageVersion.Major, PlayerSettings.WSA.packageVersion.Minor, PlayerSettings.WSA.packageVersion.Build + 1);

    // private string IncrementBuildNumber(string buildNumber) {
    //     int.TryParse(buildNumber, out int outputBuildNumber);
    //     return (outputBuildNumber + 1).ToString();
    // }
}