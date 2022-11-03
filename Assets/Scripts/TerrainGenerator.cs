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
	private List<Layer> layers = new List<Layer> { new Layer(VoxelType.Blueprint, 64) };

	// Class settings
	private const int surfaceVariation = 10;
	private const int surfaceStartHeight = 16;

	// Start is called before the first frame update
	private void Start()
	{
		// Get components
		voxelGrid = GetComponent<VoxelGrid>();

		// Temporary layer settings
		layers.Clear();
		layers.Add(new Layer(VoxelType.Grass, 2));
		layers.Add(new Layer(VoxelType.Dirt, 3));
		layers.Add(new Layer(VoxelType.Stone, 64));
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
				voxelGrid.AddVoxel(new Vector3Int(x, y, z), new Voxel(layers[0].voxelType));

				// Set some things up for depth
				int depth = 1;
				int layerIndex = 0;
				y--;

				// Loop through lower blocks
				while (y >= 0)
				{
					// Check if we need to switch layers
					depth++;
					if (depth > layers[layerIndex].depth && layerIndex < layers.Count - 1)
					{
						layerIndex++;
						depth = 1;
					}

					// Add voxel and move to next level
					voxelGrid.AddVoxel(
						new Vector3Int(x, y, z),
						new Voxel(layers[layerIndex].voxelType)
					);
					y--;
				}
			}
		}

		// Apply changes
		voxelGrid.GenerateMesh();
	}

	// Layer properties
	private struct Layer
	{
		// Struct variables
		public VoxelType voxelType;
		public int depth;

		// Constructor
		public Layer(VoxelType voxelType, int depth)
		{
			this.voxelType = voxelType;
			this.depth = depth;
		}
	}
}
