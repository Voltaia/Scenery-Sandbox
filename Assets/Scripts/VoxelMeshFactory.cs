// Dependencies
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// Generates voxel meshes
public static class VoxelMeshFactory
{
	// Static variables
	private static int TexturesBlockWidth;
	private static Color32[] Pixels;

	// Set up
	public static void SetTextureData(int texturesBlockWidth, Texture2D texture2D)
	{
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
					VoxelTextureData textureData = VoxelRenderer.GetTextureData(voxel.type);

					// Render methods
					switch (textureData.renderMethod)
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
		VoxelTextureData textureData = VoxelRenderer.GetTextureData(voxel.type);
		int cursorStartX = textureData.sideTextureCoordinates.x * textureWidth;
		int cursorStartY = textureData.sideTextureCoordinates.y * textureWidth;
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
		Mesh decorationMesh = GenerateMesh(decorationVoxelGrid);

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

		int blockTypeIndex = (int)voxel.type;

		// Check left
		if (CanPlaceQuadInDirection(voxelGrid, x - 1, y, z))
			meshData.AddQuad(voxelPosition, voxel, Side.Left, TexturesBlockWidth, invertFaces);

		// Check right
		if (CanPlaceQuadInDirection(voxelGrid, x + 1, y, z))
			meshData.AddQuad(voxelPosition, voxel, Side.Right, TexturesBlockWidth, invertFaces);

		// Check up
		if (CanPlaceQuadInDirection(voxelGrid, x, y + 1, z))
			meshData.AddQuad(voxelPosition, voxel, Side.Top, TexturesBlockWidth, invertFaces);

		// Check down
		if (CanPlaceQuadInDirection(voxelGrid, x, y - 1, z))
			meshData.AddQuad(voxelPosition, voxel, Side.Bottom, TexturesBlockWidth, invertFaces);

		// Check backwards
		if (CanPlaceQuadInDirection(voxelGrid, x, y, z - 1))
			meshData.AddQuad(voxelPosition, voxel, Side.Front, TexturesBlockWidth, invertFaces);

		// Check forwards
		if (CanPlaceQuadInDirection(voxelGrid, x, y, z + 1))
			meshData.AddQuad(voxelPosition, voxel, Side.Back, TexturesBlockWidth, invertFaces);
	}

	// Makes several checks to see if it is okay to place a quad
	private static bool CanPlaceQuadInDirection(VoxelGrid voxelGrid, int adjacentX, int adjacentY, int adjacentZ)
	{
		// Check if edge of grid
		bool edgeOfGrid = voxelGrid.IsOutOfBounds(adjacentX, adjacentY, adjacentZ);
		if (edgeOfGrid) return true;

		// Make checks for open air
		Voxel adjacentVoxel = voxelGrid.ReadVoxel(adjacentX, adjacentY, adjacentZ);
		VoxelTextureData adjacentVoxelData = VoxelRenderer.GetTextureData(adjacentVoxel.type);
		bool openAir = adjacentVoxelData.renderMethod == RenderMethod.None;
		bool adjacentVoxelTransparency =
			adjacentVoxelData.renderMethod == RenderMethod.Transparent
			|| adjacentVoxelData.renderMethod == RenderMethod.Decoration;
		return openAir || adjacentVoxelTransparency;
	}
}