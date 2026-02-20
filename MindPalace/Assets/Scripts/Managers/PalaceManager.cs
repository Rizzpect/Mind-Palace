using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Main singleton manager for the Mind Palace.
/// Handles loading/saving memory data, card management, statistics, and locus interaction.
/// SETUP: Drag the MemoryCardUI component reference into the inspector after running "MindPalace > Create Memory UI"
/// </summary>
public class PalaceManager : MonoBehaviour {
	public static PalaceManager Instance { get; private set; }

	[Header("UI References")]
	[Tooltip("Drag the MemoryCardUI component here after creating it via the MindPalace menu")]
	public MemoryCardUI memoryCardUI;
	
	[Header("Loci References")]
	[Tooltip("Add all LocusHotspot components in the scene here (or they'll be auto-found)")]
	public List<LocusHotspot> loci = new List<LocusHotspot>();

	private MemorySaveData _save;

	// === Public Stats API (used by PalaceHUD) ===
	public int TotalCards => _save?.cards.Count ?? 0;
	public int DueCards => _save?.cards.Count(c => c.DueUtc <= DateTime.UtcNow) ?? 0;
	public int TotalLoci => loci.Count;

	private void Awake() {
		if (Instance != null && Instance != this) { Destroy(gameObject); return; }
		Instance = this;
	}

	private void Start() {
		// Load existing save data or create new
		_save = SaveSystem.LoadOrCreate();
		
		// Auto-register all hotspots in scene if list is empty
		if (loci.Count == 0) {
			loci = new List<LocusHotspot>(FindObjectsOfType<LocusHotspot>());
		}
		
		// Create sample cards for testing if no cards exist
		if (_save.cards.Count == 0) SeedSampleCards();
		
		// Ensure player has correct tag for hotspot detection
		UpdatePlayerTag();

		Debug.Log($"🏛️ Mind Palace loaded: {_save.cards.Count} cards across {loci.Count} loci");
	}

	private void UpdatePlayerTag() {
		var player = GameObject.FindGameObjectWithTag("Player");
		if (player == null) {
			var controller = GameObject.Find("PlayerArmature");
			if (controller != null) controller.tag = "Player";
		}
	}

	private void SeedSampleCards() {
		var now = DateTime.UtcNow;
		var firstLocus = loci.FirstOrDefault();
		if (firstLocus == null) return;

		// Create a few starter cards so the demo feels alive
		_save.cards.Add(new MemoryCard {
			id = Guid.NewGuid().ToString(),
			locusId = firstLocus.locusId,
			front = "What is the Method of Loci?",
			back = "A memory technique that uses spatial visualization to organize and recall information.",
			ease = 2.5, intervalDays = 0,
			dueIsoUtc = now.ToString("o")
		});
		
		if (loci.Count > 1) {
			_save.cards.Add(new MemoryCard {
				id = Guid.NewGuid().ToString(),
				locusId = loci[1].locusId,
				front = "Capital of France?",
				back = "Paris",
				ease = 2.5, intervalDays = 0,
				dueIsoUtc = now.ToString("o")
			});
		}

		SaveSystem.Save(_save);
	}

	/// <summary>
	/// Called when player presses E near a hotspot.
	/// Gets all cards for this locus and opens the UI for browsing/creating cards.
	/// </summary>
	public void OpenLocus(LocusHotspot locus) {
		if (memoryCardUI == null) {
			Debug.LogError("PalaceManager: memoryCardUI is not assigned! Use MindPalace > Create Memory UI menu.");
			return;
		}
		
		// Get all cards at this locus, sorted by due date
		var locusCards = _save.cards
			.Where(c => c.locusId == locus.locusId)
			.OrderBy(c => c.DueUtc)
			.ToList();

		// If no cards exist for this locus, create a new blank one
		if (locusCards.Count == 0) {
			var newCard = CreateBlankCard(locus.locusId);
			locusCards.Add(newCard);
		}

		// Open the UI with all cards for this locus
		memoryCardUI.OpenWithCards(locusCards, locus.displayName, OnCardUpdated, OnCardDeleted, (locusId) => CreateBlankCard(locusId));
	}

	/// <summary>Create a new blank card at the specified locus</summary>
	public MemoryCard CreateBlankCard(string locusId) {
		var card = new MemoryCard {
			id = Guid.NewGuid().ToString(),
			locusId = locusId,
			front = "",
			back = "",
			ease = 2.5,
			intervalDays = 0,
			dueIsoUtc = DateTime.UtcNow.ToString("o")
		};
		_save.cards.Add(card);
		return card;
	}

	private void OnApplicationQuit() {
		if (_save != null) SaveSystem.Save(_save);
	}

	private void OnCardUpdated(MemoryCard updated) {
		var idx = _save.cards.FindIndex(c => c.id == updated.id);
		if (idx >= 0) _save.cards[idx] = updated;
		SaveSystem.Save(_save);
	}

	private void OnCardDeleted(MemoryCard deleted) {
		_save.cards.RemoveAll(c => c.id == deleted.id);
		SaveSystem.Save(_save);
		Debug.Log($"🗑️ Card deleted: \"{deleted.front}\"");
	}
}
