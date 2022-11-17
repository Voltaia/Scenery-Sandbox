// Dependencies
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Different types of voxel structures
public enum StructureType
{
	Tree,
}

// Voxel grids with structures in them and crap
public static class VoxelStructures
{
	// Get a voxel structure
	public static VoxelGrid GetStructure(StructureType structureType)
	{
		// Initialize variable
		VoxelGrid voxelGrid;

		// Create the structure
		switch (structureType)
		{
			// Tree structure
			case StructureType.Tree:
				voxelGrid = new VoxelGrid(5, 7, 5);
				voxelGrid.WriteVoxel(2, 0, 2, VoxelType.Wood);
				voxelGrid.WriteVoxel(2, 1, 2, VoxelType.Wood);
				return voxelGrid;

			// Blueprint block
			default:
				voxelGrid = new VoxelGrid(3, 3, 3);
				for (int x = 0; x < 3; x++)
					for (int y = 0; y < 3; y++)
						for (int z = 0; z < 0; z++)
							voxelGrid.WriteVoxel(x, y, z, VoxelType.Blueprint);
				return voxelGrid;
		}
	}
}
