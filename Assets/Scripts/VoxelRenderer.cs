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
	public VoxelTextureData[] voxelTexturesData;

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
		OldVoxelMeshFactory voxelMeshFactory = new OldVoxelMeshFactory(voxelGrid, texturesBlockWidth, voxelTexturesData, texture2D);
		meshFilter.mesh = voxelMeshFactory.GenerateMesh();

		// What this should look like
		//meshFilter.mesh = VoxelMeshFactory.GenerateMesh(voxelGrid, texturesBlockWidth, voxelTexturesData);
		//Mesh decoration = VoxelMeshFactory.GenerateMesh(voxelGrid, texture2D);
	}
}
