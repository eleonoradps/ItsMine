using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D body;
    Transform playerPos;
    Camera cam;
    CameraFollower cameraFollower;
    Vector2 mousePosition;
    Vector2 direction;
    [SerializeField]float speed;

    
    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        playerPos = GetComponent<Transform>();
        cam = FindObjectOfType<Camera>();
        cameraFollower = FindObjectOfType<CameraFollower>();
    }

    void Update()
    {
        
        body.velocity = new Vector2(direction.x * speed * Time.deltaTime, direction.y * speed * Time.deltaTime);

        direction = new Vector2(Input.GetAxisRaw("Horizontal") * speed, Input.GetAxisRaw("Vertical") * speed);
        mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
        
        //fix camera position in player position
        cameraFollower.GetPlayerPos(playerPos);
    }

    public Vector2 ReturnPlayerPos()
    {
        return playerPos.position;
    }
}
