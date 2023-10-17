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
	public VoxelTextureData[] voxelTexturesData;
	public static VoxelTextureData[] texturesData;

	// Class variables
	private MeshRenderer meshRenderer;
	private MeshFilter meshFilter;

	// Start is called before the first frame update
	private void Awake()
	{
		// Set static textures data
		texturesData = voxelTexturesData;

		// Get components
		meshRenderer = gameObject.AddComponent<MeshRenderer>();
		meshRenderer.material = material;
		meshFilter = gameObject.AddComponent<MeshFilter>();
	}

	// Refresh the mesh
	public void Refresh(Texture2D texture2D)
	{
		material.SetTexture("_Texture2D", texture2D);
		VoxelMeshFactory.SetTextureData(texturesBlockWidth, texture2D);
		meshFilter.mesh = VoxelMeshFactory.GenerateMesh(voxelGrid);
	}

	// Get voxel texture data
	public static VoxelTextureData GetTextureData(VoxelType voxelType)
	{
		return texturesData[(int)voxelType];
	}
}
