// Dependencies
using UnityEngine;

// Holds texture values for voxels
[CreateAssetMenu(fileName = "Blank Voxel Data", menuName = "ScriptableObjects/Voxel Data")]
public class VoxelData : ScriptableObject
{
	public Vector2Int topTextureCoordinates;
	public Vector2Int sideTextureCoordinates;
	public Vector2Int bottomTextureCoordinates;
}