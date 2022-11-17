// Dependencies
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Generates flora on a voxel grid
public class FloraGenerator
{
    // Class variables
    private VoxelGrid voxelGrid;

	// Class settings
	private const int Spacing = 5;
	private const int MinimumFlora = 8;
	private const int MaximumFlora = 12;

	// Constructor
	public FloraGenerator(VoxelGrid voxelGrid)
	{
		// Fill in variables
		this.voxelGrid = voxelGrid;
	}

	// Write flora to grid
	public void WriteFlora(int seed)
	{
		// Set seed
		Random.InitState(seed);

		// Get random amount of flora
		int floraToPlace = Random.Range(MinimumFlora, MaximumFlora);

		// Loop through placements of flora
		int floraPlaced = 0;
		List<Vector3Int> spawnPositions = new List<Vector3Int>();
		while (floraPlaced < floraToPlace)
		{
			// Get random position
			int spawnX = Random.Range(0, voxelGrid.width);
			int spawnY = voxelGrid.height;
			int spawnZ = Random.Range(0, voxelGrid.length);

			// Check if reasonable distance from other spawn positions
			bool positionAcceptable = true;
			foreach (Vector3Int spawnPosition in spawnPositions)
			{
				if(VoxelGrid.GetVoxelDistance(
					spawnX, -1, spawnZ,
					spawnPosition.x, -1, spawnPosition.z
				) <= Spacing) positionAcceptable = false;
			}

			// Check if passed the first test
			if (positionAcceptable)
			{
				// Reset pass flag
				positionAcceptable = false;

				// Check if surface below is acceptable
				while (!positionAcceptable && spawnY >= 2)
				{
					// Move down
					spawnY--;

					// Check block below and set y as we go
					VoxelType voxelTypeBelow = voxelGrid.ReadVoxel(spawnX, spawnY - 1, spawnZ);
					if (voxelTypeBelow != VoxelType.Air && voxelTypeBelow != VoxelType.Stone) positionAcceptable = true;
				}
			}

			// Place if acceptable
			if (positionAcceptable)
			{
				// Place it
				voxelGrid.WriteVoxel(spawnX, spawnY, spawnZ, VoxelType.Wood);
				spawnPositions.Add(new Vector3Int(spawnX, spawnY, spawnZ));
				floraPlaced++;
			}
		}
	}
}
