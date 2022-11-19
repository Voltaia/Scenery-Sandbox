// Dependencies
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Renders a voxel grid
public class VoxelRenderer : MonoBehaviour
{
	// Inspector variables
	public VoxelGrid voxelGrid;
	public int texturesBlockWidth;
	public Material material;
	public Texture2D texture2D;
	public VoxelData[] voxelsData;

	// Class variables
	private MeshRenderer meshRenderer;
	private MeshFilter meshFilter;

	// Start is called before the first frame update
	private void Awake()
	{
		// Get components
		meshRenderer = gameObject.AddComponent<MeshRenderer>();
		meshRenderer.material = material;
		meshFilter = gameObject.AddComponent<MeshFilter>();

		// If voxel grid is filled, generate a mesh for it
		if (voxelGrid != null) Refresh();
	}

	// Refresh the mesh
	public void Refresh()
	{
		VoxelMeshFactory voxelMeshFactory = new VoxelMeshFactory(voxelGrid, texturesBlockWidth, voxelsData, texture2D);
		meshFilter.mesh = voxelMeshFactory.GenerateMesh();
	}
}
