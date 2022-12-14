// Dependencies
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// HEY
// LISTEN
// THIS CLASS NEEDS A GOOD, OLD FASHIONED, REFACTOR

// Generates voxel meshes
public class VoxelMeshFactory
{
	// Class variables
	private VoxelGrid voxelGrid;
	private int texturesBlockWidth;
	private VoxelTextureData[] voxelTexturesData;
	private Texture2D texture2D;
	private Color32[] pixels;
	private Mesh mesh = new Mesh();
	private List<Vector3> vertices = new List<Vector3>();
	private List<int> triangles = new List<int>();
	private List<Vector2> uv = new List<Vector2>();
	private List<Color32> colors32 = new List<Color32>();

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
	public VoxelMeshFactory(VoxelGrid voxelGrid, int texturesBlockWidth, VoxelTextureData[] voxelsData, Texture2D texture2D)
	{
		// Fill in variables
		this.voxelGrid = voxelGrid;
		this.texturesBlockWidth = texturesBlockWidth;
		this.voxelTexturesData = voxelsData;
		this.texture2D = texture2D;
		pixels = texture2D.GetPixels32();

		// Increase max vertex count
		mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
	}

	// Generate the mesh
	public Mesh GenerateMesh()
	{
		// Empty some variables
		vertices.Clear();
		triangles.Clear();
		uv.Clear();

		// Loop through all dimensions
		for (int x = 0; x < voxelGrid.width; x++)
			for (int y = 0; y < voxelGrid.height; y++)
				for (int z = 0; z < voxelGrid.length; z++)
				{
					// Get the voxel
					Voxel voxel = voxelGrid.ReadVoxel(x, y, z);
					VoxelTextureData voxelData = voxelTexturesData[(int)voxel.type];

					// Render methods
					switch (voxelData.renderMethod)
					{
						// Standard
						case RenderMethod.Standard:
							// Add a normal quad cube
							AddQuadCube(x, y, z, voxel, false);
							break;

						// Transparent
						case RenderMethod.Transparent:
							// Add a normal quad cube
							AddQuadCube(x, y, z, voxel, false);

							// Add an inverted quad cube
							AddQuadCube(x, y, z, voxel, true);
							break;

						// Decoration
						case RenderMethod.Decoration:
							// Add a decoration
							AddDecoration(x, y, z, voxel);
							break;
					}
				}

		// Apply changes
		RefreshMesh();

		// Return mesh
		return mesh;
	}

	// Add decoration
	private void AddDecoration(int x, int y, int z, Voxel voxel)
	{
		// Get texture map width
		int texturesPixelWidth = (int)Mathf.Sqrt(pixels.Length);
		int textureWidth = texturesPixelWidth / texturesBlockWidth;

		// Create voxel grid
		VoxelGrid decorationVoxelGrid = new VoxelGrid(textureWidth, textureWidth, textureWidth);

		// Get texture position
		VoxelTextureData voxelData = voxelTexturesData[(int)voxel.type];
		int cursorStartX = voxelData.sideTextureCoordinates.x * textureWidth;
		int cursorStartY = voxelData.sideTextureCoordinates.y * textureWidth;
		for (int cursorX = cursorStartX; cursorX < cursorStartX + textureWidth; cursorX++)
			for (int cursorY = cursorStartY; cursorY < cursorStartY + textureWidth; cursorY++)
			{
				int pixelIndex = cursorX + cursorY * texturesPixelWidth;
				Color32 color = pixels[pixelIndex];
				Vector3Int writePosition = new Vector3Int(cursorX - cursorStartX, cursorY - cursorStartY, 8);
				if (color.a > 0.5f)
				{
					decorationVoxelGrid.WriteVoxel(writePosition.x, writePosition.y, writePosition.z, new Voxel(color));
					decorationVoxelGrid.WriteVoxel(writePosition.x, writePosition.y, writePosition.z - 1, new Voxel(color));
				}
			}

		// Generate the mesh
		VoxelMeshFactory voxelMeshFactory = new VoxelMeshFactory(decorationVoxelGrid, texturesBlockWidth, voxelTexturesData, texture2D);
		Mesh decorationMesh = voxelMeshFactory.GenerateMesh();

		// Grab, scale and transform the vertices
		List<Vector3> decorationVertices = new List<Vector3>();
		decorationVertices.AddRange(decorationMesh.vertices);
		for (int vertexIndex = 0; vertexIndex < decorationVertices.Count; vertexIndex++)
		{
			// Scale
			decorationVertices[vertexIndex] *= 0.0625f;

			// Offset accordingly
			decorationVertices[vertexIndex] += new Vector3(x, y, z);
		}
		
		// Grab and offset the triangles
		List<int> decorationTriangles = new List<int>();
		decorationTriangles.AddRange(decorationMesh.triangles);
		for (int triangleIndex = 0; triangleIndex < decorationTriangles.Count; triangleIndex++)
			decorationTriangles[triangleIndex] += vertices.Count;
		
		// Apply the decoration to the mesh
		vertices.AddRange(decorationVertices);
		triangles.AddRange(decorationTriangles);
		uv.AddRange(decorationMesh.uv);
		colors32.AddRange(decorationMesh.colors32);
	}

