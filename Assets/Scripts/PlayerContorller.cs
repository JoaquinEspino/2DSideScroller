using System.Collections;
using System.Threading;
using UnityEngine;

public class PlayerContorller : MonoBehaviour
{
    public float speed = 5f;
    public float jumpSpeed = 5f;
    public float wallJumpSpeed = 2f;
    private float movement = 0f;
    private Rigidbody2D rigidBody;
    //Checking if player is grounded
    public Transform groundCheckPoint;
    public float groundCheckRadius;
    public LayerMask groundLayer;
    private bool isTouchingGround;
    //Checking if player is on the ceiling
    public Transform ceilingCheckPoint;
    public float ceilingCheckRadius;
    public LayerMask ceilingLayer;
    private bool isTouchingCeiling;
    //Checking if player falls of edge of Ceiling
    public LayerMask edgeLayer;
    private bool onEdge;
    //Checking if player is on a wall
    public Transform wallCheckPoint;
    public float wallCheckRadius;
    public LayerMask wallLayer;
    private bool isTouchingWall;

    private bool jumpAllowed;
    private bool wallJumpAllowed;
    private bool specialAbilityAllowed;

    private Animator playerAnimation;
    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        playerAnimation = GetComponent<Animator>();


    }

    // Update is called once per frame
    void Update()
    {
        isTouchingCeiling = Physics2D.OverlapCircle(ceilingCheckPoint.position, ceilingCheckRadius, ceilingLayer);
        onEdge = Physics2D.OverlapCircle(ceilingCheckPoint.position, ceilingCheckRadius, edgeLayer);
        isTouchingGround = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);
        isTouchingWall = Physics2D.OverlapCircle(wallCheckPoint.position, wallCheckRadius, wallLayer);

        movement = Input.GetAxis("Horizontal");

        Vector3 characterScale = transform.localScale;
        if (movement > 0f)
        {
            Run();
            //flipping the sprite horizontally
            characterScale.x = 5f;
        }
        else if(movement < 0f)
        {
            Run();
            //flipping the sprite horizontally
            characterScale.x = -5f;
        }
        if (isTouchingGround)
        {
            jumpAllowed = true;
            specialAbilityAllowed = false;
        }
        if (isTouchingWall)
        {
            rigidBody.velocity = new Vector2(0, 0);
            jumpAllowed = true;
            specialAbilityAllowed = false;
        }
        if (Input.GetButtonDown("Jump") && isTouchingWall)
        {
            jumpAllowed = false;
            specialAbilityAllowed = true;
            wallJumpAllowed = true;
            Invoke("StopWallJump", 0.08f);
        }
        if (wallJumpAllowed && characterScale.x == 5f)
        {
            WallJumpLeft();
        }
        else if(wallJumpAllowed && characterScale.x == -5f)
        {
            WallJumpRight();
        }
        if (Input.GetButtonDown("Jump") && jumpAllowed && specialAbilityAllowed == false && isTouchingGround)
        {
            jumpAllowed = false;
            specialAbilityAllowed = true;
            Jump();
        }
       
        if (specialAbilityAllowed && Input.GetButtonDown("Jump") && jumpAllowed == false && isTouchingGround == false && isTouchingCeiling == false && isTouchingWall == false)
        {
            Stick();
            characterScale.y = -5f;
            specialAbilityAllowed = false;
        }
        
        else if(isTouchingCeiling && Input.GetButtonDown("Jump"))
        {
            Fall();
            characterScale.y = 5f;
        }
        else if (onEdge)
        {
            EdgeDrop();
            characterScale.y = 5f;
        }
        

        transform.localScale = characterScale;
        

        playerAnimation.SetFloat("Speed", Mathf.Abs(rigidBody.velocity.x));
        playerAnimation.SetBool("OnGround", isTouchingGround);
        playerAnimation.SetBool("OnCeiling", isTouchingCeiling); 
        playerAnimation.SetBool("OnWall", isTouchingWall);
    }
    void Run()
    {
        rigidBody.velocity = new Vector2(movement * speed, rigidBody.velocity.y);
    }

    void Jump()
    {
        rigidBody.velocity = new Vector2(rigidBody.velocity.x, jumpSpeed);
    }

    void Stick()
    {
        rigidBody.gravityScale *= -15;
        specialAbilityAllowed = false;
    }

    void Fall()
    {
        rigidBody.gravityScale = 1;
    }
    void EdgeDrop()
    {
        rigidBody.gravityScale = 1;
    }
    void WallJumpLeft()
    {
        rigidBody.velocity = new Vector2(-wallJumpSpeed, jumpSpeed);
    }
    void WallJumpRight()
    {
        rigidBody.velocity = new Vector2(wallJumpSpeed, jumpSpeed);
    }
    void StopWallJump()
    {
        wallJumpAllowed = false;
    }
}