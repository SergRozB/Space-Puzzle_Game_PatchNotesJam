using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class player : MonoBehaviour
{

    bool isMoving = false;
    public float speed = 100.0f;
    public bool canJump = true;
    public TMPro.TextMeshProUGUI scoreText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        Vector2 movement = MovementFunc();
        transform.Translate(new Vector2(movement.x, movement.y));
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "flor")
        {
            Debug.Log("Collided with floor");
            canJump = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "flor")
        {
            canJump = false;
        }

        if (collision.gameObject.tag == "coin")
        {
            destroyCoin(collision.gameObject);
        }
    }

    void destroyCoin(GameObject coin) { 
        Destroy(coin);
    }

    Vector2 MovementFunc() 
    {
        Vector2 direction = Vector3.zero;
        if (Keyboard.current.spaceKey.isPressed)
        {
            if (canJump) {
                direction.y = 1;
            }
        }
        if (Keyboard.current.aKey.isPressed)
        {
            direction.x = -1;
        }
        if (Keyboard.current.dKey.isPressed)
        {
            direction.x = 1;
        }
        if (Keyboard.current.sKey.isPressed)
        {
            direction.y = -1;
        }

        if (direction.magnitude > 0)
        {
            direction = direction.normalized;
            direction *= speed;
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }

        return direction;
    }
}
