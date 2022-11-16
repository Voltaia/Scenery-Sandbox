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
			Vector3Int newSpawnPosition = new Vector3Int(Random.Range(0, voxelGrid.width), -1, Random.Range(0, voxelGrid.length));

			// Check if reasonable distance from other spawn positions
			bool positionAcceptable = true;
			foreach (Vector3Int spawnPosition in spawnPositions)
			{
				if (Vector3Int.Distance(
					newSpawnPosition,
					new Vector3Int(spawnPosition.x, -1, spawnPosition.z)
				) <= Spacing) positionAcceptable = false;
			}

			// Check if passed the first test
			if (positionAcceptable)
			{
				// Reset pass flag
				positionAcceptable = false;

				// Check if surface below is acceptable
				newSpawnPosition.y = voxelGrid.height;
				while (!positionAcceptable && newSpawnPosition.y >= 2)
				{
					// Move down
					newSpawnPosition.y--;

					// Check block below and set y as we go
					VoxelType voxelTypeBelow = voxelGrid.ReadVoxel(newSpawnPosition + Vector3Int.down);
					if (voxelTypeBelow != VoxelType.Air && voxelTypeBelow != VoxelType.Stone) positionAcceptable = true;
				}
			}

			// Place if acceptable
			if (positionAcceptable)
			{
				// Place it
				voxelGrid.WriteVoxel(newSpawnPosition, VoxelType.Wood);
				spawnPositions.Add(newSpawnPosition);
				floraPlaced++;
			}
		}
	}
}
