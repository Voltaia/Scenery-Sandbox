// Dependencies
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A grid of voxels
public class VoxelGrid
{
	// Class variables
	private VoxelType[,,] voxels;
	public readonly int width;
	public readonly int height;
	public readonly int length;

	// Constructor
	public VoxelGrid(int width, int height, int length)
	{
		// Apply variables
		this.width = width;
		this.height = height;
		this.length = length;

		// Create a new grid
		NewGrid();
	}

	// Create a new, empty grid
	public void NewGrid()
	{
		// Loop through all dimensions and create air voxels
		voxels = new VoxelType[width, height, length];
		for (int x = 0; x < width; x++)
			for (int y = 0; y < height; y++)
				for (int z = 0; z < length; z++)
					voxels[x, y, z] = VoxelType.Air;
	}

	// Write a voxel with coordinates
	public void WriteVoxel(int x, int y, int z, VoxelType voxelType)
	{
		// Add voxel
		voxels[x, y, z] = voxelType;
	}

	// Write a sphere shape
	public void WriteSphere(int centerX, int centerY, int centerZ, int radiusPlus, VoxelType voxelType)
	{
		// Loop through the positions of the sphere
		for (int x = centerX - radiusPlus; x <= centerX + radiusPlus; x++)
			for (int y = centerY - radiusPlus; y <= centerY + radiusPlus; y++)
				for (int z = centerZ - radiusPlus; z <= centerZ + radiusPlus; z++)
				{
					// Check distance and write if within radius
					if (
						!IsOutOfBounds(x, y, z)
						&& GetVoxelDistance(centerX, centerY, centerZ, x, y, z) < radiusPlus
					) WriteVoxel(x, y, z, voxelType);
				}
	}

	// Write a different voxel grid to this voxel grid
	public void WriteVoxelGrid(VoxelGrid voxelGrid, int cornerX, int cornerY, int cornerZ, bool overwriteSolids)
	{
		// Loop through dimension
		for (int copyX = 0; copyX < voxelGrid.width; copyX++)
			for (int copyY = 0; copyY < voxelGrid.height; copyY++)
				for (int copyZ = 0; copyZ < voxelGrid.length; copyZ++)
				{
					// Get coordinates to copy to
					int writeX = cornerX + copyX;
					int writeY = cornerY + copyY;
					int writeZ = cornerZ + copyZ;

					// Copy
					if (IsOutOfBounds(writeX, writeY, writeZ)) continue;
					if (!overwriteSolids && ReadVoxel(writeX, writeY, writeZ) != VoxelType.Air) continue;
					WriteVoxel(writeX, writeY, writeZ, voxelGrid.ReadVoxel(copyX, copyY, copyZ));
				}
	}

	// Get distance between voxel positions
	public static float GetVoxelDistance(float x1, float y1, float z1, float x2, float y2, float z2)
	{
		return Mathf.Sqrt(Mathf.Pow(x1 - x2, 2) + Mathf.Pow(y1 - y2, 2) + Mathf.Pow(z1 - z2, 2));
	}

	// Read a voxel
	public VoxelType ReadVoxel(int x, int y, int z)
	{
		// Read voxel
		return voxels[x, y, z];
	}

	// Check if position is out of bounds
	public bool IsOutOfBounds(int x, int y, int z)
	{
		return x < 0 || x >= width
			|| y < 0 || y >= height
			|| z < 0 || z >= length;
	}
}
