#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

/// <summary>
/// Automatically builds a complete Mind Palace scene with one menu click.
/// Creates rooms, floor, player, hotspots, UI, and lighting.
/// </summary>
public static class SceneAutoBuilder {
	
	[MenuItem("MindPalace/⭐ Build Complete Palace Scene (Auto)", false, 0)]
	public static void BuildCompletePalaceScene() {
		if (!EditorUtility.DisplayDialog(
			"Build Complete Scene?",
			"This will clear the current scene and build:\n\n" +
			"• Floor (10×30m)\n" +
			"• 3 Rooms + Corridor\n" +
			"• Player with camera\n" +
			"• Memory UI system\n" +
			"• 4 Interactive hotspots\n" +
			"• Directional lighting\n\n" +
			"Continue?",
			"Yes, Build It!",
			"Cancel")) {
			return;
		}

		// Clear scene
		var allObjects = Object.FindObjectsOfType<GameObject>();
		foreach (var obj in allObjects) {
			Object.DestroyImmediate(obj);
		}

		Debug.Log("🏗️ Building Mind Palace scene...");

		// 1. Create Floor
		var floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
		floor.name = "Floor";
		floor.transform.position = new Vector3(0, 0, 12);
		floor.transform.localScale = new Vector3(10, 0.2f, 30);
		Debug.Log("✅ Floor created");

		// 2. Create Room A
		var roomA = GameObject.CreatePrimitive(PrimitiveType.Cube);
		roomA.name = "Room_A";
		roomA.transform.position = new Vector3(0, 2.5f, 0);
		roomA.transform.localScale = new Vector3(8, 5, 8);
		Object.DestroyImmediate(roomA.GetComponent<BoxCollider>()); // Remove collider so player doesn't hit walls
		Debug.Log("✅ Room A created");

		// 3. Create Corridor
		var corridor = GameObject.CreatePrimitive(PrimitiveType.Cube);
		corridor.name = "Corridor";
		corridor.transform.position = new Vector3(0, 2.5f, 6);
		corridor.transform.localScale = new Vector3(4, 5, 4);
		Object.DestroyImmediate(corridor.GetComponent<BoxCollider>());
		Debug.Log("✅ Corridor created");

		// 4. Create Room B
		var roomB = GameObject.CreatePrimitive(PrimitiveType.Cube);
		roomB.name = "Room_B";
		roomB.transform.position = new Vector3(0, 2.5f, 12);
		roomB.transform.localScale = new Vector3(8, 5, 8);
		Object.DestroyImmediate(roomB.GetComponent<BoxCollider>());
		Debug.Log("✅ Room B created");

		// 5. Create Room C
		var roomC = GameObject.CreatePrimitive(PrimitiveType.Cube);
		roomC.name = "Room_C";
		roomC.transform.position = new Vector3(0, 2.5f, 24);
		roomC.transform.localScale = new Vector3(8, 5, 8);
		Object.DestroyImmediate(roomC.GetComponent<BoxCollider>());
		Debug.Log("✅ Room C created");

		// 6. Create Player
		var player = new GameObject("Player");
		player.tag = "Player";
		player.transform.position = new Vector3(0, 1.2f, 6);
		
		var charController = player.AddComponent<CharacterController>();
		charController.height = 2f;
		charController.radius = 0.5f;
		charController.center = Vector3.zero;
		
		var fpController = player.AddComponent<SimpleFirstPersonController>();
		
		var camera = new GameObject("Main Camera");
		camera.tag = "MainCamera";
		camera.transform.SetParent(player.transform);
		camera.transform.localPosition = new Vector3(0, 1.6f, 0);
		var cam = camera.AddComponent<Camera>();
		cam.fieldOfView = 60;
		camera.AddComponent<AudioListener>();
		
		fpController.playerCamera = camera.transform;
		Debug.Log("✅ Player with camera created");

		// 7. Create Memory UI
		MindPalaceTools.CreateMemoryUI();
		Debug.Log("✅ Memory UI created");

		// 8. Create Hotspots
		CreateHotspotAt("Desk_A", new Vector3(2, 1, 0), "desk_a");
		CreateHotspotAt("Window_A", new Vector3(-3, 1, 0), "window_a");
		CreateHotspotAt("Globe_B", new Vector3(3, 1, 12), "globe_b");
		CreateHotspotAt("Shelf_C", new Vector3(0, 1, 24), "shelf_c");
		Debug.Log("✅ 4 Hotspots created");

		// 9. Create Directional Light
		var lightGO = new GameObject("Directional Light");
		var light = lightGO.AddComponent<Light>();
		light.type = LightType.Directional;
		light.color = new Color(1f, 0.956f, 0.839f);
		light.intensity = 1f;
		light.shadows = LightShadows.Soft;
		lightGO.transform.rotation = Quaternion.Euler(50, -30, 0);
		Debug.Log("✅ Directional Light created");

		// 10. Save scene
		EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
		EditorSceneManager.SaveOpenScenes();

		Debug.Log("🎉 <b>Mind Palace scene built successfully!</b>");
		Debug.Log("📋 Next steps:\n" +
			"  1. Press Play ▶️\n" +
			"  2. Walk with WASD, Sprint with Shift, look with mouse\n" +
			"  3. Walk to a hotspot (yellow cube)\n" +
			"  4. Press E to open memory card\n" +
			"  5. Type question/answer, click Good/Easy to grade\n" +
			"  6. Use ◀ ▶ to browse cards, + New Card to add more\n" +
			"  7. Cards are saved automatically!");

		EditorUtility.DisplayDialog(
			"Scene Built! 🎉",
			"Your Mind Palace is ready!\n\n" +
			"Press Play ▶️ and walk to a hotspot.\n" +
			"Press E to create memory cards.\n" +
			"Use Shift to sprint!\n\n" +
			"Check the Console for next steps.",
			"Got It!");
	}

	private static void CreateHotspotAt(string name, Vector3 position, string locusId) {
		var marker = new GameObject(name);
		marker.transform.position = position;
		
		var hotspotGO = new GameObject(name + "_Hotspot");
		hotspotGO.transform.position = position + Vector3.up * 0.5f;
		
		var col = hotspotGO.AddComponent<SphereCollider>();
		col.isTrigger = true;
		col.radius = 1.2f;
		
		var hotspot = hotspotGO.AddComponent<LocusHotspot>();
		hotspot.locusId = locusId;
		hotspot.displayName = name;
		
		// Optional: Create a simple "Press E" prompt
		var promptGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
		promptGO.name = "Prompt";
		promptGO.transform.SetParent(hotspotGO.transform);
		promptGO.transform.localPosition = Vector3.up * 0.8f;
		promptGO.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
		Object.DestroyImmediate(promptGO.GetComponent<BoxCollider>());
		var renderer = promptGO.GetComponent<MeshRenderer>();
		renderer.material.color = Color.yellow;
		promptGO.SetActive(false);
		
		hotspot.interactPrompt = promptGO;
		
		Object.DestroyImmediate(marker); // Clean up temporary marker
	}
}
#endif

