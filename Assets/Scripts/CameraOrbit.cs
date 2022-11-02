// Dependencies
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// Orbits around a central point
public class CameraOrbit : MonoBehaviour
{
	// Inspector variables
	public Vector3 focus;
	public float height;
	public float radius;

	// Start is called before the first frame update
	private void Start()
	{

	}

	// Update is called once per frame
	private void Update()
	{

	}

	// Draw camera features
	private void OnDrawGizmos()
	{
		Handles.color = Color.blue;
		Handles.DrawWireDisc(focus + height * Vector3.up, Vector3.up, radius);
	}
}
