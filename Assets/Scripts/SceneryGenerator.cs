// Dependencies
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Generates a terrain
[RequireComponent(typeof(VoxelGrid))]
public class SceneryGenerator : MonoBehaviour
{
	// Inspector variables
	public bool generateNew;

	// Class variables
	private VoxelGrid voxelGrid;
	private TerrainGenerator terrainGenerator;

	// Start is called before the first frame update
	private void Start()
	{
		// Fill in variables
		voxelGrid = GetComponent<VoxelGrid>();
		terrainGenerator = new TerrainGenerator(voxelGrid, new List<TerrainGenerator.Layer>());
	}

	// Called every frame
	private void Update()
	{
		if (generateNew)
		{
			generateNew = false;
			Generate();
		}
	}

	// Generate a new terrain
	public void Generate()
	{
		// Wipe voxel grid
		voxelGrid.NewGrid();

		// Generate a seed
		int seed = Random.Range(0, 999);

		// Make changes to voxel grid
		terrainGenerator.WriteTerrain(seed);

		// Apply changes
		voxelGrid.GenerateMesh();
	}
}
