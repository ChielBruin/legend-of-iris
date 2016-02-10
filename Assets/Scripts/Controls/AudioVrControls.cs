using System;
using UnityEngine;


[Serializable]
public class AudioVrControls : BaseControls
{
	public float walkSpeed = 4f;
	public float turnSpeed = 1f;
	
	public override void OnEnable()
	{
		AudioVR.Link.startServer();
	}
	
	public override UnityEngine.Vector3 GetMove(Vector3 current)
	{
		Vector3 forward = CameraManager.GetCameraForwardMovementVector();
		return forward * Time.deltaTime * walkSpeed * AudioVR.Link.getMove();
	}
	
	public override UnityEngine.Quaternion GetRotation(Quaternion current)
	{
		/*float turned = Time.deltaTime * turnSpeed * Input.GetAxis("Horizontal");
		return Quaternion.AngleAxis(turned, Vector3.up);*/
		Vector3 currForward = CameraManager.GetCameraForwardMovementVector();
		Vector3 newForward = AudioVR.Link.getForward();
		float angle = Vector3.Angle(currForward, newForward);
		Vector3 cross = Vector3.Cross(currForward, newForward);
		if (cross.y < 0) angle = -angle;
		Debug.LogError (currForward + "->" + newForward + ": " + angle);
		return Quaternion.AngleAxis(angle, Vector3.up);
	}
	
	public override void OnDisable()
	{

	}
}