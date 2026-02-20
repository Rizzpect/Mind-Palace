#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public static class MindPalaceTools {
	[MenuItem("MindPalace/Create Memory UI")] 
	public static void CreateMemoryUI() {
		var uiRoot = new GameObject("UI_Root", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
		var canvas = uiRoot.GetComponent<Canvas>();
		canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		var scaler = uiRoot.GetComponent<CanvasScaler>();
		scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
		scaler.referenceResolution = new Vector2(1920, 1080);

		// Full-screen semi-transparent panel
		var panel = new GameObject("MemoryPanel", typeof(Image));
		panel.transform.SetParent(uiRoot.transform, false);
		var img = panel.GetComponent<Image>();
		img.color = new Color(0.05f, 0.05f, 0.1f, 0.85f);
		var panelRT = panel.GetComponent<RectTransform>();
		panelRT.anchorMin = Vector2.zero; panelRT.anchorMax = Vector2.one;
		panelRT.offsetMin = Vector2.zero; panelRT.offsetMax = Vector2.zero;
		panel.SetActive(false);

		// --- Header: Locus Name Label ---
		var locusLabel = MakeLabel("LocusNameLabel", panel.transform, new Vector2(0, 250), 32, FontStyle.Bold, "Memory Locus");

		// --- Card Counter Label ---
		var cardCountLabel = MakeLabel("CardCountLabel", panel.transform, new Vector2(0, 200), 18, FontStyle.Italic, "Card 1 / 1");

		// --- Input Fields ---
		TMP_InputField MakeField(string name, Vector2 anchoredPos, string placeholder) {
			var go = new GameObject(name, typeof(RectTransform), typeof(TMP_InputField), typeof(TextMeshProUGUI), typeof(Image));
			go.transform.SetParent(panel.transform, false);
			var rt = go.GetComponent<RectTransform>();
			rt.sizeDelta = new Vector2(800, 80);
			rt.anchoredPosition = anchoredPos;
			var bg = go.GetComponent<Image>(); bg.color = new Color(1, 1, 1, 0.08f);
			var text = go.GetComponent<TextMeshProUGUI>();
			text.enableWordWrapping = true; text.fontSize = 26; text.text = "";
			var field = go.GetComponent<TMP_InputField>();
			field.textViewport = rt;
			return field;
		}

		var front = MakeField("FrontField", new Vector2(0, 100), "Question / Front");
		var back  = MakeField("BackField",  new Vector2(0, 0),   "Answer / Back");

		// --- Button helper ---
		Button MakeButton(string name, Vector2 pos, string label, Color color, Vector2? size = null) {
			var btnSize = size ?? new Vector2(150, 50);
			var go = new GameObject(name, typeof(RectTransform), typeof(Button), typeof(Image));
			go.transform.SetParent(panel.transform, false);
			var rt = go.GetComponent<RectTransform>();
			rt.sizeDelta = btnSize;
			rt.anchoredPosition = pos;
			var imgB = go.GetComponent<Image>(); imgB.color = color;
			var textGO = new GameObject("Label", typeof(TextMeshProUGUI));
			textGO.transform.SetParent(go.transform, false);
			var text = textGO.GetComponent<TextMeshProUGUI>();
			text.text = label; text.alignment = TextAlignmentOptions.Center; text.fontSize = 20;
			var rtText = textGO.GetComponent<RectTransform>(); rtText.sizeDelta = btnSize;
			return go.GetComponent<Button>();
		}

		// --- Grade Buttons ---
		var again = MakeButton("AgainBtn", new Vector2(-240, -120), "Again",  new Color(0.8f, 0.2f, 0.2f, 0.5f));
		var hard  = MakeButton("HardBtn",  new Vector2(-80,  -120), "Hard",   new Color(0.8f, 0.6f, 0.2f, 0.5f));
		var good  = MakeButton("GoodBtn",  new Vector2(80,   -120), "Good",   new Color(0.2f, 0.7f, 0.3f, 0.5f));
		var easy  = MakeButton("EasyBtn",  new Vector2(240,  -120), "Easy",   new Color(0.2f, 0.5f, 0.8f, 0.5f));

		// --- Navigation Buttons ---
		var prev    = MakeButton("PrevBtn",    new Vector2(-200, -200), "◀ Prev",   new Color(1, 1, 1, 0.15f), new Vector2(120, 40));
		var next    = MakeButton("NextBtn",    new Vector2(200,  -200), "Next ▶",   new Color(1, 1, 1, 0.15f), new Vector2(120, 40));
		var newCard = MakeButton("NewCardBtn", new Vector2(0,    -200), "+ New Card", new Color(0.3f, 0.7f, 0.4f, 0.3f), new Vector2(160, 40));

		// --- Utility Buttons ---
		var deleteBtn = MakeButton("DeleteBtn", new Vector2(350, -200), "🗑 Delete", new Color(0.7f, 0.2f, 0.2f, 0.3f), new Vector2(120, 40));
		var close     = MakeButton("CloseBtn",  new Vector2(0,   -280), "Close",     new Color(1, 1, 1, 0.2f), new Vector2(150, 50));

		// --- Wire the MemoryCardUI component ---
		var ui = uiRoot.AddComponent<MemoryCardUI>();
		ui.panelRoot = panel;
		ui.frontField = front;
		ui.backField = back;
		ui.againBtn = again;
		ui.hardBtn = hard;
		ui.goodBtn = good;
		ui.easyBtn = easy;
		ui.closeBtn = close;
		ui.prevBtn = prev;
		ui.nextBtn = next;
		ui.newCardBtn = newCard;
		ui.deleteBtn = deleteBtn;
		ui.cardCountLabel = cardCountLabel.GetComponent<TextMeshProUGUI>();
		ui.locusNameLabel = locusLabel.GetComponent<TextMeshProUGUI>();

		// --- Ensure PalaceManager exists ---
		var mgrGO = GameObject.Find("Managers");
		if (mgrGO == null) mgrGO = new GameObject("Managers");
		var mgr = mgrGO.GetComponent<PalaceManager>();
		if (mgr == null) mgr = mgrGO.AddComponent<PalaceManager>();
		mgr.memoryCardUI = ui;

		// --- Ensure HUD exists ---
		if (Object.FindObjectOfType<PalaceHUD>() == null) {
			mgrGO.AddComponent<PalaceHUD>();
			Debug.Log("✅ PalaceHUD added to Managers");
		}

		// --- Ensure EventSystem exists ---
		if (Object.FindObjectOfType<EventSystem>() == null) {
			var eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
			Debug.Log("✅ EventSystem created");
		}

		Selection.activeGameObject = uiRoot;
		Debug.Log("✅ Memory UI created with navigation, card management, and grade buttons");
	}

	static GameObject MakeLabel(string name, Transform parent, Vector2 pos, int fontSize, FontStyle style, string text) {
		var go = new GameObject(name, typeof(TextMeshProUGUI));
		go.transform.SetParent(parent, false);
		var rt = go.GetComponent<RectTransform>();
		rt.sizeDelta = new Vector2(600, 40);
		rt.anchoredPosition = pos;
		var tmp = go.GetComponent<TextMeshProUGUI>();
		tmp.text = text;
		tmp.fontSize = fontSize;
		tmp.fontStyle = (FontStyles)style;
		tmp.alignment = TextAlignmentOptions.Center;
		tmp.color = new Color(1, 1, 1, 0.9f);
		return go;
	}

	[MenuItem("MindPalace/Create Hotspot At Selection")] 
	public static void CreateHotspotAtSelection() {
		var sel = Selection.activeGameObject;
		if (sel == null) { Debug.LogWarning("Select a GameObject (like a desk) first."); return; }
		var go = new GameObject(sel.name + "_Hotspot", typeof(SphereCollider), typeof(LocusHotspot));
		go.transform.position = sel.transform.position + Vector3.up * 1.0f;
		var col = go.GetComponent<SphereCollider>(); col.isTrigger = true; col.radius = 1.2f;
		var locus = go.GetComponent<LocusHotspot>();
		locus.locusId = sel.name.ToLower();
		locus.displayName = sel.name.Replace("_", " ");
		Selection.activeGameObject = go;
	}

	[MenuItem("MindPalace/Create Simple Player")]
	public static void CreateSimplePlayer() {
		var existing = GameObject.FindGameObjectWithTag("Player");
		if (existing != null) {
			if (!EditorUtility.DisplayDialog("Player Exists", 
				$"A player already exists: {existing.name}\n\nCreate another one?", 
				"Yes", "Cancel")) {
				return;
			}
		}

		var playerGO = GameObject.CreatePrimitive(PrimitiveType.Capsule);
		playerGO.name = "Player";
		playerGO.tag = "Player";
		playerGO.transform.position = new Vector3(0, 1.2f, 6);
		
		Object.DestroyImmediate(playerGO.GetComponent<Collider>());
		var controller = playerGO.AddComponent<CharacterController>();
		controller.radius = 0.5f;
		controller.height = 2.0f;
		controller.center = Vector3.zero;
		
		var cameraGO = new GameObject("PlayerCamera");
		cameraGO.transform.SetParent(playerGO.transform);
		cameraGO.transform.localPosition = new Vector3(0, 0.6f, 0);
		var cam = cameraGO.AddComponent<Camera>();
		cam.tag = "MainCamera";
		
		var fpController = playerGO.AddComponent<SimpleFirstPersonController>();
		fpController.playerCamera = cameraGO.transform;
		
		Selection.activeGameObject = playerGO;
		Debug.Log("✅ Player created with sprint (Shift) and head bob. WASD to move, Mouse to look.");
	}
}
#endif
