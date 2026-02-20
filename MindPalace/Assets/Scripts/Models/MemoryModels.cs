using System;
using System.Collections.Generic;

/// <summary>
/// Represents a single memory card attached to a locus (location) in the palace.
/// Uses spaced repetition data (ease, reps, lapses) to schedule reviews.
/// </summary>
[Serializable]
public class MemoryCard {
	public string id;              // Unique identifier (GUID)
	public string locusId;         // Which hotspot this card belongs to
	public string front;           // Question/prompt
	public string back;            // Answer
	public string imagePath;       // Optional: path to associated image (future feature)
	public string dueIsoUtc;       // ISO 8601 UTC timestamp for next review
	public int intervalDays;       // Days until next review
	public double ease;            // Ease factor (starts at 2.5, min 1.3)
	public int reps;               // Successful repetitions in a row
	public int lapses;             // Number of times forgotten

	/// <summary>Helper property to parse/set due date as DateTime</summary>
	public DateTime DueUtc {
		get { return DateTime.TryParse(dueIsoUtc, out var dt) ? DateTime.SpecifyKind(dt, DateTimeKind.Utc) : DateTime.UtcNow; }
		set { dueIsoUtc = value.ToUniversalTime().ToString("o"); }
	}
}

/// <summary>
/// Top-level save data structure. Serialized to JSON at Application.persistentDataPath.
/// </summary>
[Serializable]
public class MemorySaveData {
	public string palaceId = "default-palace";
	public List<MemoryCard> cards = new List<MemoryCard>();
	public int streak;              // Days in a row with reviews (future feature)
	public string lastSessionUtc;   // Last time user reviewed (future feature)
}
