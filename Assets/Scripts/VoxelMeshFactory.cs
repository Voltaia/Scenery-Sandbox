// Dependencies
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Generates voxel meshes
public class VoxelMeshFactory
{
	// Class variables
	private VoxelGrid voxelGrid;
	private int texturesBlockWidth;
	private VoxelData[] voxelsData;
	private Mesh mesh = new Mesh();
	private List<Vector3> vertices = new List<Vector3>();
	private List<int> triangles = new List<int>();
	private List<Vector2> uvCoordinates = new List<Vector2>();

	// Voxel corner positions
	public static Vector3[] cornerOffsets = {
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

	// Constructor
	public VoxelMeshFactory(VoxelGrid voxelGrid, int texturesBlockWidth, VoxelData[] voxelsData)
	{
		// Fill in variables
		this.voxelGrid = voxelGrid;
		this.texturesBlockWidth = texturesBlockWidth;
		this.voxelsData = voxelsData;

		// Increase max vertex count
		mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
	}

	// Generate the mesh
	public Mesh GenerateMesh()
	{
		// Empty some variables
		vertices.Clear();
		triangles.Clear();
		uvCoordinates.Clear();

		// Loop through all dimensions
		for (int x = 0; x < voxelGrid.width; x++)
			for (int y = 0; y < voxelGrid.height; y++)
				for (int z = 0; z < voxelGrid.length; z++)
				{
					// Get the voxel
					VoxelType voxelType = voxelGrid.ReadVoxel(x, y, z);
					VoxelData voxelData = voxelsData[(int)voxelType];

					// Render methods
					switch (voxelData.renderMethod)
					{
						// Standard
						case RenderMethod.Standard:
							// Add a normal quad cube
							AddQuadCube(x, y, z, voxelType, false);
							break;

						// Transparent
						case RenderMethod.Transparent:
							// Add a normal quad cube
							AddQuadCube(x, y, z, voxelType, false);

							// Add an inverted quad cube
							bool hasTransparency = voxelsData[(int)voxelType].renderMethod == RenderMethod.Transparent;
							if (hasTransparency) AddQuadCube(x, y, z, voxelType, true);
							break;
					}
				}

		// Apply changes
		RefreshMesh();

		// Return mesh
		return mesh;
	}

	// Add a quad cube
	private void AddQuadCube(int x, int y, int z, VoxelType voxelType, bool invertFaces)
	{
		// Get voxel position TEMPORARY REPLACE VECTOR3INTS
		Vector3Int voxelPosition = new Vector3Int(x, y, z);

		// Check left
		if (CanPlaceQuadInDirection(x - 1, y, z))
			AddQuad(voxelPosition, voxelType, VoxelSide.Left, invertFaces);

		// Check right
		if (CanPlaceQuadInDirection(x + 1, y, z))
			AddQuad(voxelPosition, voxelType, VoxelSide.Right, invertFaces);

		// Check up
		if (CanPlaceQuadInDirection(x, y + 1, z))
			AddQuad(voxelPosition, voxelType, VoxelSide.Top, invertFaces);

		// Check down
		if (CanPlaceQuadInDirection(x, y - 1, z))
			AddQuad(voxelPosition, voxelType, VoxelSide.Bottom, invertFaces);

		// Check backwards
		if (CanPlaceQuadInDirection(x, y, z - 1))
			AddQuad(voxelPosition, voxelType, VoxelSide.Front, invertFaces);

		// Check forwards
		if (CanPlaceQuadInDirection(x, y, z + 1))
			AddQuad(voxelPosition, voxelType, VoxelSide.Back, invertFaces);
	}

	// Makes several checks to see if it is okay to place a quad
	private bool CanPlaceQuadInDirection(int adjacentX, int adjacentY, int adjacentZ)
	{
		// Check if edge of grid
		bool edgeOfGrid = voxelGrid.IsOutOfBounds(adjacentX, adjacentY, adjacentZ);
		if (edgeOfGrid) return true;

		// Make checks for open air
		VoxelType adjacentVoxelType = voxelGrid.ReadVoxel(adjacentX, adjacentY, adjacentZ);
		VoxelData adjacentVoxelData = voxelsData[(int)adjacentVoxelType];
		bool openAir = adjacentVoxelData.renderMethod == RenderMethod.None;
		bool adjacentVoxelTransparency = adjacentVoxelData.renderMethod == RenderMethod.Transparent;
		return openAir || adjacentVoxelTransparency;
	}

	// Add a quad to the triangles and vertices
	private void AddQuad(Vector3Int position, VoxelType voxelType, VoxelSide side, bool invertFace)
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

		// Check if face is inverted
		if (!invertFace)
		{
			// Add triangles on the outside
			triangles.AddRange(new int[]
			{
				vertexStartingIndex + 0, vertexStartingIndex + 1, vertexStartingIndex + 2, // First triangle
				vertexStartingIndex + 3, vertexStartingIndex + 2, vertexStartingIndex + 1 // Second triangle
			});
		}
		else
		{
			// Add triangles on the inside
			triangles.AddRange(new int[]
			{
				vertexStartingIndex + 2, vertexStartingIndex + 1, vertexStartingIndex + 0, // First triangle
				vertexStartingIndex + 1, vertexStartingIndex + 2, vertexStartingIndex + 3 // Second triangle
			});
		}

		// Add UV coordinates
		float textureUnit = 1.0f / texturesBlockWidth;
		uvCoordinates.AddRange(new Vector2[]{
			uvStartCoordinates,
			uvStartCoordinates + new Vector2(0.0f, textureUnit),
			uvStartCoordinates + new Vector2(textureUnit, 0.0f),
			uvStartCoordinates + new Vector2(textureUnit, textureUnit),
		});
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
}
