// Dependencies
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// Orbits around a central point
public class CameraDrone : MonoBehaviour
{
	// Inspector variables
	public float height;
	public float radius;
	public float orbitAmount;

	// Class variables
	private VoxelRenderer voxelRenderer;
	private Vector3 subjectPosition = Vector3.zero;

	// Start is called before the first frame update
	private void Start()
	{
		// Get voxel renderer
		voxelRenderer = FindObjectOfType<VoxelRenderer>();

		// Wait for voxel grid variable
		StartCoroutine(WaitForVoxelGrid());
	}

	// Update is called once per frame
	private void Update()
	{
		// Update orbit amount
		if (Application.isPlaying) orbitAmount += 0.1f * Time.deltaTime;

		// Set position
		transform.position = new Vector3(
			subjectPosition.x + Mathf.Cos(orbitAmount) * radius,
			subjectPosition.y + height,
			subjectPosition.z + Mathf.Sin(orbitAmount) * radius
		);

		// Look at subject
		transform.LookAt(subjectPosition);
	}

#if UNITY_EDITOR
	// Draw camera features
	private void OnDrawGizmos()
	{
		Handles.color = Color.blue;
		Handles.DrawWireDisc(subjectPosition + height * Vector3.up, Vector3.up, radius);
	}
#endif

	// Don't populate the position until voxel grid is filled
	private IEnumerator WaitForVoxelGrid()
	{
		// Wait for voxel grid to populate
		while (voxelRenderer.voxelGrid == null) yield return null;

		// Update subject position now that we can access it
		subjectPosition
			= voxelRenderer.transform.position
			+ new Vector3(voxelRenderer.voxelGrid.width / 2, voxelRenderer.voxelGrid.height / 2, voxelRenderer.voxelGrid.length / 2);
	}
}