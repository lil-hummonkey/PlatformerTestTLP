using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
 
    [SerializeField]
    float speed = 5;

    [SerializeField]
    float jumpForce = 12000;

    [SerializeField]
    Transform feet;

    [SerializeField]
    LayerMask groundLayer;
    Rigidbody2D rigidbody;
    bool hasReleasedJumpButton = true;

    Vector2 footPosition;

    Vector2 bottomColliderSize = Vector2.zero;

    float groundRadius = 0.2f;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        
        
    }
    // Update is called once per frame
    void Update()
    {
        float speedX = Input.GetAxisRaw("Horizontal");
        float jumpY = Input.GetAxisRaw("Jump");

        Vector2 movement = new Vector2(speedX, 0);
        movement = movement.normalized * speed * Time.deltaTime;

        transform.Translate(movement);

        //bool isGrounded = Physics2D.OverlapCircle(GetFoot(), groundRadius, groundLayer);
        bool isGrounded = Physics2D.OverlapBox(GetFoot(), GetFootSize(), 0, groundLayer);

        if (Input.GetAxisRaw("Jump") > 0 && hasReleasedJumpButton == true && isGrounded)
        {
            
            rigidbody.AddForce(Vector2.up * jumpForce);
            hasReleasedJumpButton = false;
        }

        if (Input.GetAxisRaw("Jump") == 0)
        {
            hasReleasedJumpButton = true;
        }
        
        if(speedX > 0)
        {
            gameObject.transform.localScale = new Vector3(1,1,1);
        }

         if(speedX < 0)
        {
            gameObject.transform.localScale = new Vector3(-1,1,1);
        }
    }

    private Vector2 GetFoot()
    {
        float height = GetComponent<Collider2D>().bounds.size.y;
        return transform.position + Vector3.down * height / 2;
    }

    private Vector2 GetFootSize()
    {
        return new Vector2(bottomColliderSize.x = GetComponent<Collider2D>().bounds.size.x * 0.9f, 0.1f);
    }
    

    private void OnDrawGizmos() 
    {
        
            
            Gizmos.DrawWireCube(GetFoot(), GetFootSize());
        
    }
}
