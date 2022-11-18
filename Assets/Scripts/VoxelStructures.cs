// Dependencies
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Different types of voxel structures
public enum StructureType
{
	Tree,
	Flower
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
				// Create grid
				voxelGrid = new VoxelGrid(3, 5, 3);

				// Write leaves
				voxelGrid.WriteVoxel(0, 4, 1, VoxelType.Leaves);
				voxelGrid.WriteVoxel(2, 4, 1, VoxelType.Leaves);
				voxelGrid.WriteVoxel(1, 4, 0, VoxelType.Leaves);
				voxelGrid.WriteVoxel(1, 4, 1, VoxelType.Leaves);
				voxelGrid.WriteVoxel(1, 4, 2, VoxelType.Leaves);
				for (int x = 0; x < 3; x++)
					for (int y = 2; y < 4; y++)
						for (int z = 0; z < 3; z++)
							voxelGrid.WriteVoxel(x, y, z, VoxelType.Leaves);

				// Write trunk
				voxelGrid.WriteVoxel(1, 0, 1, VoxelType.Wood);
				voxelGrid.WriteVoxel(1, 1, 1, VoxelType.Wood);
				voxelGrid.WriteVoxel(1, 2, 1, VoxelType.Wood);
				voxelGrid.WriteVoxel(1, 3, 1, VoxelType.Wood);

				// Return structure
				return voxelGrid;

			// Flower structure
			case StructureType.Flower:
				// Create grid
				voxelGrid = new VoxelGrid(1, 1, 1);

				// Write flower
				voxelGrid.WriteVoxel(0, 0, 0, VoxelType.Rose);

				// Return structure
				return voxelGrid;

			// Blueprint block
			default:
				// Create grid
				voxelGrid = new VoxelGrid(3, 3, 3);

				// Write blueprint cube
				for (int x = 0; x < 3; x++)
					for (int y = 0; y < 3; y++)
						for (int z = 0; z < 0; z++)
							voxelGrid.WriteVoxel(x, y, z, VoxelType.Blueprint);
				
				// Return structure
				return voxelGrid;
		}
	}
}
