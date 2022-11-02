// Dependencies
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Generates a terrain
[RequireComponent(typeof(VoxelGrid))]
public class TerrainGenerator : MonoBehaviour
{
	// Inspector variables
	public bool generateNew;

	// Class variables
	private VoxelGrid voxelGrid;

	// Class settings
	private const int surfaceVariation = 10;
	private const int surfaceStartHeight = 10;

	// Start is called before the first frame update
	private void Start()
	{
		// Get components
		voxelGrid = GetComponent<VoxelGrid>();
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

		// Loop through X and Z coordinates
		for (int x = 0; x < voxelGrid.Width; x++)
		{
			for (int z = 0; z < voxelGrid.Length; z++)
			{
				// Generate height using perlin noise
				float perlinX = ((float)x / voxelGrid.Width) * (seed / 1000.0f);
				float perlinY = ((float)z / voxelGrid.Length) * (seed / 1000.0f);
				int y = surfaceStartHeight + (int)(Mathf.PerlinNoise(perlinX, perlinY) * surfaceVariation * 2);
				voxelGrid.AddVoxel(new Vector3Int(x, y, z), new Voxel(VoxelType.Grass));

				// Add lower blocks
				y--;
				while (y >= 0)
				{
					voxelGrid.AddVoxel(new Vector3Int(x, y, z), new Voxel(VoxelType.Dirt));
					y--;
				}
			}
		}

		// Apply changes
		voxelGrid.GenerateMesh();
	}
}
