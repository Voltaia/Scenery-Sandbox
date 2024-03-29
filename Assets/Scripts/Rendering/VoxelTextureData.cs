// Dependencies
using UnityEngine;

// Voxel rendering methods
public enum RenderMethod
{
	None,
	Standard,
	Transparent,
	Decoration
}

// Holds texture values for voxels
[CreateAssetMenu(fileName = "Voxel Texture Data", menuName = "Voxel Texture Data")]
public class VoxelTextureData : ScriptableObject
{
	public Vector2Int topTextureCoordinates;
	public Vector2Int sideTextureCoordinates;
	public Vector2Int bottomTextureCoordinates;
	public RenderMethod renderMethod;
}