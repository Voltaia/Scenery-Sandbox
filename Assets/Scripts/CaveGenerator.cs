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
		int caves = Random.Range(1, 5);

		// Loop through caves
		for (int cave = 1; cave <= caves; cave++)
		{
			// Get start location
			Vector3 startPosition = new Vector3(
				Random.Range(0, voxelGrid.Width),
				Random.Range(0, voxelGrid.Height),
				Random.Range(0, voxelGrid.Length)
			);

			// Get direction
			Vector3 direction = new Vector3(
				Random.Range(-1.0f, 1.0f),
				Random.Range(-1.0f, 1.0f),
				Random.Range(-1.0f, 1.0f)
			);

			// Write cave
			WriteCave(startPosition, direction);
		}
	}

	// Write a singular cave
	private void WriteCave(Vector3 startPosition, Vector3 direction)
	{
		// Get some random variables
		int length = Random.Range(voxelGrid.Width / 2, voxelGrid.Width);
		Vector3 curve = new Vector3(
			Random.Range(-0.1f, 0.1f),
			Random.Range(-0.1f, 0.1f),
			Random.Range(-0.1f, 0.1f)
		);

		// Loop through length
		for (int currentLength = 0; currentLength < length; currentLength++)
		{
			voxelGrid.WriteSphere(VectorFloatToInt(startPosition), 3, VoxelType.Air);
			startPosition += direction;
			direction += curve;
		}
	}

	// Convert from vector float to vector int
	private Vector3Int VectorFloatToInt(Vector3 vector)
	{
		return new Vector3Int(
			Mathf.FloorToInt(vector.x),
			Mathf.FloorToInt(vector.y),
			Mathf.FloorToInt(vector.z)
		);
	}
}
