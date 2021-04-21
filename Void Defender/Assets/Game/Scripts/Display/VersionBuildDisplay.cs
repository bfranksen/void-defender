using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VersionBuildDisplay : MonoBehaviour {

    TextMeshProUGUI versionText;

    private void Awake() {
        SetVersionText();
    }

    private void SetVersionText() {
        versionText = GetComponent<TextMeshProUGUI>();
        ResourceRequest request = Resources.LoadAsync("Build", typeof(BuildScriptableObject));
        request.completed += Request_completed;
    }

    private void Request_completed(AsyncOperation obj) {
        BuildScriptableObject buildScriptableObject = ((ResourceRequest)obj).asset as BuildScriptableObject;
        if (buildScriptableObject == null) {
            Debug.LogError("Build scriptable object not found in resources directory! Check build log for errors!");
        } else {
            versionText.SetText($"v{Application.version}.{buildScriptableObject.BuildNumber}");
        }
    }
}
