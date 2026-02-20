using System.IO;
using UnityEngine;

/// <summary>
/// Handles JSON serialization/deserialization of memory data to disk.
/// Save location: Application.persistentDataPath/palace-save.json
/// </summary>
public static class SaveSystem {
	private static string SavePath => Path.Combine(Application.persistentDataPath, "palace-save.json");

	/// <summary>Serialize and write save data to disk</summary>
	public static void Save(MemorySaveData data) {
		string json = JsonUtility.ToJson(data, true);
		File.WriteAllText(SavePath, json);
		Debug.Log($"Saved {data.cards.Count} cards to {SavePath}");
	}

	/// <summary>Load save data from disk, or return new empty data if file doesn't exist</summary>
	public static MemorySaveData LoadOrCreate() {
		try {
			if (File.Exists(SavePath)) {
				string json = File.ReadAllText(SavePath);
				var data = JsonUtility.FromJson<MemorySaveData>(json);
				Debug.Log($"Loaded {data.cards.Count} cards from {SavePath}");
				return data ?? new MemorySaveData();
			}
		} catch (System.Exception ex) {
			Debug.LogWarning($"Failed to load save: {ex.Message}");
		}
		return new MemorySaveData();
	}
}
