using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Generates caves on a voxel grid
public class CaveGenerator
{
	// Class variables
	private VoxelGrid voxelGrid;

	// Class settings
	private const float BranchChance = 0.025f;
	private const float MaxCurve = 5.0f;
	private const float AttemptBranchingPercentage = 0.15f;
	private const int Radius = 3;
	private const int MinimumCaves = 2;
	private const int MaximumCaves = 3;

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
		int caves = Random.Range(MinimumCaves, MaximumCaves);

		// Loop through caves
		for (int cave = 1; cave <= caves; cave++)
		{
			// Get start location
			float offsetFromSide = Radius;
			float startX = Random.Range(offsetFromSide, voxelGrid.width - offsetFromSide);
			float startZ = Random.Range(offsetFromSide, voxelGrid.length - offsetFromSide);

			// Get start height
			float startY = (float)voxelGrid.GetSurfaceY((int)startX, (int)startZ);

			// Initialize as vector
			Vector3 startPosition = new Vector3(startX, startY, startZ);

			// Get direction
			Vector3 direction = new Vector3(
				Random.Range(-1.0f, 1.0f),
				Random.Range(-0.25f, -1.0f), // We only want our starter cave to point down
				Random.Range(-1.0f, 1.0f)
			).normalized;

			// Write cave
			WriteCave(startPosition, direction);
		}
	}

	// Write a singular cave
	private void WriteCave(Vector3 startPosition, Vector3 direction)
	{
		// Get some random variables
		int length = Random.Range(voxelGrid.width / 2, voxelGrid.width);
		Vector3 curveDirection = new Vector3(
			Random.Range(-1.0f, 1.0f),
			Random.Range(-1.0f, 1.0f),
			Random.Range(-1.0f, 1.0f)
		).normalized;
		float curveAmount = Random.Range(-MaxCurve, MaxCurve);

		// Loop through length
		for (int currentLength = 0; currentLength < length; currentLength++)
		{
			// Convert float vector to int vector
			Vector3Int gridPosition = VectorFloatToInt(startPosition);

			// Quit creating cave if it has wondered out of bounds
			if (voxelGrid.IsOutOfBounds(gridPosition.x, gridPosition.y, gridPosition.z)) return;

			// Bore segment of cave
			voxelGrid.WriteSphere(gridPosition.x, gridPosition.y, gridPosition.z, Radius, VoxelType.Air);

			// Change cave position
			startPosition += direction;
			direction = Quaternion.AngleAxis(curveAmount, curveDirection) * direction;

			// Split cave
			bool canStartBranching = currentLength > length * AttemptBranchingPercentage;
			if (canStartBranching && Random.value < BranchChance)
			{
				// Get a direction for the new cave and generate it
				float forkAngle = Random.value < 0.5f ? -90.0f : 90.0f;
				Vector3 forkDirection = Quaternion.AngleAxis(forkAngle, Vector3.up) * direction;
				WriteCave(startPosition, forkDirection);
			}
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
