// Dependencies
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Generates an outline around a voxel renderer
[RequireComponent(typeof(VoxelRenderer))]
public class VoxelGridOutline : MonoBehaviour
{
	// Inspector variables
	public Material material;

	// Class variables
	private VoxelRenderer voxelRenderer;
	private LineRenderer[] lineRenderers = new LineRenderer[12];

	// Start is called before the first frame update
	private void Start()
	{
		// Set up
		voxelRenderer = GetComponent<VoxelRenderer>();

		// Wait for voxel grid to populate before generating
		StartCoroutine(WaitForVoxelGrid());
	}

	// When enabled
	private void OnEnable()
	{
		foreach (LineRenderer lineRenderer in lineRenderers) if (lineRenderer != null) lineRenderer.enabled = true;
	}

	// When disabled
	private void OnDisable()
	{
		foreach (LineRenderer lineRenderer in lineRenderers) if (lineRenderer != null) lineRenderer.enabled = false;
	}

	// Generate everything after voxel grid is populated
	private IEnumerator WaitForVoxelGrid()
	{
		// Wait for voxel grid to populate
		while (voxelRenderer.voxelGrid == null) yield return null;

		// Create game objects for line renderers
		for (int index = 0; index < lineRenderers.Length; index++)
		{
			// Create parent game object
			GameObject gameObject = new GameObject("Line");
			gameObject.transform.SetParent(transform);

			// Create line renderer
			LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
			lineRenderer.material = material;
			lineRenderer.startWidth = 0.1f;
			lineRenderer.endWidth = 0.1f;
			lineRenderer.numCapVertices = 4;
			lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			lineRenderers[index] = lineRenderer;
		}

		// Set corners
		Vector3[] corners = new Vector3[voxelRenderer.cornerOffsets.Length];
		for (int index = 0; index < corners.Length; index++) corners[index] = new Vector3(
			voxelRenderer.cornerOffsets[index].x * voxelRenderer.voxelGrid.width,
			voxelRenderer.cornerOffsets[index].y * voxelRenderer.voxelGrid.height,
			voxelRenderer.cornerOffsets[index].z * voxelRenderer.voxelGrid.length
		);

		// Bottom lines
		lineRenderers[0].SetPosition(0, corners[0]);
		lineRenderers[0].SetPosition(1, corners[1]);

		lineRenderers[1].SetPosition(0, corners[0]);
		lineRenderers[1].SetPosition(1, corners[3]);

		lineRenderers[2].SetPosition(0, corners[1]);
		lineRenderers[2].SetPosition(1, corners[2]);

		lineRenderers[3].SetPosition(0, corners[2]);
		lineRenderers[3].SetPosition(1, corners[3]);

		// Middle lines
		lineRenderers[4].SetPosition(0, corners[1]);
		lineRenderers[4].SetPosition(1, corners[5]);

		lineRenderers[5].SetPosition(0, corners[3]);
		lineRenderers[5].SetPosition(1, corners[7]);

		lineRenderers[6].SetPosition(0, corners[0]);
		lineRenderers[6].SetPosition(1, corners[4]);

		lineRenderers[7].SetPosition(0, corners[2]);
		lineRenderers[7].SetPosition(1, corners[6]);

		// Top lines
		lineRenderers[8].SetPosition(0, corners[5]);
		lineRenderers[8].SetPosition(1, corners[6]);

		lineRenderers[9].SetPosition(0, corners[4]);
		lineRenderers[9].SetPosition(1, corners[7]);

		lineRenderers[10].SetPosition(0, corners[6]);
		lineRenderers[10].SetPosition(1, corners[7]);

		lineRenderers[11].SetPosition(0, corners[4]);
		lineRenderers[11].SetPosition(1, corners[5]);
	}
}
