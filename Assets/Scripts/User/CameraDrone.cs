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
	public float orbitSpeed;
	public float droneSpeed;
	public float droneRotationSpeed;

	// Class variables
	private VoxelRenderer voxelRenderer;
	private Vector3 subjectPosition = Vector3.zero;
	private Setting setting = Setting.Orbit;
	private Vector3 dronePosition;
	private Quaternion droneRotation;

	// Setting types
	public enum Setting
	{
		Orbit,
		Drone
	}

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
		// Break camera from orbit
		if (
			setting != Setting.Drone
			&& (Input.GetKeyDown(KeyCode.W)
			|| Input.GetKeyDown(KeyCode.S)
			|| Input.GetKeyDown(KeyCode.A)
			|| Input.GetKeyDown(KeyCode.D)
			|| Input.GetKeyDown(KeyCode.UpArrow)
			|| Input.GetKeyDown(KeyCode.DownArrow)
			|| Input.GetKeyDown(KeyCode.LeftArrow)
			|| Input.GetKeyDown(KeyCode.RightArrow))
		)
		{
			setting = Setting.Drone;
			dronePosition = transform.position;
			droneRotation = transform.rotation;
		}

		// Break camera from drone
		if (Input.GetKeyDown(KeyCode.Tab)) setting = Setting.Orbit;

		// Switch between orbit and drone
		switch (setting)
		{
			case Setting.Orbit:
				// Update orbit amount
				if (Application.isPlaying) orbitAmount += orbitSpeed * Time.deltaTime;

				// Set position
				transform.position = new Vector3(
					subjectPosition.x + Mathf.Cos(orbitAmount) * radius,
					subjectPosition.y + height,
					subjectPosition.z + Mathf.Sin(orbitAmount) * radius
				);

				// Look at subject
				transform.LookAt(subjectPosition);
				break;

			case Setting.Drone:
				// Speed modifier on SHIFT
				float actualDroneSpeed = Input.GetKey(KeyCode.LeftShift) ? droneSpeed * 2 : droneSpeed;

				// Movement input
				Vector3 deltaPosition = Vector3.zero;
				if (Input.GetKey(KeyCode.W)) deltaPosition += transform.forward;
				if (Input.GetKey(KeyCode.S)) deltaPosition -= transform.forward;
				if (Input.GetKey(KeyCode.D)) deltaPosition += transform.right;
				if (Input.GetKey(KeyCode.A)) deltaPosition -= transform.right;
				dronePosition += deltaPosition.normalized * actualDroneSpeed * Time.deltaTime;
				transform.position = Vector3.Lerp(transform.position, dronePosition, 0.1f);

				// Rotation input
				if (Input.GetKey(KeyCode.UpArrow)) droneRotation *= Quaternion.Euler(Vector3.left * droneRotationSpeed * Time.deltaTime);
				if (Input.GetKey(KeyCode.DownArrow)) droneRotation *= Quaternion.Euler(Vector3.right * droneRotationSpeed * Time.deltaTime);
				if (Input.GetKey(KeyCode.RightArrow)) droneRotation = Quaternion.Euler(Vector3.up * droneRotationSpeed * Time.deltaTime) * droneRotation;
				if (Input.GetKey(KeyCode.LeftArrow)) droneRotation = Quaternion.Euler(Vector3.down * droneRotationSpeed * Time.deltaTime) * droneRotation;
				transform.rotation = Quaternion.Lerp(transform.rotation, droneRotation, 0.1f);

				break;
		}
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