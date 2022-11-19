// Dependencies
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Renders a voxel grid
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class VoxelRenderer : MonoBehaviour
{
	// Inspector variables
	public VoxelGrid voxelGrid;
	public int texturesBlockWidth;
	public VoxelData[] voxelsData;

	// Class variables
	private MeshFilter meshFilter;

	// Start is called before the first frame update
	private void Awake()
	{
		// Get components
		meshFilter = GetComponent<MeshFilter>();

		// If voxel grid is filled, generate a mesh for it
		if (voxelGrid != null) Refresh();
	}

	// Refresh the mesh
	public void Refresh()
	{
		VoxelMeshFactory voxelMeshFactory = new VoxelMeshFactory(voxelGrid, texturesBlockWidth, voxelsData);
		meshFilter.mesh = voxelMeshFactory.GenerateMesh();
	}
}
