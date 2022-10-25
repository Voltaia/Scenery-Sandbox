// Dependencies
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Generates an outline around a voxel grid
[RequireComponent(typeof(VoxelGrid))]
public class VoxelGridOutline : MonoBehaviour
{
	// Inspector variables
	public Material material;

	// Start is called before the first frame update
	private void Start()
	{
		// Set up
		VoxelGrid voxelGrid = GetComponent<VoxelGrid>();
		LineRenderer[] lineRenderers = new LineRenderer[12];

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
		Vector3[] corners = new Vector3[voxelGrid.cornerOffsets.Length];
		for (int index = 0; index < corners.Length; index++) corners[index] = new Vector3(
			voxelGrid.cornerOffsets[index].x * voxelGrid.Width,
			voxelGrid.cornerOffsets[index].y * voxelGrid.Height,
			voxelGrid.cornerOffsets[index].z * voxelGrid.Length
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
