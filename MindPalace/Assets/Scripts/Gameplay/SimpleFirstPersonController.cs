using UnityEngine;

/// <summary>
/// First-person controller for Mind Palace.
/// Attach to a GameObject with a CharacterController component.
/// WASD to move, Mouse to look, Shift to sprint, E to interact (handled by LocusHotspot).
/// 
/// NOTE: This is a simple fallback. For better controls, use Unity Starter Assets!
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class SimpleFirstPersonController : MonoBehaviour {
	
	[Header("Movement Settings")]
	[Tooltip("Walk speed in units per second")]
	public float walkSpeed = 4.0f;
	
	[Tooltip("Sprint speed in units per second")]
	public float sprintSpeed = 7.0f;
	
	[Tooltip("Gravity force (negative Y)")]
	public float gravity = -9.81f;
	
	[Header("Look Settings")]
	[Tooltip("Mouse sensitivity for camera rotation")]
	public float mouseSensitivity = 2.0f;
	
	[Tooltip("Max angle you can look up (degrees)")]
	public float maxLookUpAngle = 80f;
	
	[Tooltip("Max angle you can look down (degrees)")]
	public float maxLookDownAngle = -80f;
	
	[Header("Head Bob")]
	[Tooltip("Enable subtle head bob while walking")]
	public bool enableHeadBob = true;
	
	[Tooltip("Head bob intensity")]
	public float bobAmount = 0.04f;
	
	[Tooltip("Head bob speed")]
	public float bobSpeed = 10f;
	
	[Header("Camera Reference")]
	[Tooltip("Drag the Main Camera here (or child camera object)")]
	public Transform playerCamera;
	
	private CharacterController _controller;
	private Vector3 _velocity;
	private float _rotationX = 0f;
	private float _bobTimer = 0f;
	private float _defaultCameraY;
	private bool _isSprinting;

	private void Start() {
		_controller = GetComponent<CharacterController>();
		
		// Auto-find camera if not assigned
		if (playerCamera == null) {
			playerCamera = GetComponentInChildren<Camera>()?.transform;
			if (playerCamera == null) {
				playerCamera = Camera.main?.transform;
			}
		}

		if (playerCamera != null) {
			_defaultCameraY = playerCamera.localPosition.y;
		}
		
		// Lock cursor for FPS control
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		
		// Ensure player has correct tag
		if (!CompareTag("Player")) {
			Debug.LogWarning("SimpleFirstPersonController: Setting GameObject tag to 'Player' for hotspot detection.");
			gameObject.tag = "Player";
		}
	}

	private void Update() {
		// Don't process input when game is paused (UI is open)
		if (Time.timeScale == 0f) return;

		HandleMovement();
		HandleLook();
		HandleHeadBob();
		
		// Allow Escape to unlock cursor (for testing)
		if (Input.GetKeyDown(KeyCode.Escape)) {
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}

		// Click to re-lock cursor
		if (Input.GetMouseButtonDown(0) && Cursor.lockState != CursorLockMode.Locked) {
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
	}

	private void HandleMovement() {
		// Get input (WASD or arrow keys)
		float horizontal = Input.GetAxis("Horizontal"); // A/D
		float vertical = Input.GetAxis("Vertical");     // W/S
		
		// Sprint check
		_isSprinting = Input.GetKey(KeyCode.LeftShift) && vertical > 0;
		float speed = _isSprinting ? sprintSpeed : walkSpeed;
		
		// Calculate movement direction relative to player facing
		Vector3 move = transform.right * horizontal + transform.forward * vertical;
		
		// Apply movement
		_controller.Move(move * speed * Time.deltaTime);
		
		// Apply gravity
		if (_controller.isGrounded && _velocity.y < 0) {
			_velocity.y = -2f; // Small negative to stay grounded
		}
		_velocity.y += gravity * Time.deltaTime;
		_controller.Move(_velocity * Time.deltaTime);
	}

	private void HandleLook() {
		if (playerCamera == null) return;
		
		// Get mouse input
		float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
		float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
		
		// Rotate player body left/right
		transform.Rotate(Vector3.up * mouseX);
		
		// Rotate camera up/down (with clamp)
		_rotationX -= mouseY;
		_rotationX = Mathf.Clamp(_rotationX, maxLookDownAngle, maxLookUpAngle);
		playerCamera.localRotation = Quaternion.Euler(_rotationX, 0f, 0f);
	}

	private void HandleHeadBob() {
		if (!enableHeadBob || playerCamera == null) return;

		float horizontal = Input.GetAxis("Horizontal");
		float vertical = Input.GetAxis("Vertical");
		bool isMoving = Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f;

		if (isMoving && _controller.isGrounded) {
			float speed = _isSprinting ? bobSpeed * 1.4f : bobSpeed;
			_bobTimer += Time.deltaTime * speed;
			float bobOffset = Mathf.Sin(_bobTimer) * bobAmount;
			
			Vector3 camPos = playerCamera.localPosition;
			camPos.y = _defaultCameraY + bobOffset;
			playerCamera.localPosition = camPos;
		} else {
			_bobTimer = 0f;
			// Smoothly return to default position
			Vector3 camPos = playerCamera.localPosition;
			camPos.y = Mathf.Lerp(camPos.y, _defaultCameraY, Time.deltaTime * 5f);
			playerCamera.localPosition = camPos;
		}
	}

	// Draw player bounds in editor
	private void OnDrawGizmosSelected() {
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(transform.position, 0.5f);
	}
}
