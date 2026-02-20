#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public static class AutoDecorate {
	
	[MenuItem("GameObject/🎨 Make Palace Beautiful (Auto)", false, 0)]
	public static void MakeBeautiful() {
		if (!EditorUtility.DisplayDialog(
			"Make Palace Beautiful?",
			"This will automatically:\n\n" +
			"✨ Add colors to all rooms\n" +
			"🪑 Add furniture (desks, shelves, plants)\n" +
			"🏛️ Add 2 more rooms\n" +
			"💡 Add lighting\n\n" +
			"Continue?",
			"Yes, Make It Beautiful!",
			"Cancel")) {
			return;
		}

		Debug.Log("🎨 Making your palace beautiful...");

		// STEP 1: Add Colors
		AddColors();
		
		// STEP 2: Add Furniture
		AddFurniture();
		
		// STEP 3: Add More Rooms
		AddMoreRooms();

		Debug.Log("✅ DONE! Your palace is beautiful!");
		EditorUtility.DisplayDialog(
			"Palace Complete! 🎉",
			"Your Mind Palace is now beautiful!\n\n" +
			"✅ Colorful rooms\n" +
			"✅ Furniture and decorations\n" +
			"✅ 5 rooms total\n" +
			"✅ Lighting\n\n" +
			"Press Play ▶️ to explore!",
			"Awesome!");
	}

	static void AddColors() {
		Debug.Log("Adding colors...");
		
		var blueMat = new Material(Shader.Find("Standard"));
		blueMat.color = new Color(0.3f, 0.5f, 0.8f);
		
		var yellowMat = new Material(Shader.Find("Standard"));
		yellowMat.color = new Color(0.9f, 0.8f, 0.4f);
		
		var grayMat = new Material(Shader.Find("Standard"));
		grayMat.color = new Color(0.6f, 0.6f, 0.65f);
		
		var greenMat = new Material(Shader.Find("Standard"));
		greenMat.color = new Color(0.4f, 0.7f, 0.5f);
		
		var floorMat = new Material(Shader.Find("Standard"));
		floorMat.color = new Color(0.8f, 0.75f, 0.7f);
		
		ApplyMaterial("Room_A", blueMat);
		ApplyMaterial("Room_B", yellowMat);
		ApplyMaterial("Room_C", grayMat);
		ApplyMaterial("Corridor", greenMat);
		ApplyMaterial("Floor", floorMat);
		
		Debug.Log("✅ Colors added!");
	}

	static void AddFurniture() {
		Debug.Log("Adding furniture...");
		
		// Room A - Study
		CreateDesk(new Vector3(2, 0.5f, 0), "Desk_A");
		CreateBookshelf(new Vector3(-3, 1.5f, 0), "Bookshelf_A");
		CreateChair(new Vector3(2, 0.3f, -1), "Chair_A");
		
		// Room B - Living
		CreateTable(new Vector3(0, 0.4f, 12), "Table_B");
		CreateGlobe(new Vector3(3, 0.8f, 12), "Globe_B");
		CreatePlant(new Vector3(-3, 0.5f, 12), "Plant_B");
		
		// Room C - Archive
		CreateArchiveCabinet(new Vector3(0, 1f, 24), "Archive_C");
		CreateBox(new Vector3(2, 0.3f, 24), "Box_C");
		CreateBox(new Vector3(-2, 0.3f, 24), "Box_C2");
		
		// Corridor
		CreateLamp(new Vector3(0, 3f, 6), "Lamp_Corridor");
		
		Debug.Log("✅ Furniture added!");
	}

	static void AddMoreRooms() {
		Debug.Log("Adding more rooms...");
		
		// Room D - Library
		var roomD = GameObject.CreatePrimitive(PrimitiveType.Cube);
		roomD.name = "Room_D_Library";
		roomD.transform.position = new Vector3(10, 2.5f, 0);
		roomD.transform.localScale = new Vector3(6, 5, 6);
		Object.DestroyImmediate(roomD.GetComponent<BoxCollider>());
		
		var purpleMat = new Material(Shader.Find("Standard"));
		purpleMat.color = new Color(0.6f, 0.4f, 0.7f);
		roomD.GetComponent<MeshRenderer>().material = purpleMat;
		
		// Room E - Garden
		var roomE = GameObject.CreatePrimitive(PrimitiveType.Cube);
		roomE.name = "Room_E_Garden";
		roomE.transform.position = new Vector3(-10, 2.5f, 12);
		roomE.transform.localScale = new Vector3(6, 5, 6);
		Object.DestroyImmediate(roomE.GetComponent<BoxCollider>());
		
		var mintMat = new Material(Shader.Find("Standard"));
		mintMat.color = new Color(0.5f, 0.8f, 0.7f);
		roomE.GetComponent<MeshRenderer>().material = mintMat;
		
		// Add furniture to new rooms
		CreateBookshelf(new Vector3(10, 1.5f, 0), "Bookshelf_D");
		CreatePlant(new Vector3(-10, 0.5f, 12), "Plant_E");
		CreatePlant(new Vector3(-9, 0.5f, 11), "Plant_E2");
		
		// Extend floor
		var floor = GameObject.Find("Floor");
		if (floor != null) {
			floor.transform.localScale = new Vector3(25, 0.2f, 30);
		}
		
		Debug.Log("✅ More rooms added!");
	}

	// Helper functions
	static void CreateDesk(Vector3 pos, string name) {
		var desk = GameObject.CreatePrimitive(PrimitiveType.Cube);
		desk.name = name;
		desk.transform.position = pos;
		desk.transform.localScale = new Vector3(1.5f, 0.1f, 0.8f);
		desk.GetComponent<MeshRenderer>().material.color = new Color(0.4f, 0.25f, 0.15f);
	}
	
	static void CreateBookshelf(Vector3 pos, string name) {
		var shelf = GameObject.CreatePrimitive(PrimitiveType.Cube);
		shelf.name = name;
		shelf.transform.position = pos;
		shelf.transform.localScale = new Vector3(2f, 3f, 0.4f);
		shelf.GetComponent<MeshRenderer>().material.color = new Color(0.3f, 0.2f, 0.15f);
		
		for (int i = 0; i < 5; i++) {
			var book = GameObject.CreatePrimitive(PrimitiveType.Cube);
			book.name = "Book";
			book.transform.SetParent(shelf.transform);
			book.transform.localPosition = new Vector3(-0.7f + i * 0.3f, 0.5f, 0.3f);
			book.transform.localScale = new Vector3(0.15f, 0.4f, 0.25f);
			book.GetComponent<MeshRenderer>().material.color = Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.5f, 1f);
			Object.DestroyImmediate(book.GetComponent<BoxCollider>());
		}
	}
	
	static void CreateChair(Vector3 pos, string name) {
		var chair = GameObject.CreatePrimitive(PrimitiveType.Cube);
		chair.name = name;
		chair.transform.position = pos;
		chair.transform.localScale = new Vector3(0.5f, 0.6f, 0.5f);
		chair.GetComponent<MeshRenderer>().material.color = new Color(0.5f, 0.3f, 0.2f);
	}
	
	static void CreateTable(Vector3 pos, string name) {
		var table = GameObject.CreatePrimitive(PrimitiveType.Cube);
		table.name = name;
		table.transform.position = pos;
		table.transform.localScale = new Vector3(2f, 0.1f, 1.2f);
		table.GetComponent<MeshRenderer>().material.color = new Color(0.6f, 0.4f, 0.25f);
	}
	
	static void CreateGlobe(Vector3 pos, string name) {
		var globe = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		globe.name = name;
		globe.transform.position = pos;
		globe.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
		globe.GetComponent<MeshRenderer>().material.color = new Color(0.3f, 0.6f, 0.8f);
		
		var stand = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
		stand.name = "Stand";
		stand.transform.SetParent(globe.transform);
		stand.transform.localPosition = new Vector3(0, -0.5f, 0);
		stand.transform.localScale = new Vector3(0.15f, 0.5f, 0.15f);
		stand.GetComponent<MeshRenderer>().material.color = new Color(0.3f, 0.2f, 0.1f);
		Object.DestroyImmediate(stand.GetComponent<CapsuleCollider>());
	}
	
	static void CreatePlant(Vector3 pos, string name) {
		var pot = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
		pot.name = name;
		pot.transform.position = pos;
		pot.transform.localScale = new Vector3(0.4f, 0.5f, 0.4f);
		pot.GetComponent<MeshRenderer>().material.color = new Color(0.6f, 0.3f, 0.2f);
		
		var leaves = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		leaves.name = "Leaves";
		leaves.transform.SetParent(pot.transform);
		leaves.transform.localPosition = new Vector3(0, 1f, 0);
		leaves.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
		leaves.GetComponent<MeshRenderer>().material.color = new Color(0.2f, 0.6f, 0.3f);
		Object.DestroyImmediate(leaves.GetComponent<SphereCollider>());
	}
	
	static void CreateArchiveCabinet(Vector3 pos, string name) {
		var cabinet = GameObject.CreatePrimitive(PrimitiveType.Cube);
		cabinet.name = name;
		cabinet.transform.position = pos;
		cabinet.transform.localScale = new Vector3(1.5f, 2f, 0.6f);
		cabinet.GetComponent<MeshRenderer>().material.color = new Color(0.4f, 0.4f, 0.45f);
	}
	
	static void CreateBox(Vector3 pos, string name) {
		var box = GameObject.CreatePrimitive(PrimitiveType.Cube);
		box.name = name;
		box.transform.position = pos;
		box.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
		box.GetComponent<MeshRenderer>().material.color = new Color(0.7f, 0.6f, 0.5f);
	}
	
	static void CreateLamp(Vector3 pos, string name) {
		var lamp = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		lamp.name = name;
		lamp.transform.position = pos;
		lamp.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
		lamp.GetComponent<MeshRenderer>().material.color = new Color(1f, 0.95f, 0.8f);
		Object.DestroyImmediate(lamp.GetComponent<SphereCollider>());
		
		var light = lamp.AddComponent<Light>();
		light.type = LightType.Point;
		light.range = 8f;
		light.intensity = 0.5f;
		light.color = new Color(1f, 0.9f, 0.7f);
	}
	
	static void ApplyMaterial(string objName, Material mat) {
		var obj = GameObject.Find(objName);
		if (obj != null) {
			var renderer = obj.GetComponent<MeshRenderer>();
			if (renderer != null) {
				renderer.material = mat;
			}
		}
	}
}
#endif

