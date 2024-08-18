using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Collections;
using System;
using Unity.VisualScripting;

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

	public bool canDrive; 
	[SerializeField] private bool useDownwardsForce;
	[SerializeField] private float downwardsForceMultiplier = 4;
	public float maxMotorTorque;
	[Range(1,2)]
	public float motorTorqueUpgrade = 1.1f;
	public float maxBreakPower = 850;
	[Range(1,2)]
	public float breakPowerUpgrade = 1.5f;
	public float maxSteeringAngle;
	public List<Dot_Truck> truck_Infos;
    [SerializeField] private float maxThrottleSpeed = 10;	
	[SerializeField] private bool useTiltLimits;
	[SerializeField] private float stallDeceleration = 200; 
	[SerializeField] private float tiltLimit = 126f;
	[SerializeField] private TMP_Text speedometerText;
	[SerializeField] private TMP_Text gearText;
	private float tiltLimitHigh;
	public Rigidbody rb;
	[SerializeField] private float visualSpeedModifier = 4;

	private int currentGear; 
	public Dictionary<int, float> Gears = new Dictionary<int, float>();
	public int[] Speeds; 
	private float gearTimer = .5f; 
	private int gearUpgrades; 
	[SerializeField] private float gearBoost = 0; 
	private bool isChangingGear;

	[SerializeField] private bool atThrottleLimit;

	[SerializeField] private float boostPower = 200;
	[Range(1,2)]public float BoostPowerUpgrades = 1.2f;

    [SerializeField] private GameObject boostVFX; 
	[SerializeField] private GameObject gearVFX; 

	[SerializeField] private bool hasNitro; 


	[SerializeField] private Transform CargoPoint; 

	public bool ComingFromRespawn; 
	public float Speed;

	private float downGearValue; 
	private float upGearValue; 
	[SerializeField]private float pitchModifier = 1.5f;
	[SerializeField] private float lowPitchValue = .5f; 

	[SerializeField] private AudioSource carAudio; 
	[SerializeField] private float audioPerc; 

	private float carAudioVolume; 

	[SerializeField] private AudioClip[] gearClips; 
	[SerializeField] private AudioSource gearSource;


	[Header("Visual Upgrades")]
	[SerializeField] private GameObject backBar; 

	private void Awake() {
		rb = GetComponent<Rigidbody>();
	}

	private void Start() {
		tiltLimitHigh = 360 - tiltLimit; 

		currentGear = 1; 
		gearText.text = "1";

		for (int i = 0; i < Speeds.Length; i++)
		{
			Gears.Add(Speeds.Length - i, Speeds[i]);
		}

		downGearValue = 0; 
		upGearValue = Gears[currentGear + 1];

		carAudioVolume = carAudio.volume; 

		gearSource.clip = gearClips[0];
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

	private void HandleGears(float speedValue)
	{
		if (isChangingGear) return;
		var expectedGear = currentGear; 

		if (speedValue < 0)
		{
			expectedGear = -1;
		} else 
		{
			foreach(KeyValuePair<int, float> gear in Gears)
			{
				if (speedValue >= gear.Value)
				{
					if (gear.Key != currentGear)
					{
						expectedGear = gear.Key;
						break;
					}
				}
			}
		}

		print (expectedGear);

		if (expectedGear != currentGear)
		{
			if (expectedGear > currentGear && (speedValue - 2) >= Gears[Mathf.Min(currentGear + 1, 5)] )
			{ 
				StartCoroutine(ChangeGear(Mathf.Min(currentGear + 1, 5)));
			} else if (expectedGear < currentGear && (speedValue + 2) <= Gears[Mathf.Max(currentGear, 1)])
			{
				StartCoroutine(ChangeGear(expectedGear));
			}

		}

	}

	public void ParkCar()
	{
		canDrive = false;
	}

	public void UnParkCar()
	{
		canDrive = true;
	}


	private IEnumerator ChangeGear(int newGear)
	{
		carAudio.volume = (carAudioVolume * .2f); 
		gearSource.Play();
		isChangingGear = true;
		yield return new WaitForSeconds(gearTimer);

		currentGear = newGear;

		if (currentGear != 5 && currentGear != -1)
		{
			downGearValue = Gears[currentGear]; 
			upGearValue = Gears[currentGear + 1]; 
		}  else 
		{
			downGearValue = Gears[currentGear]; 
			upGearValue = maxThrottleSpeed; 
		}

		if (newGear != -1)
			gearText.text = newGear.ToString();
		else 
			gearText.text = "R";

		carAudio.volume = carAudioVolume;
		isChangingGear = false;

		if (gearBoost != 0)
		{
			rb.AddForce(transform.forward * gearBoost);

			gearVFX.SetActive(true); 
			yield return new WaitForSeconds(.1f);
			gearVFX.SetActive(false);
		}
	}


	public void Update()
	{
		if (!canDrive)
		{
			foreach (Dot_Truck truck_Info in truck_Infos)
			{

				truck_Info.leftWheel.brakeTorque = maxBreakPower;
				truck_Info.rightWheel.brakeTorque = maxBreakPower;
			}
			return; 
		}

		if (useDownwardsForce)
		{
			rb.AddForce(transform.up * - (Speed * downwardsForceMultiplier));
		}

		if (useTiltLimits)
		{
			TiltControl();
		}


		if (hasNitro && Input.GetKey(KeyCode.LeftShift))
		{
			rb.AddForce(transform.forward * boostPower); 
			boostVFX.SetActive(true);
		} else if (boostVFX.activeSelf)
		{
			boostVFX.SetActive(false);
		}

		float input = Input.GetAxis("Vertical");
		float motor = maxMotorTorque * input; 
		float steering = maxSteeringAngle * Input.GetAxis("Horizontal");
		float brakeTorque = Mathf.Abs(Input.GetAxis("Jump"));
		if (brakeTorque > 0.001) {
			brakeTorque = maxBreakPower;
			motor = 0;
		} else {
			brakeTorque = 0;
		}

		if (input == 0) {
			brakeTorque = stallDeceleration; 
		}

		Speed = rb.linearVelocity.magnitude;

		var visualSpeed = Speed * visualSpeedModifier; 
		HandleGears(visualSpeed);
		speedometerText.text = visualSpeed.ToString("F0") + "KM/H";
		PitchAudio();

		atThrottleLimit = visualSpeed > maxThrottleSpeed; 

		if (!atThrottleLimit && visualSpeed > maxThrottleSpeed - 10)
		{
			var perc = (visualSpeed - (maxThrottleSpeed - 10)) / 10; 
			perc = Mathf.Abs(perc - 1);

			motor = motor * perc;
		}

		if (atThrottleLimit || isChangingGear) motor = 0;

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

	private void PitchAudio()
	{
		var speed = Speed * visualSpeedModifier; 
		var perc = (speed - downGearValue) / (upGearValue - downGearValue);
		var shiftValue = perc * pitchModifier; 

		//print ($"{downGearValue} / {speed} / {upGearValue}"); 
		//print ("perc: " + perc + "shiftValue: " + shiftValue);


		carAudio.pitch = lowPitchValue + shiftValue; 
	}

	public void AddPower()
	{
		maxMotorTorque *= motorTorqueUpgrade; 
		maxThrottleSpeed += 5;
	}

	public void AddBreaks()
	{
		maxBreakPower *= breakPowerUpgrade; 
	}

	public void NitroUpgrade()
	{
		if (!hasNitro)
		{
			hasNitro = true;
			return;
		}

		boostPower *= BoostPowerUpgrades;
	}

	public void AddBackBar()
	{
		backBar.SetActive(true);
	}

	public void UpdateGearBox()
	{
		gearUpgrades ++; 


		switch (gearUpgrades)
		{
			case 1: 
				gearTimer  = .2f; 
				gearSource.clip = gearClips[1]; 
				break;
			case 2:
				gearTimer = .1f;
				gearSource.clip = gearClips[2]; 
				gearBoost = 200; 
				break;
			case 3:
				gearTimer = .075f;
				gearBoost += 200; 
				break;
			case 4: 
				gearTimer = .05f; 
				gearBoost += 500;
				break;
			default:
				gearBoost += 500; 
				break;
		}
	}

	public void AddCargo(GameObject newCargo)
	{
		var cargoKids = CargoPoint.GetComponentsInChildren<Transform>();
		foreach(var c in cargoKids)
		{
			if (c.gameObject != CargoPoint)
				c.gameObject.SetActive(false);
		}	
		Instantiate(newCargo, CargoPoint.position, CargoPoint.rotation, CargoPoint);
	}

}