// Dependencies
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Generates a terrain
[RequireComponent(typeof(VoxelRenderer))]
public class SceneryGenerator : MonoBehaviour
{
	// Inspector variables
	[Header("Generation")]
	public bool generateNew = true;
	public bool randomizeSeed = true;
	[Range(0, 1000)]
	public int seed;

	[Header("Options")]
	public bool generateTerrain = true;
	public bool generateCaves = true;
	public bool generateFlora = true;

	// Class variables
	private VoxelGrid voxelGrid;
	private VoxelRenderer voxelRenderer;
	private TerrainGenerator terrainGenerator;
	private CaveGenerator caveGenerator;
	private FloraGenerator floraGenerator;

	// Start is called before the first frame update
	private void Awake()
	{
		// Create a new voxel grid
		voxelGrid = new VoxelGrid(48, 48, 48);

		// Fill in variables
		voxelRenderer = GetComponent<VoxelRenderer>();
		voxelRenderer.voxelGrid = voxelGrid;
		terrainGenerator = new TerrainGenerator(voxelGrid);
		caveGenerator = new CaveGenerator(voxelGrid);
		floraGenerator = new FloraGenerator(voxelGrid);
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
		// Reset voxel grid
		voxelGrid.NewGrid();

		// Make changes to voxel grid
		if (generateTerrain) terrainGenerator.WriteTerrain(seed);
		if (generateCaves) caveGenerator.WriteCaves(seed);
		if (generateFlora) floraGenerator.WriteFlora(seed);

		// Apply changes
		voxelRenderer.GenerateMesh();
	}

	// Randomize seed
	private void RandomizeSeed()
	{
		seed = Random.Range(0, 999);
	}
}
