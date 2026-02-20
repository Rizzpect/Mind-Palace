#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

/// <summary>
/// Diagnostic tool to validate Mind Palace scene setup.
/// Menu: MindPalace → Validate Scene Setup
/// </summary>
public static class MindPalaceDiagnostics {
	
	[MenuItem("MindPalace/Validate Scene Setup")]
	public static void ValidateSetup() {
		Debug.Log("=== Mind Palace Scene Validation ===");
		bool allGood = true;

		// Check for PalaceManager
		var manager = Object.FindObjectOfType<PalaceManager>();
		if (manager == null) {
			Debug.LogError("❌ PalaceManager not found! Create via 'MindPalace > Create Memory UI'");
			allGood = false;
		} else {
			Debug.Log("✅ PalaceManager exists");
			
			// Check UI reference
			if (manager.memoryCardUI == null) {
				Debug.LogError("❌ PalaceManager.memoryCardUI is not assigned!");
				allGood = false;
			} else {
				Debug.Log("✅ MemoryCardUI is wired correctly");
			}
			
			// Check loci
			if (manager.loci.Count == 0) {
				var hotspots = Object.FindObjectsOfType<LocusHotspot>();
				if (hotspots.Length > 0) {
					Debug.LogWarning($"⚠️ Found {hotspots.Length} hotspots but not registered. They'll auto-register on Play.");
				} else {
					Debug.LogWarning("⚠️ No LocusHotspot objects found. Create via 'MindPalace > Create Hotspot At Selection'");
				}
			} else {
				Debug.Log($"✅ {manager.loci.Count} loci registered");
			}
		}

		// Check for player
		var player = GameObject.FindGameObjectWithTag("Player");
		if (player == null) {
			Debug.LogError("❌ No GameObject with Tag='Player' found! Hotspots won't detect player.");
			allGood = false;
		} else {
			Debug.Log($"✅ Player found: {player.name}");
			
			// Check for character controller or rigidbody
			if (player.GetComponent<CharacterController>() == null && player.GetComponent<Rigidbody>() == null) {
				Debug.LogWarning("⚠️ Player has no CharacterController or Rigidbody. Movement may not work.");
			}
		}

		// Check for hotspots
		var allHotspots = Object.FindObjectsOfType<LocusHotspot>();
		if (allHotspots.Length == 0) {
			Debug.LogWarning("⚠️ No LocusHotspot components found. Add them via 'MindPalace > Create Hotspot At Selection'");
		} else {
			Debug.Log($"✅ {allHotspots.Length} hotspots in scene:");
			foreach (var hotspot in allHotspots) {
				var col = hotspot.GetComponent<Collider>();
				if (col != null && col.isTrigger) {
					Debug.Log($"  • {hotspot.name} (ID: {hotspot.locusId}) - OK");
				} else {
					Debug.LogError($"  • {hotspot.name} - ❌ Missing trigger collider!");
					allGood = false;
				}
			}
		}

		// Check for floor/ground
		bool hasFloor = false;
		foreach (var obj in Object.FindObjectsOfType<GameObject>()) {
			if (obj.name.ToLower().Contains("floor") || obj.name.ToLower().Contains("ground")) {
				var col = obj.GetComponent<Collider>();
				if (col != null && !col.isTrigger) {
					hasFloor = true;
					Debug.Log($"✅ Floor/ground found: {obj.name}");
					break;
				}
			}
		}
		if (!hasFloor) {
			Debug.LogWarning("⚠️ No floor/ground collider found. Player may fall through world.");
		}

		// Check lighting
		var lights = Object.FindObjectsOfType<Light>();
		if (lights.Length == 0) {
			Debug.LogWarning("⚠️ No lights in scene. World will be dark.");
		} else {
			Debug.Log($"✅ {lights.Length} light(s) found");
		}

		// Summary
		Debug.Log("=================================");
		if (allGood) {
			Debug.Log("✅ Scene validation passed! Ready to play.");
		} else {
			Debug.LogWarning("⚠️ Some issues found. Check errors above.");
		}
		
		Debug.Log($"Save file location: {Application.persistentDataPath}/palace-save.json");
		Debug.Log("=================================");
	}

	[MenuItem("MindPalace/Open Save File Location")]
	public static void OpenSaveLocation() {
		string path = Application.persistentDataPath;
		System.Diagnostics.Process.Start(path);
		Debug.Log($"Opened: {path}");
	}

	[MenuItem("MindPalace/Delete Save File (Reset)")]
	public static void DeleteSave() {
		string savePath = System.IO.Path.Combine(Application.persistentDataPath, "palace-save.json");
		if (System.IO.File.Exists(savePath)) {
			if (EditorUtility.DisplayDialog("Delete Save File?", 
				$"This will permanently delete:\n{savePath}\n\nAll memory cards will be lost!", 
				"Delete", "Cancel")) {
				System.IO.File.Delete(savePath);
				Debug.Log("✅ Save file deleted. Fresh start on next play.");
			}
		} else {
			Debug.Log("No save file found (it's already deleted or never created).");
		}
	}

	[MenuItem("MindPalace/Help - Show Setup Steps")]
	public static void ShowHelp() {
		string help = @"
=== MIND PALACE SETUP QUICK REFERENCE ===

1. CREATE UI (run once):
   MindPalace → Create Memory UI
   
2. BUILD SCENE:
   • Add 3-4 Cube GameObjects for rooms (remove BoxCollider)
   • Add Floor Cube with BoxCollider (scale Y=0.2)
   • Add Directional Light (baked mode)
   • Add Player with Tag='Player'

3. ADD HOTSPOTS:
   • Select a desk/prop GameObject
   • MindPalace → Create Hotspot At Selection
   • Repeat for 4-6 locations

4. VALIDATE:
   MindPalace → Validate Scene Setup

5. PLAY:
   • Walk to hotspot (WASD)
   • Press E to interact
   • Grade with Again/Hard/Good/Easy

Full guide: See SETUP_GUIDE.md in project root!
========================================
";
		Debug.Log(help);
		EditorUtility.DisplayDialog("Mind Palace Setup", help, "Got it!");
	}
}
#endif
