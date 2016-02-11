using System;
using UnityEngine;


[Serializable]
public class AudioVrControls : BaseControls
{
	public float walkSpeed = 4f;
	public float turnSpeed = 1f;
	
	public override void OnEnable() {
		AudioVR.Link.startServer();
	}
	
	public override Vector3 GetMove(Vector3 current) {
		Vector3 forward = CameraManager.GetCameraForwardMovementVector();
		return forward * Time.deltaTime * walkSpeed * AudioVR.Link.getMove();
	}
	
	public override Quaternion GetRotation(Quaternion current) {
		Vector3 currForward = CameraManager.GetCameraForwardMovementVector();
		Vector3 newForward = AudioVR.Link.getForward();
		float angle = VectorExtension.angle(currForward, newForward); // allows for negative angles
		return Quaternion.AngleAxis(angle, Vector3.up);
	}

	public override void OnDisable() {
        AudioVR.Link.stopServer();
    }
}