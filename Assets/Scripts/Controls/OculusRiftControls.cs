using System;
using UnityEngine;

[Serializable]
/// <summary>
/// Controls used when playing with the oculus.
/// </summary>
public class OculusRiftControls : BaseControls
{
    public float walkSpeed;
    public float turnSpeed;

    public override UnityEngine.Vector3 GetMove(Vector3 current)
    {
        Vector3 forward = CameraManager.GetCameraForwardMovementVector();
        return forward * Time.deltaTime * walkSpeed * Input.GetAxis("Vertical");
    }

	/// <summary>
	/// Gets the horizontal axis relative heading from the oculus.
	/// Note that the oculus inputs are mapped to O and P.
	/// </summary>
	/// <returns>The relative horizontal heading</returns>
    private float GetHorizontalAxis()
    {
        float r = 0f;
        if (Input.GetKey(KeyCode.O))
        {
            r += -1f;
        }
        if (Input.GetKey(KeyCode.P))
        {
            r += 1f;
        }
        return r;
    }

    public override UnityEngine.Quaternion GetRotation(Quaternion current)
    {
        float turned = Time.deltaTime * turnSpeed * GetHorizontalAxis();
        return Quaternion.AngleAxis(turned, Vector3.up);
    }

    public override void OnDisable()
    {

    }

	public override ControllerOption GetType() {
		return ControllerOption.OculusRiftControls;
	}
}
