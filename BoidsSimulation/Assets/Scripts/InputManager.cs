using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
	public Transform player_cam;
	public Transform player;
	
	public float mouse_sensitivity;
	public float speed = 10f;
	public bool enable;
	private float x_rotation = 0f;
	
	private Vector2 mouse;
	private CharacterController controller;
	
    // Start is called before the first frame update
    void Start()
    {
        controller = player.GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
		if (enable){
			// player rotation
			mouse = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * mouse_sensitivity * Time.deltaTime;
			player.Rotate(player.up * mouse.x);
			
			// camera rotation
			x_rotation -= mouse.y;
			x_rotation = Mathf.Clamp(x_rotation, -90f, 90f);
			player_cam.localRotation = Quaternion.Euler(x_rotation, 0f, 0f);
			
			// player movement
			float x = Input.GetAxis("Horizontal");
			float z = Input.GetAxis("Vertical");
			float x_rotation_rad = -x_rotation / 180f * Mathf.PI;
			Vector3 forward_direction = player.forward * Mathf.Cos(x_rotation_rad) + player.up * Mathf.Sin(x_rotation_rad);
			Vector3 move = player.right * x + forward_direction * z;
			if (Input.GetKey("space"))
				move += player.up;
			controller.Move(move * speed * Time.deltaTime);
			
			
			if (Input.GetMouseButtonDown(0)){
				if (Time.timeScale > 0.5f)
					Time.timeScale = 0f;
				else
					Time.timeScale = 1f;
			}
		}
    }
}
