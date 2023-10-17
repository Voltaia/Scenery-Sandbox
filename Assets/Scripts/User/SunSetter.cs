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
	public float cinematicSpeed = 1.5f;

	// Class variables
	private Vector3 startRotation;

	// Different settings for the sun
	public enum Setting
	{
		RealTime,
		Cinematic,
		Paused
	}

	// Start is called before the first frame update
	private void Start()
	{
		// Get the starting rotation of the sun
		startRotation = transform.rotation.eulerAngles;
	}

	// Called every frame
	private void Update()
	{
		// Check input for tab which switches between settings
		if (Input.GetKeyUp(KeyCode.Space))
			if (setting == Setting.Cinematic) setting = Setting.Paused;
			else setting = Setting.Cinematic;

		// Switch between settings
		switch (setting)
		{
			case Setting.RealTime:
				hour = System.DateTime.Now.Hour + System.DateTime.Now.Minute / 60f;
				break;

			case Setting.Cinematic:
				hour = Mathf.Repeat(hour + cinematicSpeed * Time.deltaTime, 24f);
				break;

			case Setting.Paused:
				if (Input.GetKeyDown(KeyCode.LeftArrow)) hour--;
				else if (Input.GetKeyUp(KeyCode.RightArrow)) hour++;
				break;
		}

		// Set sun rotation
		float dayProgress = hour / 24.0f;
		transform.rotation = Quaternion.Euler((dayProgress * 360) - 90, startRotation.x, startRotation.z);
	}
}
