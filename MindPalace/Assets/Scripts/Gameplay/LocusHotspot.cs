using UnityEngine;

/// <summary>
/// Hotspot trigger that detects when the player enters and allows interaction with E key.
/// Shows interaction prompt via PalaceHUD when player is nearby.
/// Attach to a GameObject with a trigger collider (SphereCollider recommended).
/// SETUP: Use "MindPalace > Create Hotspot At Selection" to auto-create these.
/// </summary>
[RequireComponent(typeof(Collider))]
public class LocusHotspot : MonoBehaviour {
	[Header("Locus Identity")]
	[Tooltip("Unique ID for this memory location. Cards are associated with this ID.")]
	public string locusId;
	
	[Tooltip("Human-readable name shown in prompts")]
	public string displayName;
	
	[Header("Optional UI")]
	[Tooltip("GameObject to show when player is nearby (e.g., 'Press E' text)")]
	public GameObject interactPrompt;
	
	[Header("Visual Feedback")]
	[Tooltip("Color when player is nearby (applied to Prompt child if it has a Renderer)")]
	public Color highlightColor = Color.yellow;
	
	private bool _playerInside;
	private PalaceHUD _hud;
	private Color _originalColor;
	private Renderer _promptRenderer;

	private void Start() {
		_hud = FindObjectOfType<PalaceHUD>();
		
		if (interactPrompt != null) {
			_promptRenderer = interactPrompt.GetComponent<Renderer>();
			if (_promptRenderer != null) {
				_originalColor = _promptRenderer.material.color;
			}
		}
	}

	private void Reset() {
		var col = GetComponent<Collider>();
		col.isTrigger = true;
		if (string.IsNullOrEmpty(locusId)) locusId = gameObject.name.ToLower();
		if (string.IsNullOrEmpty(displayName)) displayName = gameObject.name.Replace("_Hotspot", "").Replace("_", " ");
	}

	private void OnTriggerEnter(Collider other) {
		if (other.CompareTag("Player")) {
			_playerInside = true;
			if (interactPrompt != null) interactPrompt.SetActive(true);
			
			// Show HUD prompt
			if (_hud != null) {
				_hud.ShowPrompt(displayName);
			}

			// Highlight prompt visual
			if (_promptRenderer != null) {
				_promptRenderer.material.color = highlightColor;
			}
		}
	}

	private void OnTriggerExit(Collider other) {
		if (other.CompareTag("Player")) {
			_playerInside = false;
			if (interactPrompt != null) interactPrompt.SetActive(false);
			
			// Hide HUD prompt
			if (_hud != null) {
				_hud.HidePrompt();
			}

			// Restore prompt color
			if (_promptRenderer != null) {
				_promptRenderer.material.color = _originalColor;
			}
		}
	}

	private void Update() {
		// Check for E key press while player is inside trigger
		if (_playerInside && Input.GetKeyDown(KeyCode.E)) {
			if (PalaceManager.Instance != null) {
				PalaceManager.Instance.OpenLocus(this);
			} else {
				Debug.LogError("LocusHotspot: PalaceManager.Instance is null! Add PalaceManager to scene.");
			}
		}
	}

	// Draw hotspot radius in editor for easy visualization
	private void OnDrawGizmos() {
		var col = GetComponent<SphereCollider>();
		if (col != null) {
			Gizmos.color = new Color(1f, 0.9f, 0.2f, 0.15f);
			Gizmos.DrawSphere(transform.position, col.radius);
			Gizmos.color = new Color(1f, 0.9f, 0.2f, 0.6f);
			Gizmos.DrawWireSphere(transform.position, col.radius);
		}
	}

	private void OnDrawGizmosSelected() {
		var col = GetComponent<SphereCollider>();
		if (col != null) {
			Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
			Gizmos.DrawSphere(transform.position, col.radius);
		}
	}
}
