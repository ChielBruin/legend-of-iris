using UnityEngine;
using System.Collections;

/// <summary>
/// Manager for the different control methods.
/// Facilitates a nice way to switch them during the game. 
/// </summary>
public class ControlsManager : MonoBehaviour {
    public static ControlsManager instance;
    void Awake()
    {
        instance = this;
    }
    private static BaseControls _current;
    public static BaseControls current
    {
        get
        {
			// Override controlls when the oculus is active
            if (CameraManager.instance.oculusRiftActivated)
            {
                return instance.oculusRiftControls;
            }
            else
            {
                return _current;
            }
        }
        set
        {
            _current = value;
        }
    }
    public SingleAxisControls singleAxisControls;
    public OculusRiftControls oculusRiftControls;
    public FirstPersonShooterControls firstPersonShooterControls;
    public FixedDirectionControls fixedDirectionControls;
	public MobileControls mobileControls;

	// Switch the default contronls depending on if the game is run on mobile or not
#if UNITY_ANDROID
	public ControllerOption DefaultControls = ControllerOption.MobileControls;
#else
    public ControllerOption DefaultControls = ControllerOption.MobileControls;
#endif
    public bool needsReset;

	/// <summary>
	/// Set the current controls to the SingleAxisControls
	/// </summary>
    public void SetSingleAxisControls()
    {
        SetControls(singleAxisControls);
    }

	/// <summary>
	/// Set the current controls to the FPS controls
	/// </summary>
    public void SetFirstPersonShooterControls()
    {
        SetControls(firstPersonShooterControls);
    }


	/// <summary>
	/// Set the current controls to the DixedDirectionControls
	/// </summary>
    public void SetFixedDirectionControls()
    {
        SetControls(fixedDirectionControls);
    }

	/// <returns>True when the manager needs a reset, false otherwise</returns>
    public bool NeedsReset()
    {
        return needsReset;
    }

	/// <summary>
	/// Method called when the reset is done
	/// </summary>
    public void ResetDone()
    {
        needsReset = false;
    }

	/// <summary>
	/// Set the current controls to the given control.
	/// Marks the manager for a reset when the change occurs.
	/// Note that mobile controlls always override any control when on mobile.
	/// </summary>
	/// <param name="controls">The new control to set it to</param>
    public void SetControls(BaseControls controls) {
#if UNITY_ANDROID
		if (controls.GetType() != ControllerOption.MobileControls) {
			SetControls(mobileControls);
			return;
		}
#endif
		if (current == controls) return;
        if (current != null)
            current.OnDisable();
        current = controls;
        current.OnEnable();
        needsReset = true;
    }

	/// <summary>
	/// Called when the manager is started, initiates with the default controls.
	/// </summary>
    void Start()
    {
        switch(DefaultControls)
        {
            case ControllerOption.FixedDirectionControls: SetControls(fixedDirectionControls); break;
            case ControllerOption.SingleAxisControls: SetControls(singleAxisControls); break;
			case ControllerOption.MobileControls: SetControls(mobileControls); break;
        }
    }
}
