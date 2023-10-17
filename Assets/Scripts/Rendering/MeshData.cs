// Dependencies
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Enum for determining quad side
public enum Side
{
	Left,
	Right,
	Top,
	Bottom,
	Front,
	Back
}

// Mesh data
public class MeshData
{
	// Variables
	private Mesh mesh = new Mesh();
	public List<Vector3> vertices = new List<Vector3>();
	public List<int> triangles = new List<int>();
	public List<Vector2> uv = new List<Vector2>();
	public List<Color32> colors32 = new List<Color32>();

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

	// Properties
	public Mesh Mesh
	{
		get
		{
			// Apply changes to mesh
			mesh.vertices = vertices.ToArray();
			mesh.triangles = triangles.ToArray();
			mesh.uv = uv.ToArray();
			mesh.colors32 = colors32.ToArray();
			mesh.RecalculateNormals();
			return mesh;
		}
	}

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

	// Constructor
	public MeshData()
	{
		mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
	}

	// Add vertices
	public void AddQuadVertices(Vector3Int position, Color32 color, int[] faceCorners)
	{
		foreach (int corner in faceCorners)
		{
			vertices.Add(position + cornerOffsets[corner]);
			colors32.Add(color);
		}
	}

	// Add face
	public void AddQuadTriangles(int vertexStartingIndex, bool isInverted)
	{
		// Check if inverted
		if (!isInverted)
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
	}

	// Add UV coordinates
	public void AddQuadUVs(Vector2 uvStartCoordinates, float textureUnit)
	{
		uv.AddRange(new Vector2[]{
			uvStartCoordinates,
			uvStartCoordinates + new Vector2(0.0f, textureUnit),
			uvStartCoordinates + new Vector2(textureUnit, 0.0f),
			uvStartCoordinates + new Vector2(textureUnit, textureUnit),
		});
	}

	// Add a quad to the triangles and vertices
	public void AddQuad(Vector3Int position, Voxel voxel, Side side, int size, bool invertFace)
	{
		// Set up
		int vertexStartingIndex = vertices.Count;

		// Get the UV coordinates for each side of the voxel
		Vector2 uvSideCoordinates = (Vector2)voxel.textureData.sideTextureCoordinates / size;
		Vector2 uvTopCoordinates = (Vector2)voxel.textureData.topTextureCoordinates / size;
		Vector2 uvBottomCoordinates = (Vector2)voxel.textureData.bottomTextureCoordinates / size;

		// Placeholder variables
		int[] faceCorners;
		Vector2 uvStartCoordinates;

		// Check which side
		switch (side)
		{
			case Side.Left:
				faceCorners = FaceCorners.Left;
				uvStartCoordinates = uvSideCoordinates;
				break;

			case Side.Right:
				faceCorners = FaceCorners.Right;
				uvStartCoordinates = uvSideCoordinates;
				break;

			case Side.Top:
				faceCorners = FaceCorners.Top;
				uvStartCoordinates = uvTopCoordinates;
				break;

			case Side.Bottom:
				faceCorners = FaceCorners.Bottom;
				uvStartCoordinates = uvBottomCoordinates;
				break;

			case Side.Front:
				faceCorners = FaceCorners.Front;
				uvStartCoordinates = uvSideCoordinates;
				break;

			case Side.Back:
				faceCorners = FaceCorners.Back;
				uvStartCoordinates = uvSideCoordinates;
				break;

			default: return;
		}

		// Add vertices
		AddQuadVertices(position, voxel.color, faceCorners);

		// Add face
		AddQuadTriangles(vertexStartingIndex, invertFace);

		// Add UV coordinates
		float textureUnit = 1.0f / size;
		AddQuadUVs(uvStartCoordinates, textureUnit);
	}
}