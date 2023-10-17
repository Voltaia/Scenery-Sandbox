// Dependencies
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A grid of voxels
public class VoxelGrid
{
	// Class variables
	private Voxel[,,] voxels;
	public readonly int width;
	public readonly int height;
	public readonly int length;
	public readonly float diagonalSize;

	// Constructor
	public VoxelGrid(int width, int height, int length)
	{
		// Apply variables
		this.width = width;
		this.height = height;
		this.length = length;
		diagonalSize = Mathf.Sqrt(Mathf.Pow(width, 2) + Mathf.Pow(length, 2)) / 2;

		// Create a new grid
		NewGrid();
	}

	// Create a new, empty grid
	public void NewGrid()
	{
		// Loop through all dimensions and create air voxels
		voxels = new Voxel[width, height, length];
		for (int x = 0; x < width; x++)
			for (int y = 0; y < height; y++)
				for (int z = 0; z < length; z++)
					voxels[x, y, z] = new Voxel();
	}

	// Write a voxel with coordinates
	public void WriteVoxel(int x, int y, int z, Voxel voxel)
	{
		// Add voxel
		voxels[x, y, z] = voxel;
	}

	// Write a sphere shape
	public void WriteSphere(int centerX, int centerY, int centerZ, int radiusPlus, Voxel voxel)
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
					) WriteVoxel(x, y, z, voxel);
				}
	}

	// Write a different voxel grid to this voxel grid
	public void WriteVoxelGrid(VoxelGrid voxelGrid, int cornerX, int cornerY, int cornerZ, bool overwriteSolids)
	{
		// Check if it will be out of bounds
		if (
			IsOutOfBounds(cornerX, cornerY, cornerZ)
			|| IsOutOfBounds(cornerX + voxelGrid.width, cornerY + voxelGrid.height, cornerZ + voxelGrid.length)
		) return;

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
					if (overwriteSolids || ReadVoxel(writeX, writeY, writeZ).type == VoxelType.Air)
						WriteVoxel(writeX, writeY, writeZ, voxelGrid.ReadVoxel(copyX, copyY, copyZ));
				}
	}

	// Get distance between voxel positions
	public static float GetVoxelDistance(float x1, float y1, float z1, float x2, float y2, float z2)
	{
		return Mathf.Sqrt(Mathf.Pow(x1 - x2, 2) + Mathf.Pow(y1 - y2, 2) + Mathf.Pow(z1 - z2, 2));
	}

	// Read a voxel
	public Voxel ReadVoxel(int x, int y, int z)
	{
		// Read voxel
		return voxels[x, y, z];
	}

	// Get surface at location
	public int GetSurfaceY(int x, int z)
	{
		// Loop until surface is found
		int surfaceY = height;
		bool surfaceFound = false;
		while (!surfaceFound && surfaceY > 0)
		{
			// Scan down
			surfaceY--;

			// Check block below and set y as we go
			Voxel voxel = ReadVoxel(x, surfaceY, z);
			if (voxel.type != VoxelType.Air) surfaceFound = true;
		}

		// Return surface height
		return surfaceY;
	}

	// Check if position is out of bounds
	public bool IsOutOfBounds(int x, int y, int z)
	{
		return x < 0 || x >= width
			|| y < 0 || y >= height
			|| z < 0 || z >= length;
	}
}
