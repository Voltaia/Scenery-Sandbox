// Dependencies
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A grid of voxels
public class VoxelGrid
{
	// Class variables
	public VoxelType[,,] voxels;
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
		{
			for (int y = 0; y < height; y++)
			{
				for (int z = 0; z < length; z++)
				{

					voxels[x, y, z] = VoxelType.Air;
				}
			}
		}
	}

	// Write a voxel with coordinates
	public void WriteVoxel(int x, int y, int z, VoxelType voxelType)
	{
		// Add voxel
		voxels[x, y, z] = voxelType;
	}
	public void WriteVoxel(Vector3Int position, VoxelType voxelType)
	{ WriteVoxel(position.x, position.y, position.z, voxelType); }

	// Write a sphere shape
	public void WriteSphere(int centerX, int centerY, int centerZ, int radiusPlus, VoxelType voxelType)
	{
		// Loop through the positions of the sphere
		for (int x = centerX - radiusPlus; x <= centerX + radiusPlus; x++)
		{
			for (int y = centerY - radiusPlus; y <= centerY + radiusPlus; y++)
			{
				for (int z = centerZ - radiusPlus; z <= centerZ + radiusPlus; z++)
				{
					// Check distance and write if within radius
					if (
						!IsOutOfBounds(x, y, z)
						&& GetVoxelDistance(centerX, centerY, centerZ, x, y, z) < radiusPlus
					) WriteVoxel(x, y, z, voxelType);
				}
			}
		}
	}
	public void WriteSphere(Vector3Int centerPosition, int radiusPlus, VoxelType voxelType)
	{ WriteSphere(centerPosition.x, centerPosition.y, centerPosition.z, radiusPlus, voxelType); }

	// Get distance between voxel positions
	public float GetVoxelDistance(float x1, float y1, float z1, float x2, float y2, float z2)
	{
		return Mathf.Sqrt(Mathf.Pow(x1 - x2, 2) + Mathf.Pow(y1 - y2, 2) + Mathf.Pow(z1 - z2, 2));
	}

	// Read a voxel
	public VoxelType ReadVoxel(Vector3Int position)
	{
		// Read voxel
		return voxels[position.x, position.y, position.z];
	}

	// Check if position is out of bounds
	public bool IsOutOfBounds(int x, int y, int z)
	{
		return x < 0 || x >= width
			|| y < 0 || y >= height
			|| z < 0 || z >= length;
	}
	public bool IsOutOfBounds(Vector3Int position)
	{ return IsOutOfBounds(position.x, position.y, position.z); }
}
