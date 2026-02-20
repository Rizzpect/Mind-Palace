using UnityEngine;

/// <summary>
/// In-game HUD overlay for the Mind Palace.
/// Shows a crosshair, interaction prompt, card statistics, and locus info.
/// Auto-created by the scene builder tools.
/// </summary>
public class PalaceHUD : MonoBehaviour {

	[Header("HUD Settings")]
	[Tooltip("Color of the crosshair dot")]
	public Color crosshairColor = new Color(1f, 1f, 1f, 0.7f);
	
	[Tooltip("Size of the crosshair in pixels")]
	public float crosshairSize = 4f;
	
	[Tooltip("Color of the interaction prompt text")]
	public Color promptColor = new Color(1f, 0.95f, 0.8f, 1f);

	private bool _showPrompt;
	private string _promptText = "";
	private string _locusName = "";
	private GUIStyle _promptStyle;
	private GUIStyle _statsStyle;
	private GUIStyle _locusStyle;
	private GUIStyle _titleStyle;
	private Texture2D _crosshairTex;

	private void Start() {
		// Create a white pixel texture for the crosshair
		_crosshairTex = new Texture2D(1, 1);
		_crosshairTex.SetPixel(0, 0, Color.white);
		_crosshairTex.Apply();
	}

	private void OnDestroy() {
		if (_crosshairTex != null) Destroy(_crosshairTex);
	}

	/// <summary>Show interaction prompt when player is near a hotspot</summary>
	public void ShowPrompt(string locusDisplayName) {
		_showPrompt = true;
		_locusName = string.IsNullOrEmpty(locusDisplayName) ? "Memory Locus" : locusDisplayName;
		_promptText = "Press  E  to interact";
	}

	/// <summary>Hide interaction prompt when player leaves hotspot</summary>
	public void HidePrompt() {
		_showPrompt = false;
	}

	private void InitStyles() {
		if (_promptStyle != null) return;

		_promptStyle = new GUIStyle(GUI.skin.label) {
			fontSize = 22,
			alignment = TextAnchor.MiddleCenter,
			fontStyle = FontStyle.Bold,
			normal = { textColor = promptColor }
		};

		_locusStyle = new GUIStyle(GUI.skin.label) {
			fontSize = 16,
			alignment = TextAnchor.MiddleCenter,
			fontStyle = FontStyle.Italic,
			normal = { textColor = new Color(0.8f, 0.85f, 1f, 0.9f) }
		};

		_statsStyle = new GUIStyle(GUI.skin.label) {
			fontSize = 14,
			alignment = TextAnchor.UpperLeft,
			normal = { textColor = new Color(1f, 1f, 1f, 0.6f) }
		};

		_titleStyle = new GUIStyle(GUI.skin.label) {
			fontSize = 18,
			alignment = TextAnchor.UpperLeft,
			fontStyle = FontStyle.Bold,
			normal = { textColor = new Color(1f, 1f, 1f, 0.8f) }
		};
	}

	private void OnGUI() {
		InitStyles();

		// === Crosshair (center dot) ===
		float cx = Screen.width / 2f;
		float cy = Screen.height / 2f;
		GUI.color = crosshairColor;
		GUI.DrawTexture(new Rect(cx - crosshairSize / 2, cy - crosshairSize / 2, crosshairSize, crosshairSize), _crosshairTex);

		// Draw crosshair arms (small cross)
		float armLength = crosshairSize * 3f;
		float armThick = 1.5f;
		GUI.DrawTexture(new Rect(cx - armLength, cy - armThick / 2, armLength - crosshairSize, armThick), _crosshairTex); // left
		GUI.DrawTexture(new Rect(cx + crosshairSize, cy - armThick / 2, armLength - crosshairSize, armThick), _crosshairTex); // right
		GUI.DrawTexture(new Rect(cx - armThick / 2, cy - armLength, armThick, armLength - crosshairSize), _crosshairTex); // top
		GUI.DrawTexture(new Rect(cx - armThick / 2, cy + crosshairSize, armThick, armLength - crosshairSize), _crosshairTex); // bottom
		GUI.color = Color.white;

		// === Interaction Prompt ===
		if (_showPrompt) {
			float promptY = cy + 60f;
			GUI.Label(new Rect(0, promptY - 25, Screen.width, 30), _locusName, _locusStyle);
			GUI.Label(new Rect(0, promptY, Screen.width, 35), _promptText, _promptStyle);
		}

		// === Stats Panel (top-left) ===
		if (PalaceManager.Instance != null) {
			float x = 15f, y = 15f;
			GUI.Label(new Rect(x, y, 300, 25), "🏛️ Mind Palace", _titleStyle);
			y += 28f;

			int totalCards = PalaceManager.Instance.TotalCards;
			int dueCards = PalaceManager.Instance.DueCards;
			int totalLoci = PalaceManager.Instance.TotalLoci;

			GUI.Label(new Rect(x, y, 300, 20), $"Cards: {totalCards}  |  Due: {dueCards}  |  Loci: {totalLoci}", _statsStyle);
		}
	}
}
