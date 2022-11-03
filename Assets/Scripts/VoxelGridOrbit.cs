// Dependencies
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// Orbits around a central point
[ExecuteInEditMode]
public class VoxelGridOrbit : MonoBehaviour
{
	// Inspector variables
	public float height;
	public float radius;
	public float orbitAmount;

	// Class variables
	private Vector3 subjectPosition;

	// Start is called before the first frame update
	private void Start()
	{
		VoxelGrid voxelGrid = FindObjectOfType<VoxelGrid>();
		subjectPosition = voxelGrid.transform.position + new Vector3(voxelGrid.Width / 2, voxelGrid.Height / 2, voxelGrid.Length / 2);
	}

	// Update is called once per frame
	private void Update()
	{
		// Update orbit amount
		if (Application.isPlaying) orbitAmount += 0.1f * Time.deltaTime;

		// Set position
		transform.position = new Vector3(
			subjectPosition.x + Mathf.Cos(orbitAmount) * radius,
			height,
			subjectPosition.z + Mathf.Sin(orbitAmount) * radius
		);

		// Look at subject
		transform.LookAt(subjectPosition);
	}

	// Draw camera features
	private void OnDrawGizmos()
	{
		Handles.color = Color.blue;
		Handles.DrawWireDisc(subjectPosition + height * Vector3.up, Vector3.up, radius);
	}
}
