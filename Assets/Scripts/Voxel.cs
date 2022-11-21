// Dependencies
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Every type of voxel
public enum VoxelType
{
	Air,
	Blank,
	Blueprint,
	Grass,
	DripGrass,
	Dirt,
	Stone,
	Leaves,
	Wood,
	Rose,
}

// A voxel
public class Voxel
{
	// Variables
	public VoxelType type;
	public Color32 color;

	// Constructor for blank
	public Voxel()
	{
		type = VoxelType.Air;
		color = Color.white;
	}

	// Constructor for type
	public Voxel(VoxelType type)
	{
		this.type = type;
		color = Color.white;
	}

	// Constructor for color
	public Voxel(Color32 color)
	{
		type = VoxelType.Blank;
		this.color = color;
	}
}
