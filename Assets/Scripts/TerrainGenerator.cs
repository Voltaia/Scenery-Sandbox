// Dependencies
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Generates a terrain
public class TerrainGenerator
{
	// Class variables
	private VoxelGrid voxelGrid;
	private List<Layer> layers = new List<Layer> { new Layer(VoxelType.Blueprint, 64) };

	// Class settings
	private const int SurfaceVariation = 10;
	private const int SurfaceStartHeight = 24;

	// Constructor
	public TerrainGenerator(VoxelGrid voxelGrid)
	{
		// Fill in variables
		this.voxelGrid = voxelGrid;

		// Temporary layer settings
		layers.Clear();
		layers.Add(new Layer(VoxelType.DripGrass, 1));
		layers.Add(new Layer(VoxelType.Dirt, 3));
		layers.Add(new Layer(VoxelType.Stone, 64));
	}

	// Generate a new terrain
	public void WriteTerrain(int seed)
	{
		// Set seed
		Random.InitState(seed);
		float perlinStartX = Random.Range(0, 1000);
		float perlinStartY = Random.Range(0, 1000);

		// Loop through X and Z coordinates
		for (int x = 0; x < voxelGrid.width; x++)
		{
			for (int z = 0; z < voxelGrid.length; z++)
			{
				// Generate height using perlin noise
				float scaledX = ((float)x / voxelGrid.width);
				float scaledZ = ((float)z / voxelGrid.length);
				int y = SurfaceStartHeight + (int)(Mathf.PerlinNoise(perlinStartX + scaledX, perlinStartY + scaledZ) * SurfaceVariation * 2);
				voxelGrid.WriteVoxel(x, y, z, new Voxel(layers[0].voxelType));

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
					voxelGrid.WriteVoxel(x, y, z, new Voxel(layers[layerIndex].voxelType));
					y--;
				}
			}
		}
	}

	// Layer properties
	public struct Layer
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
