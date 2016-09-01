using System;
using UnityEngine;

[Serializable]
/// <summary>
/// Single axis controls.
/// A control used when the input is composed of only a froward/backward and a left/right turn.
/// </summary>
public class SingleAxisControls : BaseControls {

    public float walkSpeed;
    public float turnSpeed;

    public override UnityEngine.Vector3 GetMove(Vector3 current) {
        Vector3 forward = CameraManager.GetCameraForwardMovementVector();
		return forward * Time.deltaTime * walkSpeed * Input.GetAxis("Vertical");
    }

    public override UnityEngine.Quaternion GetRotation(Quaternion current) {
		float turned = Time.deltaTime * turnSpeed * Input.GetAxis("Horizontal");
        return Quaternion.AngleAxis(turned, Vector3.up);
    }

	public override ControllerOption GetType() {
		return ControllerOption.SingleAxisControls;
	}
}
