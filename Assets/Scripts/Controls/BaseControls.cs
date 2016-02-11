using System;
using UnityEngine;

[Serializable]
public abstract class BaseControls
{
	/// <summary>
	/// Raises the enable event of the control.
	/// </summary>
    public abstract void OnEnable();

	/// <summary>
	/// Gets the movement vector. This vector has the length equal to the movement speed.
	/// </summary>
    public abstract Vector3 GetMove(Vector3 current);

	/// <summary>
	/// Gets the rotation Quaternion that must be applied to get from the current rotation the new rotation.
	/// </summary>
    public abstract Quaternion GetRotation(Quaternion current);

    /// <summary>
    /// Called when the control is disabled.
    /// </summary>
    public abstract void OnDisable();
}
