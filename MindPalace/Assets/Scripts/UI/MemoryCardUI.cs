using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI panel for creating/editing memory cards and grading recall.
/// Supports browsing multiple cards at a locus with Previous/Next navigation,
/// creating new cards, and deleting cards.
/// Auto-created by "MindPalace > Create Memory UI" menu command.
/// </summary>
public class MemoryCardUI : MonoBehaviour {
	[Header("UI Components - Auto-wired by editor tool")]
	public GameObject panelRoot;
	public TMP_InputField frontField;
	public TMP_InputField backField;
	public RawImage attachedImage;
	public Button againBtn;
	public Button hardBtn;
	public Button goodBtn;
	public Button easyBtn;
	public Button closeBtn;
	
	[Header("Navigation (optional - auto-created if null)")]
	public Button prevBtn;
	public Button nextBtn;
	public Button newCardBtn;
	public Button deleteBtn;
	public TextMeshProUGUI cardCountLabel;
	public TextMeshProUGUI locusNameLabel;

	private List<MemoryCard> _cards;
	private int _currentIndex;
	private string _locusId;
	private Action<MemoryCard> _onSave;
	private Action<MemoryCard> _onDelete;
	private Func<string, MemoryCard> _onCreateNew;

	private void Awake() {
		if (panelRoot == null || frontField == null || backField == null ||
		    againBtn == null || hardBtn == null || goodBtn == null || easyBtn == null || closeBtn == null) {
			Debug.LogError("MemoryCardUI: UI components not wired! Use 'MindPalace > Create Memory UI' to set up.");
			enabled = false;
			return;
		}

		panelRoot.SetActive(false);

		// Grade buttons
		againBtn.onClick.AddListener(() => Grade(MemoryScheduler.Grade.Again));
		hardBtn.onClick.AddListener(() => Grade(MemoryScheduler.Grade.Hard));
		goodBtn.onClick.AddListener(() => Grade(MemoryScheduler.Grade.Good));
		easyBtn.onClick.AddListener(() => Grade(MemoryScheduler.Grade.Easy));
		closeBtn.onClick.AddListener(Close);

		// Navigation buttons (optional — may not exist in older scenes)
		if (prevBtn != null) prevBtn.onClick.AddListener(PreviousCard);
		if (nextBtn != null) nextBtn.onClick.AddListener(NextCard);
		if (newCardBtn != null) newCardBtn.onClick.AddListener(CreateNewCard);
		if (deleteBtn != null) deleteBtn.onClick.AddListener(DeleteCurrentCard);
	}

	/// <summary>Open with a single card (legacy compatibility)</summary>
	public void Open(MemoryCard card, Action<MemoryCard> onSave) {
		OpenWithCards(new List<MemoryCard> { card }, "", onSave, null, null);
	}

	/// <summary>Open with multiple cards at a locus, supporting navigation</summary>
	public void OpenWithCards(List<MemoryCard> cards, string locusName, 
		Action<MemoryCard> onSave, Action<MemoryCard> onDelete, Func<string, MemoryCard> onCreateNew) {
		
		_cards = cards;
		_currentIndex = 0;
		_locusId = cards.Count > 0 ? cards[0].locusId : "";
		_onSave = onSave;
		_onDelete = onDelete;
		_onCreateNew = onCreateNew;

		DisplayCurrentCard();

		// Update locus name label
		if (locusNameLabel != null) {
			locusNameLabel.text = string.IsNullOrEmpty(locusName) ? "Memory Locus" : locusName;
		}

		// Show UI and pause game
		panelRoot.SetActive(true);
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
		Time.timeScale = 0f;
	}

	private void DisplayCurrentCard() {
		if (_cards == null || _cards.Count == 0) return;
		
		_currentIndex = Mathf.Clamp(_currentIndex, 0, _cards.Count - 1);
		var card = _cards[_currentIndex];

		frontField.text = card.front ?? "";
		backField.text = card.back ?? "";

		UpdateCardCountLabel();
		UpdateNavigationButtons();
	}

	private void UpdateCardCountLabel() {
		if (cardCountLabel != null) {
			cardCountLabel.text = $"Card {_currentIndex + 1} / {_cards.Count}";
		}
	}

	private void UpdateNavigationButtons() {
		if (prevBtn != null) prevBtn.interactable = _currentIndex > 0;
		if (nextBtn != null) nextBtn.interactable = _currentIndex < _cards.Count - 1;
		if (deleteBtn != null) deleteBtn.interactable = _cards.Count > 0;
	}

	// === Navigation ===

	private void PreviousCard() {
		SaveCurrentTextToCard();
		if (_currentIndex > 0) {
			_currentIndex--;
			DisplayCurrentCard();
		}
	}

	private void NextCard() {
		SaveCurrentTextToCard();
		if (_currentIndex < _cards.Count - 1) {
			_currentIndex++;
			DisplayCurrentCard();
		}
	}

	private void CreateNewCard() {
		SaveCurrentTextToCard();

		if (_onCreateNew != null) {
			var newCard = _onCreateNew.Invoke(_locusId);
			_cards.Add(newCard);
			_currentIndex = _cards.Count - 1;
			DisplayCurrentCard();
		}
	}

	private void DeleteCurrentCard() {
		if (_cards.Count == 0) return;
		
		var card = _cards[_currentIndex];
		_cards.RemoveAt(_currentIndex);
		_onDelete?.Invoke(card);

		if (_cards.Count == 0) {
			Close();
			return;
		}

		_currentIndex = Mathf.Clamp(_currentIndex, 0, _cards.Count - 1);
		DisplayCurrentCard();
	}

	// === Grading ===

	private void SaveCurrentTextToCard() {
		if (_cards == null || _cards.Count == 0) return;
		var card = _cards[_currentIndex];
		card.front = frontField.text;
		card.back = backField.text;
	}

	private void Grade(MemoryScheduler.Grade grade) {
		if (_cards == null || _cards.Count == 0) return;

		SaveCurrentTextToCard();
		var card = _cards[_currentIndex];
		
		// Apply spaced repetition algorithm
		MemoryScheduler.ApplyGrade(card, grade, DateTime.UtcNow);
		
		// Notify manager to save
		_onSave?.Invoke(card);
		Close();
	}

	public void Close() {
		panelRoot.SetActive(false);
		Time.timeScale = 1f;
		
		// Re-lock cursor for first-person controls
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}
}
