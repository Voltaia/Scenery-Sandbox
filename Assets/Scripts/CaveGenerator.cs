using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Generates caves on a voxel grid
public class CaveGenerator
{
	// Class variables
	private VoxelGrid voxelGrid;

	// Constructor
	public CaveGenerator(VoxelGrid voxelGrid)
	{
		// Fill in variables
		this.voxelGrid = voxelGrid;
	}

	// Write cave data to voxel grid
	public void WriteCaves(int seed)
	{
		// Get random values
		Random.InitState(seed);
		int caves = Random.Range(0, 40);

		// Loop through caves
		for (int cave = 1; cave <= caves; cave++)
		{
			// Get start location
			Vector3Int position = new Vector3Int(
				Random.Range(0, voxelGrid.Width),
				Random.Range(0, voxelGrid.Height),
				Random.Range(0, voxelGrid.Length)
			);

			// Get delta
			Vector3Int deltaPosition = new Vector3Int(
				Random.Range(-1, 1),
				Random.Range(-1, 1),
				Random.Range(-1, 1)
			);

			// Get length
			int length = Random.Range(16, 32);

			// Loop through length
			for (int currentLength = 0; currentLength < length; currentLength++)
			{
				voxelGrid.SetVoxel(position, new Voxel());
				position += deltaPosition;
			}
		}
	}
}
