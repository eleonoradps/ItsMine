using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D body;
    Transform playerPos;
    [SerializeField] Camera cam;
    Vector2 mousePosition;
    Vector2 direction;
    [SerializeField]float speed;

    
    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        playerPos = GetComponent<Transform>();
    }

    private void FixedUpdate()
    {
        body.velocity = new Vector2(direction.x * speed * Time.fixedDeltaTime, direction.y * speed * Time.fixedDeltaTime);
        Vector2 lookDirection = mousePosition - body.position;
        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
        body.rotation = angle;
    }
    
    void Update()
    {
        direction = new Vector2(Input.GetAxisRaw("Horizontal") * speed, Input.GetAxisRaw("Vertical") * speed);
        mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
    }

    public Vector2 ReturnPlayerPos()
    {
        return playerPos.position;
    }
}
