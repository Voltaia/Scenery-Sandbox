// Dependencies
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// Generates voxel meshes
public static class VoxelMeshFactory
{
	// Static variables
	private static VoxelTextureData[] VoxelTexturesData;
	private static int TexturesBlockWidth;
	private static Color32[] Pixels;

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

	// Set up
	public static void SetTextureData(VoxelTextureData[] voxelTexturesData, int texturesBlockWidth, Texture2D texture2D)
	{
		VoxelTexturesData = voxelTexturesData;
		TexturesBlockWidth = texturesBlockWidth;
		Pixels = texture2D.GetPixels32();
	}

	// Generate the mesh
	public static Mesh GenerateMesh(VoxelGrid voxelGrid)
	{
		// Create the mesh data
		MeshData meshData = new MeshData();

		// Loop through all dimensions
		for (int x = 0; x < voxelGrid.width; x++)
			for (int y = 0; y < voxelGrid.height; y++)
				for (int z = 0; z < voxelGrid.length; z++)
				{
					// Get the voxel
					Voxel voxel = voxelGrid.ReadVoxel(x, y, z);
					VoxelTextureData voxelData = VoxelTexturesData[(int)voxel.type];

					// Render methods
					switch (voxelData.renderMethod)
					{
						// Standard
						case RenderMethod.Standard:
							// Add a normal quad cube
							AddQuadCube(voxelGrid, meshData, x, y, z, voxel, false);
							break;

						// Transparent
						case RenderMethod.Transparent:
							// Add a normal quad cube
							AddQuadCube(voxelGrid, meshData, x, y, z, voxel, false);

							// Add an inverted quad cube
							AddQuadCube(voxelGrid, meshData, x, y, z, voxel, true);
							break;

						// Decoration
						case RenderMethod.Decoration:
							// Add a decoration
							AddDecoration(meshData, x, y, z, voxel);
							break;
					}
				}

		// Return mesh
		return meshData.Mesh;
	}

	// Add decoration
	private static void AddDecoration(MeshData meshData, int x, int y, int z, Voxel voxel)
	{
		// Get texture map width
		int texturesPixelWidth = (int)Mathf.Sqrt(Pixels.Length);
		int textureWidth = texturesPixelWidth / TexturesBlockWidth;

		// Randomize rotation
		Random.InitState(x + y + z);
		bool flipXAndZ = Random.value > 0.5f;

		// Create voxel grid
		VoxelGrid decorationVoxelGrid = new VoxelGrid(textureWidth, textureWidth, textureWidth);

		// Get texture position
		VoxelTextureData voxelData = VoxelTexturesData[(int)voxel.type];
		int cursorStartX = voxelData.sideTextureCoordinates.x * textureWidth;
		int cursorStartY = voxelData.sideTextureCoordinates.y * textureWidth;
		for (int cursorX = cursorStartX; cursorX < cursorStartX + textureWidth; cursorX++)
			for (int cursorY = cursorStartY; cursorY < cursorStartY + textureWidth; cursorY++)
			{
				int pixelIndex = cursorX + cursorY * texturesPixelWidth;
				Color32 color = Pixels[pixelIndex];
				if (color.a > 0.5f)
				{
					if (!flipXAndZ)
					{
						Vector3Int writePosition = new Vector3Int(cursorX - cursorStartX, cursorY - cursorStartY, 8);
						decorationVoxelGrid.WriteVoxel(writePosition.x, writePosition.y, writePosition.z, new Voxel(color));
						decorationVoxelGrid.WriteVoxel(writePosition.x, writePosition.y, writePosition.z - 1, new Voxel(color));
					}
					else
					{
						Vector3Int writePosition = new Vector3Int(8, cursorY - cursorStartY, cursorX - cursorStartX);
						decorationVoxelGrid.WriteVoxel(writePosition.x, writePosition.y, writePosition.z, new Voxel(color));
						decorationVoxelGrid.WriteVoxel(writePosition.x - 1, writePosition.y, writePosition.z, new Voxel(color));
					}
				}
			}

		// Generate the mesh
		Mesh decorationMesh = VoxelMeshFactory.GenerateMesh(decorationVoxelGrid);

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
			decorationTriangles[triangleIndex] += meshData.vertices.Count;
		
		// Apply the decoration to the mesh
		meshData.vertices.AddRange(decorationVertices);
		meshData.triangles.AddRange(decorationTriangles);
		meshData.uv.AddRange(decorationMesh.uv);
		meshData.colors32.AddRange(decorationMesh.colors32);
	}

