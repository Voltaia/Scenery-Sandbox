// Dependencies
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Changes the direction of the sun
public class SunSetter : MonoBehaviour
{
	// Inspector variable
	public Setting setting = Setting.RealTime;
	public float hour = 12;
	public float cinematicSpeed = 0.005f;

	// Class variables
	private Vector3 startRotation;

	// Different settings for the sun
	public enum Setting
	{
		RealTime,
		Cinematic,
		Custom
	}

	// Start is called before the first frame update
	private void Start()
	{
		// Get the starting rotation of the sun
		startRotation = transform.rotation.eulerAngles;
	}

	// Updates the sun to the hour
	private void Update()
	{
		// Check input for tab which switches between settings
		if (Input.GetKeyUp(KeyCode.Tab))
			if (setting == Setting.RealTime) setting = Setting.Cinematic;
			else setting = Setting.RealTime;

		// Switch between settings
		switch (setting)
		{
			case Setting.RealTime:
				hour = System.DateTime.Now.Hour + System.DateTime.Now.Minute / 60f;
				break;

			case Setting.Cinematic:
				hour = Mathf.Repeat(hour + cinematicSpeed, 24f);
				break;
		}

		// Set sun rotation
		float dayProgress = hour / 24.0f;
		transform.rotation = Quaternion.Euler((dayProgress * 360) - 90, startRotation.x, startRotation.z);
	}
}
