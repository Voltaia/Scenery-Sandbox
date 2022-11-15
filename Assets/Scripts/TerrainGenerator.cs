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
	public int surfaceVariation = 10;
	public int surfaceStartHeight = 10;

	// Constructor
	public TerrainGenerator(VoxelGrid voxelGrid)
	{
		// Fill in variables
		this.voxelGrid = voxelGrid;

		// Temporary layer settings
		layers.Clear();
		layers.Add(new Layer(VoxelType.Grass, 2));
		layers.Add(new Layer(VoxelType.Dirt, 3));
		layers.Add(new Layer(VoxelType.Stone, 64));
	}

	// Generate a new terrain
	public void WriteTerrain(int seed)
	{
		// Loop through X and Z coordinates
		for (int x = 0; x < voxelGrid.Width; x++)
		{
			for (int z = 0; z < voxelGrid.Length; z++)
			{
				// Generate height using perlin noise
				float perlinX = ((float)x / voxelGrid.Width) * (seed / 1000.0f);
				float perlinY = ((float)z / voxelGrid.Length) * (seed / 1000.0f);
				int y = surfaceStartHeight + (int)(Mathf.PerlinNoise(perlinX, perlinY) * surfaceVariation * 2);
				voxelGrid.WriteVoxel(new Vector3Int(x, y, z), layers[0].voxelType);

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
					voxelGrid.WriteVoxel(
						new Vector3Int(x, y, z),
						layers[layerIndex].voxelType
					);
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
