// All the types of voxels
public enum VoxelType
{
	Air,
	Blueprint,
	Grass,
	Dirt,
}

// A pretty cube
public class Voxel
{
	// Class variables
	public VoxelType type = VoxelType.Air;
	public int typeIndex = 0;

	// Empty constructor
	public Voxel() { }

	// Constructor
	public Voxel(VoxelType type)
	{
		this.type = type;
		typeIndex = (int)type;
	}
}