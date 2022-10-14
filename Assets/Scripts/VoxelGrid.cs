// Dependencies
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Generates a voxel terrain
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class VoxelGrid : MonoBehaviour
{
	// Class variables
	private Mesh mesh;
	private Voxel[][][] voxels;
	private List<Vector3> vertices = new List<Vector3>();
	private List<int> triangles = new List<int>();
	private List<Vector2> uvCoordinates = new List<Vector2>();

	// Class settings
	public readonly static int Width = 16;
	public readonly static int Height = 16;
	public readonly static int Length = 16;

	// Voxel corner positions
	private Vector3[] cornerOffsets = {
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
	private void Start()
	{
		// Initialize voxel grid
		NewGrid();

		// Set mesh filter mesh
		mesh = new Mesh();
		GetComponent<MeshFilter>().mesh = mesh;

		// Add starter voxel
		voxels[0][0][0] = new Voxel(VoxelType.Blank);
		voxels[1][0][0] = new Voxel(VoxelType.Blank);
		voxels[0][0][1] = new Voxel(VoxelType.Blank);
		voxels[0][1][0] = new Voxel(VoxelType.Blank);

		// Create terrain
		GenerateTerrain();
		RefreshTerrain();
	}

	// Create a new, empty grid
	private void NewGrid()
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

	// Add a quad to the triangles and vertices
	private void AddQuad(Vector3 position, VoxelSide side)
	{
		// Set up
		int vertexStartingIndex = vertices.Count;

		// Some TEMPORARY variables
		Vector2 sideStart = new Vector2(0.5f, 0.5f);
		Vector2 topStart = new Vector2(0.0f, 0.0f);
		Vector2 bottomStart = new Vector2(0.5f, 0.0f);

		// Placeholder variables
		int[] faceCorners;
		Vector2 uvStartCoordinate;

		// Check which side
		switch (side)
		{
			case VoxelSide.Left:
				faceCorners = FaceCorners.Left;
				uvStartCoordinate = sideStart;
				break;

			case VoxelSide.Right:
				faceCorners = FaceCorners.Right;
				uvStartCoordinate = sideStart;
				break;

			case VoxelSide.Top:
				faceCorners = FaceCorners.Top;
				uvStartCoordinate = topStart;
				break;

			case VoxelSide.Bottom:
				faceCorners = FaceCorners.Bottom;
				uvStartCoordinate = bottomStart;
				break;

			case VoxelSide.Front:
				faceCorners = FaceCorners.Front;
				uvStartCoordinate = sideStart;
				break;

			case VoxelSide.Back:
				faceCorners = FaceCorners.Back;
				uvStartCoordinate = sideStart;
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
		uvCoordinates.AddRange(new Vector2[]{
			uvStartCoordinate,
			uvStartCoordinate + new Vector2(0.0f, 0.5f),
			uvStartCoordinate + new Vector2(0.5f, 0.0f),
			uvStartCoordinate + new Vector2(0.5f, 0.5f),
		});
	}

	// Generate the mesh
	private void GenerateTerrain()
	{
		// Loop through all dimensions
		for (int x = 0; x < Width; x++)
		{
			for (int y = 0; y < Height; y++)
			{
				for (int z = 0; z < Length; z++)
				{
					// If the voxel is air itself there is no reason to create a quad
					if (voxels[x][y][z].type == VoxelType.Air) continue;

					// Get voxel position
					Vector3 voxelPosition = new Vector3(x, y, z);

					// If there is air on a side of the voxel, place a quad
					if (x - 1 < 0 || voxels[x - 1][y][z].type == VoxelType.Air) AddQuad(voxelPosition, VoxelSide.Left);
					if (x + 1 >= Width || voxels[x + 1][y][z].type == VoxelType.Air) AddQuad(voxelPosition, VoxelSide.Right);
					if (y + 1 >= Height || voxels[x][y + 1][z].type == VoxelType.Air) AddQuad(voxelPosition, VoxelSide.Top);
					if (y - 1 < 0 || voxels[x][y - 1][z].type == VoxelType.Air) AddQuad(voxelPosition, VoxelSide.Bottom);
					if (z - 1 < 0 || voxels[x][y][z - 1].type == VoxelType.Air) AddQuad(voxelPosition, VoxelSide.Front);
					if (z + 1 >= Length || voxels[x][y][z + 1].type == VoxelType.Air) AddQuad(voxelPosition, VoxelSide.Back);
				}
			}
		}
	}

	// Refresh the mesh
	private void RefreshTerrain()
	{
		mesh.Clear();
		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.uv = uvCoordinates.ToArray();
		mesh.RecalculateNormals();
	}
}
