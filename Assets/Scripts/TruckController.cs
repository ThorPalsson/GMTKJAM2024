using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Dot_Truck : System.Object
{
	public WheelCollider leftWheel;
	public GameObject leftWheelMesh;
	public WheelCollider rightWheel;
	public GameObject rightWheelMesh;
	public bool motor;
	public bool steering;
	public bool reverseTurn; 

}

public class TruckController : MonoBehaviour 
{
	public float maxMotorTorque;
	public float maxSteeringAngle;
	public List<Dot_Truck> truck_Infos;
	[SerializeField] private bool useTiltLimits;
	[SerializeField] private float tiltLimit = 126f;
	private float tiltLimitHigh;


	private void Start() {
		tiltLimitHigh = 360 - tiltLimit; 
	}


	public void VisualizeWheel(Dot_Truck wheelPair)
	{
		Quaternion rot;
		Vector3 pos;
		wheelPair.leftWheel.GetWorldPose ( out pos, out rot);
		wheelPair.leftWheelMesh.transform.position = pos;
		wheelPair.leftWheelMesh.transform.rotation = rot;
		wheelPair.rightWheel.GetWorldPose ( out pos, out rot);
		wheelPair.rightWheelMesh.transform.position = pos;
		wheelPair.rightWheelMesh.transform.rotation = rot;
	}

	private void TiltControl()
	{
		Vector3 rotation = transform.localEulerAngles; 
		float zRotation = rotation.z; 

		if (zRotation > tiltLimit && zRotation < tiltLimitHigh)
		{
			Debug.Log("Restart"); 
		}

		rotation.z = zRotation; 
		transform.localEulerAngles = rotation; 
	}

	public void Update()
	{
		if (useTiltLimits)
		{
			TiltControl();
		}

		float input = Input.GetAxis("Vertical");
		float motor = maxMotorTorque * input; 
		float steering = maxSteeringAngle * Input.GetAxis("Horizontal");
		float brakeTorque = Mathf.Abs(Input.GetAxis("Jump"));
		if (brakeTorque > 0.001) {
			brakeTorque = maxMotorTorque;
			motor = 0;
		} else {
			brakeTorque = 0;
		}

		if (input == 0) {
			brakeTorque = maxMotorTorque / 1.2f; 
		}

		foreach (Dot_Truck truck_Info in truck_Infos)
		{
			if (truck_Info.steering == true) {
				truck_Info.leftWheel.steerAngle = truck_Info.rightWheel.steerAngle = ((truck_Info.reverseTurn)?-1:1)*steering;
			}

			if (truck_Info.motor == true)
			{
				truck_Info.leftWheel.motorTorque = motor;
				truck_Info.rightWheel.motorTorque = motor;
			}

			truck_Info.leftWheel.brakeTorque = brakeTorque;
			truck_Info.rightWheel.brakeTorque = brakeTorque;

			VisualizeWheel(truck_Info);
		}
	}
}