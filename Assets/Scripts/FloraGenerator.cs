// Dependencies
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Generates flora on a voxel grid
public class FloraGenerator
{
	// Class variables
	private VoxelGrid voxelGrid;
	private List<Placement> placements = new List<Placement>();

	// Class settings
	private const float TreeSeparation = 1;
	private const int MinimumFailures = 10;
	private const float SearchFailureMultiplier = 2.5f;

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

		// Reset placements
		placements.Clear();

		// Find a spot
		VoxelGrid voxelGridToPlace = VoxelStructures.GetStructure(StructureType.Tree);
		float gridStructureRadius = Mathf.Sqrt(Mathf.Pow(voxelGridToPlace.width, 2) + Mathf.Pow(voxelGridToPlace.length, 2)) / 2.0f;
		float gridPlacementRadius = gridStructureRadius + TreeSeparation;
		List<Vector3Int> placementPositions = GetPlacementPositions(voxelGrid.width / 2, voxelGrid.length / 2, 64, gridPlacementRadius);

		// Place the flora
		foreach (Vector3Int placementPosition in placementPositions)
		{
			// Place it
			voxelGrid.WriteVoxelGrid(
				voxelGridToPlace,
				placementPosition.x - (voxelGridToPlace.width / 2),
				placementPosition.y,
				placementPosition.z - (voxelGridToPlace.length / 2),
				false
			);
		}
	}

	// Get spots distributed in radius
	private List<Vector3Int> GetPlacementPositions(int centerX, int centerZ, float searchRadius, float separationRadius)
	{
		// Look for spots to place
		List<Vector3Int> openings = new List<Vector3Int>();
		int failures = 0;
		while (failures - MinimumFailures < openings.Count * SearchFailureMultiplier)
		{
			// Get random position
			Vector3Int openPosition = new Vector3Int();
			do
			{
				openPosition.x = (int)(centerX + Mathf.Cos(Random.value * Mathf.PI * 2) * Random.value * searchRadius);
				openPosition.z = (int)(centerZ + Mathf.Sin(Random.value * Mathf.PI * 2) * Random.value * searchRadius);
			} while (voxelGrid.IsOutOfBounds(openPosition.x, 0, openPosition.z));

			// Check if reasonable distance from other spawn positions
			bool failedDistanceCheck = false;
			foreach (Placement placement in placements)
			{
				if (VoxelGrid.GetVoxelDistance(
					openPosition.x, -1, openPosition.z,
					placement.position.x, -1, placement.position.z
				) <= Mathf.Ceil(separationRadius)) failedDistanceCheck = true;
			}
			if (failedDistanceCheck)
			{
				// Failed distance check
				failures++;
				continue;
			}

			// Check if surface below is acceptable
			openPosition.y = voxelGrid.GetSurfaceY(openPosition.x, openPosition.z);
			VoxelType surfaceVoxelType = voxelGrid.ReadVoxel(openPosition.x, openPosition.y, openPosition.z);
			if (surfaceVoxelType != VoxelType.DripGrass && surfaceVoxelType != VoxelType.Grass)
			{
				// Failed surface check
				failures++;
				continue;
			}
			else openPosition.y++;

			// Passed all checks, add position
			openings.Add(openPosition);
			placements.Add(new Placement(openPosition, separationRadius));
		}

		// Return open positions
		return openings;
	}

	// Placements
	private struct Placement
	{
		// Struct variables
		public Vector3Int position;
		public float radius;

		// Constructor
		public Placement(Vector3Int position, float radius)
		{
			this.position = position;
			this.radius = radius;
		}
	}
}
