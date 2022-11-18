// Dependencies
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Generates flora on a voxel grid
public class FloraGenerator
{
	// Class variables
	private VoxelGrid voxelGrid;
	private List<Placement> forestPlacements = new List<Placement>();
	private List<Placement> floraPlacements = new List<Placement>();

	// Search failure settings
	private const int MinimumFailures = 10;
	private const float SearchFailureMultiplier = 2.5f;

	// Flora settings
	private const float ForestPadding = 2.0f;
	private const float ForestRadiusMinimum = 8.0f;
	private const float ForestRadiusMaximum = 12.0f;
	private const float TreePadding = 0.25f;

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
		forestPlacements.Clear();
		floraPlacements.Clear();

		// Get forests
		float forestRadiusAverage = ForestRadiusMinimum + (ForestRadiusMaximum - ForestRadiusMinimum);
		List<Vector3Int> forestPositions = GetPlacementPositions(
			ref forestPlacements,
			voxelGrid.width / 2, voxelGrid.length / 2,
			voxelGrid.surfaceWingspan, forestRadiusAverage + ForestPadding,
			false
		);

		// Place forests
		foreach (Vector3Int forestPosition in forestPositions)
		{
			float forestRadius = Random.Range(ForestRadiusMinimum, ForestRadiusMaximum);
			WriteForest(forestPosition.x, forestPosition.z, forestRadius);
		}
	}

	// Write forest
	private void WriteForest(int centerX, int centerZ, float radius)
	{
		// Find open spots
		VoxelGrid treeVoxelGrid = VoxelStructures.GetStructure(StructureType.Tree);
		float maxDimension = Mathf.Max(treeVoxelGrid.width, treeVoxelGrid.length) / 2.0f;
		float placementRadius = maxDimension + TreePadding;
		List<Vector3Int> placementPositions = GetPlacementPositions(ref floraPlacements, centerX, centerZ, radius, placementRadius, true);

		// Place the flora
		foreach (Vector3Int placementPosition in placementPositions)
		{
			// Place it
			voxelGrid.WriteVoxelGrid(
				treeVoxelGrid,
				placementPosition.x - (treeVoxelGrid.width / 2),
				placementPosition.y,
				placementPosition.z - (treeVoxelGrid.length / 2),
				false
			);
		}
	}

	// Get spots distributed in radius
	private List<Vector3Int> GetPlacementPositions(ref List<Placement> placementList, int centerX, int centerZ, float searchRadius, float separationRadius, bool onlyGrass)
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
			foreach (Placement placement in placementList)
			{
				float distance = VoxelGrid.GetVoxelDistance(
					openPosition.x, -1, openPosition.z,
					placement.position.x, -1, placement.position.z
				);
				if (distance <= separationRadius + placement.radius) failedDistanceCheck = true;
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
			if (onlyGrass && surfaceVoxelType != VoxelType.DripGrass && surfaceVoxelType != VoxelType.Grass)
			{
				// Failed surface check
				failures++;
				continue;
			}
			else openPosition.y++;

			// Passed all checks, add position
			openings.Add(openPosition);
			placementList.Add(new Placement(openPosition, separationRadius));
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
