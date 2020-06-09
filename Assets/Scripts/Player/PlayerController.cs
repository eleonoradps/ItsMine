using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D body;
    private Transform playerPos;
    private CameraFollower cameraFollower;
    private Vector2 direction;
    [SerializeField] private float speed;

    
    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        playerPos = GetComponent<Transform>();
        cameraFollower = FindObjectOfType<CameraFollower>();
    }

    private void Update()
    {
        
        body.velocity = new Vector2(direction.x * speed, direction.y * speed);

        direction = new Vector2(Input.GetAxisRaw("Horizontal") * speed, Input.GetAxisRaw("Vertical") * speed);

        //fix camera position in player position
        cameraFollower.GetPlayerPos(playerPos);
    }

    public Vector2 ReturnPlayerPos()
    {
        return playerPos.position;
    }
}
