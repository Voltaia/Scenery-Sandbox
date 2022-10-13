// All the types of voxels
public enum VoxelType
{
	Air,
	Blank
}

// A pretty cube
public class Voxel
{
	// Class variables
	public VoxelType type = VoxelType.Air;

	// Empty constructor
	public Voxel() { }

	// Constructor
	public Voxel(VoxelType type)
	{
		this.type = type;
	}
}