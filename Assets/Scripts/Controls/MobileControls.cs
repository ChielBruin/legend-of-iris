using UnityEngine;
using System.Collections;

[System.Serializable]
public class MobileControls : BaseControls {
	
	private float walkSpeed = 2f;
	public bool hasGyro;
	private float compassHeading = 0f;
	
	/// <summary>
	/// Gets the touch input from the android device and converts this to a value indicating the walking speed.
	/// </summary>
	/// <returns>the generated speed value</returns>
	public float getTouchInput ()	{
		if (Input.touchCount != 1) {
			return Input.GetAxis("Vertical");
		}
		return Input.GetTouch (0).position.y > Screen.height / 2 ? 1f : -1f;
	}
	
	/// <summary>
	/// Checks if the gyroscope can be used, enables the compass otherwise.
	/// </summary>
	public override void OnEnable() {
		Input.location.Start();
		hasGyro = SystemInfo.supportsGyroscope;
		if (hasGyro) {
			Input.gyro.enabled = true;
			Debug.Log("Gyroscope enabled");
		} else {
			Input.compass.enabled = true;
			Debug.Log("Compas enabled, as this device does not support a gyroscope");
		}
	}

	public override UnityEngine.Vector3 GetMove(Vector3 current) {
		Vector3 forward = CameraManager.GetCameraForwardMovementVector();
		Vector3 res = forward * Time.deltaTime * walkSpeed * getTouchInput();
		Debug.Log("speed: " + walkSpeed + " res: " + res);
		return res;
	}

	public float getCompassAvgHeading() {
		float h = Input.compass.trueHeading;
		float res = (h + compassHeading) / 2f;
		compassHeading = h;
		return res;
	}

	public override UnityEngine.Quaternion GetRotation(Quaternion current) {
		if (hasGyro) {
			return Quaternion.AngleAxis(-Mathf.Rad2Deg * Input.gyro.rotationRateUnbiased.z * Time.deltaTime, Vector3.up);
		} else {
			return Quaternion.AngleAxis(getCompassAvgHeading() - current.eulerAngles.y, Vector3.up);
		}
	}

	public override void OnDisable() {
		if (hasGyro) {
			Input.gyro.enabled = false;
		} else {
			Input.compass.enabled = false;
		}
	}

	public override ControllerOption GetType() {
		return ControllerOption.MobileControls;
	}
}
