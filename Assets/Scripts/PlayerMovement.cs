using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    //Adds a float to adjust player speed
    //SerializeField makes it so we can adjust the speed in Unity for convenience sake
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    private Rigidbody2D body;
    private Animator anim;
    private BoxCollider2D boxCollider;
    private float wallJumpCooldown;
    private float horizontalInput; 

    private void Awake()
    {
        //Allows us to quickly grab references to Rigidbody and Animator from object
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    //Update function checks every frame for player input, which helps us to avoid if and else statements
    private void Update()
    {
        //Stores input.getaxis horizontal inside which makes it easier for us to access it and write code in the future
        horizontalInput = Input.GetAxis("Horizontal");

        //Flips player when moving on the X (horizontal) axis
        if (horizontalInput > 0.01f)
            transform.localScale = Vector3.one;

        else if (horizontalInput < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1);

        //Set animator parameters
        anim.SetBool("run", horizontalInput != 0);
        anim.SetBool("grounded", IsGrounded());


        //Wall jump logic + cooldown
        if (wallJumpCooldown > 0.2f)
        {
         
            //Input.GetAxis "Horizontal" let's us check for left-right inputs on every frame
            body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);

            //Checks if player is on the wall and not grounded
            if (OnWall() && !IsGrounded())
            {
                body.gravityScale = 0;
                body.velocity = Vector2.zero;
            }
            else
                body.gravityScale = 6;
            
            //Adds jumping
            if (Input.GetKey(KeyCode.Space))
                Jump();
        }
        else
            wallJumpCooldown += Time.deltaTime;
    }

    private void Jump()
    {
        if (IsGrounded())
        {
            body.velocity = new Vector2(body.velocity.x, jumpPower);
            anim.SetTrigger("jump");
        }
        else if (OnWall() && !IsGrounded())
        {
            if(horizontalInput == 0)
            {
                
                body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 10, 0);
                transform.localScale = new Vector3(-Mathf.Sign(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else
                //this returns 1 when the player is facing right, and -1 when the player is facing left, and pushes the player off the wall in order to wall jump, hence the '-' behind mathf.sign
                //10 is the force they will be pushed off the wall, and 6 is the force they'll be pushed up
                body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 3, 6);
            wallJumpCooldown = 0;
           
        }
    }

    //This piece of code tells us when our player is grounded, and when they're not using Raycasting and Boxcasting below the player to check if they're grounded 
    private bool IsGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }
    private bool OnWall()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);
        return raycastHit.collider != null;
    }

    public bool canAttack()
    {
        return horizontalInput == 0 && IsGrounded() && !OnWall();
    }
}
