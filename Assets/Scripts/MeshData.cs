using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}