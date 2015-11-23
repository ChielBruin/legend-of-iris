using UnityEngine;

public class DemoModeManager : MonoBehaviour {

    public bool demonstrationMode = false;
    public static DemoModeManager instance;

    // Instantiate this singleton object
    void Awake() {
        instance = this;
    }

    // Turn demonstration mode on
    public void SetDemonstrationMode() {
        demonstrationMode = true;
    }

    // Turn demonstration mode off
    public void SetStoryMode() {
        demonstrationMode = false;
    }
}
