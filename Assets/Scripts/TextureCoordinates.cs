// Dependencies
using UnityEngine;

// Holds texture values for voxels
[CreateAssetMenu(fileName = "Blank Coordinates", menuName = "ScriptableObjects/Texture Coordinates", order = 1)]
public class TextureCoordinates : ScriptableObject
{
	public Vector2Int topTextureCoordinates;
	public Vector2Int sideTextureCoordinates;
	public Vector2Int bottomTextureCoordinates;
}