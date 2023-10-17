// Dependencies
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// Generates a terrain
[RequireComponent(typeof(VoxelRenderer))]
public class SceneryGenerator : MonoBehaviour
{
	// Inspector variables
	[Header("Creation")]
	public bool randomizeSeed = true;
	[Range(0, 999)]
	public int seed;

	[Header("Generation")]
	public bool generateTerrain = true;
	public bool generateCaves = true;
	public bool generateFlora = true;

	[Header("Theme")]
	public bool randomizeTheme = true;
	public Theme theme = Theme.Spring;
	public Texture2D[] texturePacks;

	// Class variables
	private VoxelGrid voxelGrid;
	private VoxelRenderer voxelRenderer;
	private TerrainGenerator terrainGenerator;
	private CaveGenerator caveGenerator;
	private FloraGenerator floraGenerator;

	// Themes
	public enum Theme
	{
		Spring,
		Summer,
		Fall,
		Winter
	}

	// Start is called before the first frame update
	private void Awake()
	{
		// Create a new voxel grid
		voxelGrid = new VoxelGrid(48, 48, 48);

		// Fill in variables
		voxelRenderer = GetComponent<VoxelRenderer>();
		voxelRenderer.voxelGrid = voxelGrid;
		terrainGenerator = new TerrainGenerator(voxelGrid);
		caveGenerator = new CaveGenerator(voxelGrid);
		floraGenerator = new FloraGenerator(voxelGrid);

		// First generation
		Generate();
	}

	// Called every frame
	private void Update()
	{
		if (Input.GetKeyUp(KeyCode.Tab)) Generate();
	}

	// Generate a new terrain
	public void Generate()
	{
		// Randomize seed
		if (randomizeSeed) RandomizeSeed();

		// Randomize theme
		if (randomizeTheme)
		{
			Random.InitState(seed);
			theme = (Theme)Random.Range(0, System.Enum.GetValues(typeof(Theme)).Length);
		}

		// Reset voxel grid
		voxelGrid.NewGrid();

		// Make changes to voxel grid
		if (generateTerrain) terrainGenerator.WriteTerrain(seed);
		if (generateCaves) caveGenerator.WriteCaves(seed);
		if (generateFlora) floraGenerator.WriteFlora(seed);

		// Apply changes
		voxelRenderer.Refresh(texturePacks[(int)theme]);
	}

	// Randomize seed
	private void RandomizeSeed()
	{
		Random.InitState(System.DateTime.Now.Millisecond);
		seed = Random.Range(0, 999);
	}

	// Editor
	[CustomEditor(typeof(SceneryGenerator))]
	private class SceneryGeneratorEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			GUILayout.Space(10);
			GUILayout.Label("Controls", EditorStyles.boldLabel);
			bool generateNew = GUILayout.Button("Generate New");
			if (generateNew)
			{
				SceneryGenerator sceneryGenerator = (SceneryGenerator)target;
				sceneryGenerator.Generate();
			}
		}
	}
}
