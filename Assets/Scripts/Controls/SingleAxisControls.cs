using System;
using UnityEngine;

[Serializable]
public class SingleAxisControls : BaseControls
{
    public float walkSpeed;
    public float turnSpeed;

    public override void OnEnable()
    {
        
    }

    public override UnityEngine.Vector3 GetMove(Vector3 current)
    {
        Vector3 forward = CameraManager.GetCameraForwardMovementVector();
#if UNITY_ANDROID
		return Vector3.zero;
		/*if(Input.touchCount == 0) return Vector3.zero;
		return forward * (Time.deltaTime * walkSpeed * Input.GetTouch(0).deltaPosition.y);*/
#else
		return forward * Time.deltaTime * walkSpeed * Input.GetAxis("Vertical");
#endif
    }

    public override UnityEngine.Quaternion GetRotation(Quaternion current)
    {
#if UNITY_ANDROID
		float currForward = Vector3.Angle(Vector3.forward, CameraManager.GetCameraForwardMovementVector());
		float newForward = Input.compass.trueHeading;
		//if gyro => return Input.gyro.attitude;
		return Quaternion.AngleAxis(newForward - currForward, Vector3.up);
#else
        float turned = Time.deltaTime * turnSpeed * Input.GetAxis("Horizontal");
        return Quaternion.AngleAxis(turned, Vector3.up);
#endif
    }

    public override void OnDisable()
    {
        
    }
}