	// Add a quad cube
	private static void AddQuadCube(VoxelGrid voxelGrid, MeshData meshData, int x, int y, int z, Voxel voxel, bool invertFaces)
	{
		// Get voxel position TEMPORARY REPLACE VECTOR3INTS
		Vector3Int voxelPosition = new Vector3Int(x, y, z);

		// Check left
		if (CanPlaceQuadInDirection(voxelGrid, x - 1, y, z))
			AddQuad(meshData, voxelPosition, voxel, VoxelSide.Left, invertFaces);

		// Check right
		if (CanPlaceQuadInDirection(voxelGrid, x + 1, y, z))
			AddQuad(meshData, voxelPosition, voxel, VoxelSide.Right, invertFaces);

		// Check up
		if (CanPlaceQuadInDirection(voxelGrid, x, y + 1, z))
			AddQuad(meshData, voxelPosition, voxel, VoxelSide.Top, invertFaces);

		// Check down
		if (CanPlaceQuadInDirection(voxelGrid, x, y - 1, z))
			AddQuad(meshData, voxelPosition, voxel, VoxelSide.Bottom, invertFaces);

		// Check backwards
		if (CanPlaceQuadInDirection(voxelGrid, x, y, z - 1))
			AddQuad(meshData, voxelPosition, voxel, VoxelSide.Front, invertFaces);

		// Check forwards
		if (CanPlaceQuadInDirection(voxelGrid, x, y, z + 1))
			AddQuad(meshData, voxelPosition, voxel, VoxelSide.Back, invertFaces);
	}

	// Makes several checks to see if it is okay to place a quad
	private static bool CanPlaceQuadInDirection(VoxelGrid voxelGrid, int adjacentX, int adjacentY, int adjacentZ)
	{
		// Check if edge of grid
		bool edgeOfGrid = voxelGrid.IsOutOfBounds(adjacentX, adjacentY, adjacentZ);
		if (edgeOfGrid) return true;

		// Make checks for open air
		Voxel adjacentVoxel = voxelGrid.ReadVoxel(adjacentX, adjacentY, adjacentZ);
		VoxelTextureData adjacentVoxelData = VoxelTexturesData[(int)adjacentVoxel.type];
		bool openAir = adjacentVoxelData.renderMethod == RenderMethod.None;
		bool adjacentVoxelTransparency =
			adjacentVoxelData.renderMethod == RenderMethod.Transparent
			|| adjacentVoxelData.renderMethod == RenderMethod.Decoration;
		return openAir || adjacentVoxelTransparency;
	}

	// Add a quad to the triangles and vertices
	private static void AddQuad(MeshData meshData, Vector3Int position, Voxel voxel, VoxelSide side, bool invertFace)
	{
		// Set up
		int vertexStartingIndex = meshData.vertices.Count;

		// Get the UV coordinates for each side of the voxel
		int blockTypeIndex = (int)voxel.type;
		VoxelTextureData voxelData = VoxelTexturesData[blockTypeIndex];
		Vector2 uvSideCoordinates = (Vector2)voxelData.sideTextureCoordinates / TexturesBlockWidth;
		Vector2 uvTopCoordinates = (Vector2)voxelData.topTextureCoordinates / TexturesBlockWidth;
		Vector2 uvBottomCoordinates = (Vector2)voxelData.bottomTextureCoordinates / TexturesBlockWidth;

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
		meshData.AddQuadVertices(position, voxel.color, faceCorners);

		// Add face
		meshData.AddQuadTriangles(vertexStartingIndex, invertFace);

		// Add UV coordinates
		float textureUnit = 1.0f / TexturesBlockWidth;
		meshData.AddQuadUVs(uvStartCoordinates, textureUnit);
	}
}