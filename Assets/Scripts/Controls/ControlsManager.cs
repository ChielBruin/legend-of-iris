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
	public AudioVrControls audioVrControls;
    public FirstPersonShooterControls firstPersonShooterControls;
    public FixedDirectionControls fixedDirectionControls;
    public ControllerOption DefaultControls = ControllerOption.SingleAxisControls;
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

    public void SetControls(BaseControls controls)
    {
        if (current != null)
            current.OnDisable();
        //current = controls;
		current = audioVrControls;
        current.OnEnable();
        needsReset = true;
    }

    void Start()
    {
        switch(DefaultControls)
        {
            case ControllerOption.FixedDirectionControls: SetControls(fixedDirectionControls); break;
            case ControllerOption.SingleAxisControls: SetControls(singleAxisControls); break;
        }
    }

    public void onDisable() {
        if (current != null) current.OnDisable();
    }

    public enum ControllerOption
    {
        SingleAxisControls, FixedDirectionControls
    }
}
