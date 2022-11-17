// Dependencies
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Renders a voxel grid
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class VoxelRenderer : MonoBehaviour
{
	// Inspector variables
	public VoxelGrid voxelGrid;
	public int texturesBlockWidth;
    public VoxelData[] voxelsData;

	// Class variables
    private Mesh mesh;
    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();
    private List<Vector2> uvCoordinates = new List<Vector2>();

	// Voxel corner positions
	[HideInInspector]
	public Vector3[] cornerOffsets = {
		new Vector3(0, 0, 0), // 0
		new Vector3(1, 0, 0), // 1
		new Vector3(1, 0, 1), // 2
		new Vector3(0, 0, 1), // 3
		new Vector3(0, 1, 0), // 4
		new Vector3(1, 1, 0), // 5
		new Vector3(1, 1, 1), // 6
		new Vector3(0, 1, 1), // 7
	};

	/* 
	 * Cube front:
	 * 4 ----- 5
	 * |       |
	 * |       |
	 * 0 ----- 1
	 * 
	 * Cube back:
	 * 7 ----- 6
	 * |       |
	 * |       |
	 * 3 ----- 2
	 */

	// Voxel face mappings
	private static class FaceCorners
	{
		public static int[] Left = { 3, 7, 0, 4 };
		public static int[] Right = { 1, 5, 2, 6 };
		public static int[] Top = { 4, 7, 5, 6 };
		public static int[] Bottom = { 3, 0, 2, 1 };
		public static int[] Front = { 0, 4, 1, 5 };
		public static int[] Back = { 2, 6, 3, 7 };
	}

	// Enum for determining quad side
	private enum VoxelSide
	{
		Left,
		Right,
		Top,
		Bottom,
		Front,
		Back
	}

	// Start is called before the first frame update
	private void Awake()
	{
		// Set mesh filter mesh
		mesh = new Mesh();
		mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; // increases max vertex count
		GetComponent<MeshFilter>().mesh = mesh;

		// Generate mesh if filled in
		if (voxelGrid != null) GenerateMesh();
	}

	// Generate the mesh
	public void GenerateMesh()
	{
		// Empty some variables
		vertices.Clear();
		triangles.Clear();
		uvCoordinates.Clear();

		// Loop through all dimensions
		for (int x = 0; x < voxelGrid.width; x++)
		{
			for (int y = 0; y < voxelGrid.height; y++)
			{
				for (int z = 0; z < voxelGrid.length; z++)
				{
					// Get the voxel
					VoxelType voxelType = voxelGrid.voxels[x, y, z];

					// If the voxel is air itself there is no reason to create a quad
					if (voxelType == VoxelType.Air) continue;

					// Get voxel position
					Vector3Int voxelPosition = new Vector3Int(x, y, z);

					// If there is air on a side of the voxel, place a quad
					if (x - 1 < 0 || voxelGrid.voxels[x - 1, y, z] == VoxelType.Air) AddQuad(voxelType, voxelPosition, VoxelSide.Left);
					if (x + 1 >= voxelGrid.width || voxelGrid.voxels[x + 1, y, z] == VoxelType.Air) AddQuad(voxelType, voxelPosition, VoxelSide.Right);
					if (y + 1 >= voxelGrid.height || voxelGrid.voxels[x, y + 1, z] == VoxelType.Air) AddQuad(voxelType, voxelPosition, VoxelSide.Top);
					if (y - 1 < 0 || voxelGrid.voxels[x, y - 1, z] == VoxelType.Air) AddQuad(voxelType, voxelPosition, VoxelSide.Bottom);
					if (z - 1 < 0 || voxelGrid.voxels[x, y, z - 1] == VoxelType.Air) AddQuad(voxelType, voxelPosition, VoxelSide.Front);
					if (z + 1 >= voxelGrid.length || voxelGrid.voxels[x, y, z + 1] == VoxelType.Air) AddQuad(voxelType, voxelPosition, VoxelSide.Back);
				}
			}
		}

		// Apply changes
		RefreshMesh();
	}

	// Refresh the mesh
	private void RefreshMesh()
	{
		mesh.Clear();
		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.uv = uvCoordinates.ToArray();
		mesh.RecalculateNormals();
	}

	// Add a quad to the triangles and vertices
	private void AddQuad(VoxelType voxelType, Vector3Int position, VoxelSide side)
	{
		// Set up
		int vertexStartingIndex = vertices.Count;

		// Get the UV coordinates for each side of the voxel
		int blockTypeIndex = (int)voxelType;
		VoxelData voxelData = voxelsData[blockTypeIndex];
		Vector2 uvSideCoordinates = (Vector2)voxelData.sideTextureCoordinates / texturesBlockWidth;
		Vector2 uvTopCoordinates = (Vector2)voxelData.topTextureCoordinates / texturesBlockWidth;
		Vector2 uvBottomCoordinates = (Vector2)voxelData.bottomTextureCoordinates / texturesBlockWidth;

		// Placeholder variables
		int[] faceCorners;
		Vector2 uvStartCoordinates;

		// Check which side
		switch (side)
		{
			case VoxelSide.Left:
				faceCorners = FaceCorners.Left;
				uvStartCoordinates = uvSideCoordinates;
				break;

			case VoxelSide.Right:
				faceCorners = FaceCorners.Right;
				uvStartCoordinates = uvSideCoordinates;
				break;

			case VoxelSide.Top:
				faceCorners = FaceCorners.Top;
				uvStartCoordinates = uvTopCoordinates;
				break;

			case VoxelSide.Bottom:
				faceCorners = FaceCorners.Bottom;
				uvStartCoordinates = uvBottomCoordinates;
				break;

			case VoxelSide.Front:
				faceCorners = FaceCorners.Front;
				uvStartCoordinates = uvSideCoordinates;
				break;

			case VoxelSide.Back:
				faceCorners = FaceCorners.Back;
				uvStartCoordinates = uvSideCoordinates;
				break;

			default: return;
		}

		// Add vertices
		foreach (int corner in faceCorners) vertices.Add(position + cornerOffsets[corner]);

		// Add triangles
		triangles.AddRange(new int[]
		{
			vertexStartingIndex + 0, vertexStartingIndex + 1, vertexStartingIndex + 2, // First triangle
			vertexStartingIndex + 3, vertexStartingIndex + 2, vertexStartingIndex + 1 // Second triangle
		});

		// Add UV coordinates
		float textureUnit = 1.0f / texturesBlockWidth;
		uvCoordinates.AddRange(new Vector2[]{
			uvStartCoordinates,
			uvStartCoordinates + new Vector2(0.0f, textureUnit),
			uvStartCoordinates + new Vector2(textureUnit, 0.0f),
			uvStartCoordinates + new Vector2(textureUnit, textureUnit),
		});
	}
}