	// Add a quad cube
	private void AddQuadCube(int x, int y, int z, Voxel voxel, bool invertFaces)
	{
		// Get voxel position TEMPORARY REPLACE VECTOR3INTS
		Vector3Int voxelPosition = new Vector3Int(x, y, z);

		// Check left
		if (CanPlaceQuadInDirection(x - 1, y, z))
			AddQuad(voxelPosition, voxel, VoxelSide.Left, invertFaces);

		// Check right
		if (CanPlaceQuadInDirection(x + 1, y, z))
			AddQuad(voxelPosition, voxel, VoxelSide.Right, invertFaces);

		// Check up
		if (CanPlaceQuadInDirection(x, y + 1, z))
			AddQuad(voxelPosition, voxel, VoxelSide.Top, invertFaces);

		// Check down
		if (CanPlaceQuadInDirection(x, y - 1, z))
			AddQuad(voxelPosition, voxel, VoxelSide.Bottom, invertFaces);

		// Check backwards
		if (CanPlaceQuadInDirection(x, y, z - 1))
			AddQuad(voxelPosition, voxel, VoxelSide.Front, invertFaces);

		// Check forwards
		if (CanPlaceQuadInDirection(x, y, z + 1))
			AddQuad(voxelPosition, voxel, VoxelSide.Back, invertFaces);
	}

	// Makes several checks to see if it is okay to place a quad
	private bool CanPlaceQuadInDirection(int adjacentX, int adjacentY, int adjacentZ)
	{
		// Check if edge of grid
		bool edgeOfGrid = voxelGrid.IsOutOfBounds(adjacentX, adjacentY, adjacentZ);
		if (edgeOfGrid) return true;

		// Make checks for open air
		Voxel adjacentVoxel = voxelGrid.ReadVoxel(adjacentX, adjacentY, adjacentZ);
		VoxelTextureData adjacentVoxelData = voxelTexturesData[(int)adjacentVoxel.type];
		bool openAir = adjacentVoxelData.renderMethod == RenderMethod.None;
		bool adjacentVoxelTransparency =
			adjacentVoxelData.renderMethod == RenderMethod.Transparent
			|| adjacentVoxelData.renderMethod == RenderMethod.Decoration;
		return openAir || adjacentVoxelTransparency;
	}

	// Add a quad to the triangles and vertices
	private void AddQuad(Vector3Int position, Voxel voxel, VoxelSide side, bool invertFace)
	{
		// Set up
		int vertexStartingIndex = vertices.Count;

		// Get the UV coordinates for each side of the voxel
		int blockTypeIndex = (int)voxel.type;
		VoxelTextureData voxelData = voxelTexturesData[blockTypeIndex];
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
		foreach (int corner in faceCorners)
		{
			vertices.Add(position + cornerOffsets[corner]);
			colors32.Add(voxel.color);
		}

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
		uv.AddRange(new Vector2[]{
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
		mesh.uv = uv.ToArray();
		mesh.colors32 = colors32.ToArray();
		mesh.RecalculateNormals();
	}
}
