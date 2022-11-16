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
	private const int MinimumSpacing = 5;
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
		// Get random amount of flora
		int floraToPlace = Random.Range(MinimumFlora, MaximumFlora);

		// Loop through placements of flora
		//int floraPlaced = 0;
	}
}
