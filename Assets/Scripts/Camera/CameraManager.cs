﻿using UnityEngine;
using System.Collections;
using System;

public class CameraManager : MonoBehaviour {

    public static CameraManager instance;
    void Awake()
    {
        instance = this;
    }

    public VisualAidsCamera visualAidsCamera;
    public Camera oculusRiftRightCamera;
    public Camera normalCamera;
    public Camera audioVRLinkCamera;
    public CanvasGroup visualAidsGroup;
    public GameObject screenAudioObject;
    public CameraSetting setting;
    public static Camera currentFirstPersonCamera;
    public static Camera currentViewingCamera;
    public bool oculusRiftActivated;

    public enum CameraSetting
    {
        NORMAL, HEADTRACKING, OCULUSRIFT, AUDIOVRLINK
    }

    public static Vector3 GetCameraForwardVector() { return instance.getCameraForwardVector(); }
    private Vector3 getCameraForwardVector()
    {
        return currentFirstPersonCamera.transform.TransformDirection(Vector3.forward);
    }

    public static Vector3 GetCameraForwardMovementVector() { return instance.getCameraForwardMovementVector(); }
    private Vector3 getCameraForwardMovementVector()
    {
        if (currentFirstPersonCamera == null)
            return new Vector3(1, 0, 0);
        return currentFirstPersonCamera.transform.TransformDirection(Vector3.forward).sety(0).normalized;
    }

    public static Vector3 GetCameraRightVector() { return instance.getCameraRightVector(); }
    private Vector3 getCameraRightVector()
    {
        return currentFirstPersonCamera.transform.TransformDirection(Vector3.right);
    }

    public bool IsOculusRiftConnected()
    {
        try
        {
            if (!OVRDevice.IsHMDPresent())
            {
                return false;
            }
        }
        catch (System.Exception e)
        {
            return false;
        }
        return true;
    }

    public void SetNormalMode()
    {
        setting = CameraSetting.NORMAL;
        setNormalCameraEnabled(true);
        setOculusRiftCameraEnabled(false);
        setVisualAidsCameraEnabled(true);
        setAudioVRLinkCameraEnabled(false);
        currentFirstPersonCamera = normalCamera;
        currentViewingCamera = visualAidsCamera.GetComponent<Camera>();
        visualAidsGroup.alpha = 1f;
        oculusRiftActivated = false;
        ScreenAudioManager.SetScreen(normalCamera);
    }

    public void SetHeadTrackingMode()
    {
        if (!IsOculusRiftConnected())
        {
            Debug.LogError("Oculus Rift is not connected.");
            return;
        }
        setting = CameraSetting.HEADTRACKING;
        setNormalCameraEnabled(false);
        setOculusRiftCameraEnabled(true);
        setVisualAidsCameraEnabled(true);
        setAudioVRLinkCameraEnabled(false);
        currentFirstPersonCamera = oculusRiftRightCamera;
        currentViewingCamera = visualAidsCamera.GetComponent<Camera>();
        visualAidsGroup.alpha = 1f;
        oculusRiftActivated = true;
        ScreenAudioManager.SetScreen(oculusRiftRightCamera);
    }

    public void SetOculusRiftMode()
    {
        if (!IsOculusRiftConnected())
        {
            Debug.LogError("Oculus Rift is not connected.");
            return;
        }
        setting = CameraSetting.OCULUSRIFT;
        setNormalCameraEnabled(false);
        setOculusRiftCameraEnabled(true);
        setVisualAidsCameraEnabled(false);
        setAudioVRLinkCameraEnabled(false);
        currentFirstPersonCamera = oculusRiftRightCamera;
        currentViewingCamera = oculusRiftRightCamera;
        visualAidsGroup.alpha = 0f;
        oculusRiftActivated = true;
        ScreenAudioManager.SetScreen(oculusRiftRightCamera);
    }

    private void SetAudioVRLinkMode()
    {
        AudioVRLink.connect();
        if (!AudioVRLink.isConnected())
        {
            Debug.LogError("Audio VR Link is not connected.");
            return;
        }
        setting = CameraSetting.AUDIOVRLINK;
        setNormalCameraEnabled(false);
        setOculusRiftCameraEnabled(false);
        setVisualAidsCameraEnabled(false);
        setAudioVRLinkCameraEnabled(true);
        currentFirstPersonCamera = audioVRLinkCamera;
        currentViewingCamera = audioVRLinkCamera;
        visualAidsGroup.alpha = 0f;
        oculusRiftActivated = false;
        ScreenAudioManager.SetScreen(audioVRLinkCamera);
    }

    private void setAudioVRLinkCameraEnabled(bool enabled)
    {
        audioVRLinkCamera.transform.parent.gameObject.SetActive(enabled);
    }

    private void setNormalCameraEnabled(bool enabled)
    {
        normalCamera.transform.parent.gameObject.SetActive(enabled);
    }

    private void setVisualAidsCameraEnabled(bool enabled)
    {
        visualAidsCamera.gameObject.SetActive(enabled);
    }

    private void setOculusRiftCameraEnabled(bool enabled)
    {
        oculusRiftRightCamera.transform.parent.gameObject.SetActive(enabled);
    }

    public void HandleSetting()
    {
        switch (setting)
        {
            case CameraSetting.NORMAL:
                SetNormalMode(); break;
            case CameraSetting.HEADTRACKING:
                SetHeadTrackingMode(); break;
            case CameraSetting.OCULUSRIFT:
                SetOculusRiftMode(); break;
            case CameraSetting.AUDIOVRLINK:
                SetAudioVRLinkMode(); break;
        }
    }

    void Start()
    {
        HandleSetting();
    }
}
