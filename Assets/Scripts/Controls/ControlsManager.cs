﻿using UnityEngine;
using System.Collections;

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

#if UNITY_ANDROID
	public ControllerOption DefaultControls = ControllerOption.MobileControls;
#else
    public ControllerOption DefaultControls = ControllerOption.MobileControls;
#endif
    public bool needsReset;

    public void SetSingleAxisControls()
    {
        SetControls(singleAxisControls);
    }

    public void SetFirstPersonShooterControls()
    {
        SetControls(firstPersonShooterControls);
    }

    public void SetFixedDirectionControls()
    {
        SetControls(fixedDirectionControls);
    }

    public bool NeedsReset()
    {
        return needsReset;
    }

    public void ResetDone()
    {
        needsReset = false;
    }

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
