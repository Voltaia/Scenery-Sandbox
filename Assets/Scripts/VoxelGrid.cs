// Dependencies
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Generates a voxel terrain
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class VoxelGrid : MonoBehaviour
{
	// Inspector variables
	public int texturesBlockWidth;
	public TextureCoordinates[] voxelTextures;

	// Class variables
	private Mesh mesh;
	private Voxel[][][] voxels;
	private List<Vector3> vertices = new List<Vector3>();
	private List<int> triangles = new List<int>();
	private List<Vector2> uvCoordinates = new List<Vector2>();

	// Class settings
	public readonly int Width = 32;
	public readonly int Height = 32;
	public readonly int Length = 32;

	// Voxel corner positions
	[HideInInspector] public Vector3[] cornerOffsets = {
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
		// Initialize voxel grid
		NewGrid();

		// Set mesh filter mesh
		mesh = new Mesh();
		mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; // increases max vertex count
		GetComponent<MeshFilter>().mesh = mesh;

		// Add starter voxels
		voxels[0][0][0] = new Voxel(VoxelType.Blueprint);
		voxels[1][0][0] = new Voxel(VoxelType.Blueprint);
		voxels[0][0][1] = new Voxel(VoxelType.Blueprint);
		voxels[0][1][0] = new Voxel(VoxelType.Blueprint);

		voxels[2][0][2] = new Voxel(VoxelType.Dirt);
		voxels[2][1][2] = new Voxel(VoxelType.Grass);

		// Create mesh
		GenerateMesh();
	}

	// Create a new, empty grid
	public void NewGrid()
	{
		// Loop through all dimensions and create air voxels
		voxels = new Voxel[Width][][];
		for (int x = 0; x < Width; x++)
		{
			voxels[x] = new Voxel[Height][];
			for (int y = 0; y < Height; y++)
			{
				voxels[x][y] = new Voxel[Length];
				for (int z = 0; z < Length; z++)
				{

					voxels[x][y][z] = new Voxel();
				}
			}
		}
	}

	// Write a voxel
	public void WriteVoxel(Vector3Int position, Voxel voxel)
	{
		// Exit if voxel is out of bounds
		if (IsOutOfBounds(position)) return;

		// Add voxel
		voxels[position.x][position.y][position.z] = voxel;
	}

	// Write a sphere shape
	public void WriteSphere(Vector3Int centerPosition, int radiusPlus, Voxel voxel)
	{
		// Loop through the positions of the sphere
		for (int x = centerPosition.x - radiusPlus; x <= centerPosition.x + radiusPlus; x++)
		{
			for (int y = centerPosition.y - radiusPlus; y <= centerPosition.y + radiusPlus; y++)
			{
				for (int z = centerPosition.z - radiusPlus; z <= centerPosition.z + radiusPlus; z++)
				{
					// Check distance
					Vector3Int currentPosition = new Vector3Int(x, y, z);
					if (Vector3Int.Distance(centerPosition, currentPosition) < radiusPlus)
						WriteVoxel(currentPosition, voxel);
				}
			}
		}
	}

	// Check if position is out of bounds
	public bool IsOutOfBounds(Vector3Int position)
	{
		return position.x < 0 || position.x >= Width
			|| position.y < 0 || position.y >= Height
			|| position.z < 0 || position.z >= Length;
	}

	// Generate the mesh
	public void GenerateMesh()
	{
		// Empty some variables
		vertices.Clear();
		triangles.Clear();
		uvCoordinates.Clear();

		// Loop through all dimensions
		for (int x = 0; x < Width; x++)
		{
			for (int y = 0; y < Height; y++)
			{
				for (int z = 0; z < Length; z++)
				{
					// Get the voxel
					Voxel voxel = voxels[x][y][z];

					// If the voxel is air itself there is no reason to create a quad
					if (voxel.type == VoxelType.Air) continue;

					// Get voxel position
					Vector3Int voxelPosition = new Vector3Int(x, y, z);

					// If there is air on a side of the voxel, place a quad
					if (x - 1 < 0 || voxels[x - 1][y][z].type == VoxelType.Air) AddQuad(voxel, voxelPosition, VoxelSide.Left);
					if (x + 1 >= Width || voxels[x + 1][y][z].type == VoxelType.Air) AddQuad(voxel, voxelPosition, VoxelSide.Right);
					if (y + 1 >= Height || voxels[x][y + 1][z].type == VoxelType.Air) AddQuad(voxel, voxelPosition, VoxelSide.Top);
					if (y - 1 < 0 || voxels[x][y - 1][z].type == VoxelType.Air) AddQuad(voxel, voxelPosition, VoxelSide.Bottom);
					if (z - 1 < 0 || voxels[x][y][z - 1].type == VoxelType.Air) AddQuad(voxel, voxelPosition, VoxelSide.Front);
					if (z + 1 >= Length || voxels[x][y][z + 1].type == VoxelType.Air) AddQuad(voxel, voxelPosition, VoxelSide.Back);
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
	private void AddQuad(Voxel voxel, Vector3Int position, VoxelSide side)
	{
		// Set up
		int vertexStartingIndex = vertices.Count;

		// Get the UV coordinates for each side of the voxel
		int blockTypeIndex = voxel.typeIndex;
		TextureCoordinates voxelTexture = voxelTextures[blockTypeIndex];
		Vector2 uvSideCoordinates = (Vector2)voxelTexture.sideTextureCoordinates / texturesBlockWidth;
		Vector2 uvTopCoordinates = (Vector2)voxelTexture.topTextureCoordinates / texturesBlockWidth;
		Vector2 uvBottomCoordinates = (Vector2)voxelTexture.bottomTextureCoordinates / texturesBlockWidth;

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
