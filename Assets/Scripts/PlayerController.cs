using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    float speed = 7;

    [SerializeField]
    float jumpForce = 8000;


    [SerializeField]
    Transform feet;

    [SerializeField]
    LayerMask groundLayer;

    [SerializeField]
    LayerMask EnemiesLayer;

    [SerializeField]
    Transform RespawnPoint;

    [SerializeField]
    monkeyController moker;
    Rigidbody2D rigidbody;
    TrailRenderer trailrender;
    bool hasReleasedJumpButton = true;





    bool isDashing;
    bool canDash = true;
    Vector2 dashDir;
    float DashVelocity = 15;
    float speedX;
   
    float dashtime = 0.25f;
    public float accelerationSpeed = 5f;
    float accelRatePerSec;

    float forwardVelocity;


    Vector2 footPosition;
    Vector2 bottomColliderSize = Vector2.zero;

    float groundRadius = 0.2f;

    bool isKill;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        accelRatePerSec = 5 + speed / accelerationSpeed ;

    }

    void Start()
    {
        trailrender = GetComponent<TrailRenderer>();

    }
    // Update is called once per frame
    void Update()
    {
        bool dashInput = Input.GetButtonDown("Dash");
        speedX = Input.GetAxisRaw("Horizontal");
        
        rigidbody.gravityScale = 5;

        


        Vector2 movement = new Vector2(speedX, 0);
        movement = movement.normalized * forwardVelocity * Time.deltaTime;

        if (!isDashing)
        {
            transform.Translate(movement);
        }

        
        bool isGrounded = Physics2D.OverlapBox(GetFoot(), GetFootSize(), 0, groundLayer);




        if (dashInput && canDash)
        {
            canDash = false;
            isDashing = true;
            trailrender.emitting = true;
            dashDir = new Vector2(speedX, Input.GetAxisRaw("Vertical"));
            if (dashDir == Vector2.zero)
            {
                dashDir = new Vector2(transform.localScale.x, 0);
            }
            StartCoroutine(StopDashing());
        }

        if (isDashing)
        {
            rigidbody.velocity = dashDir * DashVelocity;
            rigidbody.gravityScale = 0;
            return;
        }

        if (isGrounded)
        {
            canDash = true;
        }

        if (Input.GetAxisRaw("Jump") > 0 && hasReleasedJumpButton == true && isGrounded)
        {

            rigidbody.AddForce(Vector2.up * jumpForce);
            hasReleasedJumpButton = false;
        }

        if (Input.GetAxisRaw("Jump") == 0)
        {
            hasReleasedJumpButton = true;
        }

        if (speedX > 0)
        {
            gameObject.transform.localScale = new Vector3(1, 1, 1);
            
        }

        if (speedX < 0)
        {
            gameObject.transform.localScale = new Vector3(-1, 1, 1);
            
        }

        // Collider2D enemyCollision = enemy.GetComponent<Collider2D>();
        // if (enemyCollision != null)
        // {
        //     Collider2D colliders = Physics2D.OverlapBox(enemyCollision.bounds.center, enemyCollision.bounds.size, 0f);
        // }
    }

    private void FixedUpdate() {
        if (speedX == 1 || speedX == -1)
        {
            forwardVelocity += accelRatePerSec * Time.deltaTime;
            forwardVelocity = Mathf.Min(forwardVelocity, speed);
        }

        if (speedX == 0)
        {
            forwardVelocity = 0;
        }
    }

    private IEnumerator StopDashing()
    {
        yield return new WaitForSeconds(dashtime);
        print("stop");
        trailrender.emitting = false;
        trailrender.Clear();
        isDashing = false;
        rigidbody.velocity *= 0;
    }


    private Vector2 GetFoot()
    {
        float height = GetComponent<Collider2D>().bounds.size.y;
        return transform.position + Vector3.down * height * 0.45f;
    }

    private Vector2 GetFootSize()
    {
        return new Vector2(bottomColliderSize.x = GetComponent<Collider2D>().bounds.size.x * 0.7f, 0.1f);
    }


    private void OnDrawGizmos()
    {


        Gizmos.DrawWireCube(GetFoot(), GetFootSize());

    }

    



    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "enemy")
        {
            isKill = Physics2D.OverlapBox(GetFoot(), GetFootSize(), 0, EnemiesLayer);
            if (isKill)
            {
                other.gameObject.GetComponent<monkeyController>().EnemyDeath();
                
            }
            else
            {
                
                SceneManager.LoadScene("End");
            }

        }
    }
}
