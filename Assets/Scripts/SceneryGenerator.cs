// Dependencies
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Generates a terrain
[RequireComponent(typeof(VoxelGrid))]
public class SceneryGenerator : MonoBehaviour
{
	// Inspector variables
	[Header("Generation")]
	public bool generateNew = true;
	public bool randomizeSeed = true;
	public int seed;

	[Header("Options")]
	public bool generateTerrain = true;
	public bool generateCaves = true;

	// Class variables
	private VoxelGrid voxelGrid;
	private TerrainGenerator terrainGenerator;
	private CaveGenerator caveGenerator;

	// Start is called before the first frame update
	private void Start()
	{
		// Fill in variables
		voxelGrid = GetComponent<VoxelGrid>();
		terrainGenerator = new TerrainGenerator(voxelGrid);
		caveGenerator = new CaveGenerator(voxelGrid);
	}

	// Called every frame
	private void Update()
	{
		// Check if we're meant to generate a new scenery this frame
		if (generateNew)
		{
			// Generate a new scenery
			if (randomizeSeed) RandomizeSeed();
			Generate();

			// Disable for next frame
			generateNew = false;
		}
	}

	// Generate a new terrain
	public void Generate()
	{
		// Wipe voxel grid
		voxelGrid.NewGrid();

		// Make changes to voxel grid
		if (generateTerrain) terrainGenerator.WriteTerrain(seed);
		if (generateCaves) caveGenerator.WriteCaves(seed);

		// Apply changes
		voxelGrid.GenerateMesh();
	}

	// Randomize seed
	private void RandomizeSeed()
	{
		seed = Random.Range(0, 999);
	}
}
