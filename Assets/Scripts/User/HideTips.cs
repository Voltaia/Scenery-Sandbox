using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideTips : MonoBehaviour
{
	// Inspector
	public GameObject tips;

	// Called every frame
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.H)) tips.SetActive(!tips.activeSelf);
	}
}
