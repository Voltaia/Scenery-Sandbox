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

	// Write a voxel with a vector position
	public void WriteVoxel(Vector3Int position, VoxelType voxelType)
	{
		// Add voxel
		voxels[position.x, position.y, position.z] = voxelType;
	}

	// Write a voxel with coordinates
	public void WriteVoxel(int x, int y, int z, VoxelType voxelType)
	{
		// Add voxel
		voxels[x, y, z] = voxelType;
	}

	// Write a sphere shape
	public void WriteSphere(Vector3Int centerPosition, int radiusPlus, VoxelType voxelType)
	{
		// Loop through the positions of the sphere
		for (int x = centerPosition.x - radiusPlus; x <= centerPosition.x + radiusPlus; x++)
		{
			for (int y = centerPosition.y - radiusPlus; y <= centerPosition.y + radiusPlus; y++)
			{
				for (int z = centerPosition.z - radiusPlus; z <= centerPosition.z + radiusPlus; z++)
				{
					// Create vector for position
					Vector3Int currentPosition = new Vector3Int(x, y, z);

					// Check distance and write if within radius
					if (
						!IsOutOfBounds(currentPosition)
						&& Vector3Int.Distance(centerPosition, currentPosition) < radiusPlus
					) WriteVoxel(currentPosition, voxelType);
				}
			}
		}
	}

	// Read a voxel
	public VoxelType ReadVoxel(Vector3Int position)
	{
		// Read voxel
		return voxels[position.x, position.y, position.z];
	}

	// Check if position is out of bounds
	public bool IsOutOfBounds(Vector3Int position)
	{
		return position.x < 0 || position.x >= width
			|| position.y < 0 || position.y >= height
			|| position.z < 0 || position.z >= length;
	}
}
