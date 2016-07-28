using System;
using UnityEngine;

[Serializable]
public class SingleAxisControls : BaseControls
{
    public float walkSpeed;
    public float turnSpeed;

    public override void OnEnable() {
#if UNITY_ANDROID
		Input.location.Start();
		Input.compass.enabled = true;
#endif
    }

    public override UnityEngine.Vector3 GetMove(Vector3 current)
    {
        Vector3 forward = CameraManager.GetCameraForwardMovementVector();
#if UNITY_ANDROID
		if(Input.touchCount < 1) return Vector3.zero;
		return forward * Time.deltaTime * walkSpeed * Input.GetTouch(0).deltaPosition.y;
#else
		return forward * Time.deltaTime * walkSpeed * Input.GetAxis("Vertical");
#endif
    }

    public override UnityEngine.Quaternion GetRotation(Quaternion current) {
#if UNITY_ANDROID
		return Quaternion.AngleAxis(Input.compass.trueHeading - current.eulerAngles.y, Vector3.up);
#else
        float turned = Time.deltaTime * turnSpeed * Input.GetAxis("Horizontal");
        return Quaternion.AngleAxis(turned, Vector3.up);
#endif
    }

    public override void OnDisable()
    {
        
    }
}
