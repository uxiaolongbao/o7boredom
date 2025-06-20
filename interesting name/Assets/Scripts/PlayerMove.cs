using System.Collections;
using System.Numerics;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMove : MonoBehaviour
{
    //movement
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower;
    private Rigidbody2D body;
    private BoxCollider2D boxCollider;
    private float wallJumpCooldown;
    private float horizontalInput;
    [SerializeField] private float wallSlideSpeed = 7f;
    [SerializeField] private float wallJumpHorizontalForce = 15f;
    private bool isWallJumping;
    private int lastWallDirection; // 1=right, -1=left
    private bool wasOnWallLastFrame;
    //collision
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    //animation
    public Animator animator;
    //coyote time
    [SerializeField] private float coyoteTime = 0.1f;
    private float coyoteTimeCounter;
    //variable jump height
    [SerializeField] private float jumpHoldMultiplier = 0.5f;
    [SerializeField] private float minJumpHeight = 3f;
    private bool isHoldingJump;
    //multiple jumps
    [SerializeField] private int extraJumps;
    //dash
    private bool isDashing;
    [SerializeField] private float dashingPower;
    [SerializeField] private float dashingTime;
    [SerializeField] private float dashingCooldown;
    [SerializeField] private int dashNumber;
    private int dashCounter;
    private int jumpCounter;
    //Slippery make player uncontrollable
    private bool isSlipping = false;
    private float slipMomentum = 0f;
    [SerializeField] float slipMultiplier = 0.2f;       // Controls how slow you move on slippery surfaces
    [SerializeField] float slipDecayRate = 0.1f;        // Controls how slowly you stop sliding
    [SerializeField] float slipAcceleration = 0.2f;
    [SerializeField] private float overslideStrength = 2f; // How hard it pushes after releasing input
    [SerializeField] private float overslideDecay = 1f;    // How fast it fades out
    private float overslideMomentum = 0f;

    //corner checking
    [SerializeField] private float cornerPushForce = 2f;
    [SerializeField] private float wallStickThreshold = 0.1f;

    //disable movement
    private bool canMove = true;

    //ladder movement
    [SerializeField] private float ladderClimbSpeed = 3f;
    private bool onLadderZone = false;
    private bool isOnLadder = false;
    private float ladderInput = 0f;
    private float defaultGravity;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    //this is for ladder
    private void Start()
    {
        body = GetComponent<Rigidbody2D>();
        defaultGravity = body.gravityScale; 
    }
    private void Update()
    {
        //Disable movement
        if (!canMove || isDashing || isWallJumping)
            return;

        if (isDashing)
        {
            return;
        }
        if (isWallJumping) 
            return;
        
        if (isGrounded() && onWall())
        {
            UnityEngine.Vector2 wallNormal = GetWallNormal();
            body.linearVelocity = new UnityEngine.Vector2(wallNormal.x * cornerPushForce, body.linearVelocity.y);
        }
        
        horizontalInput = Input.GetAxis("Horizontal");
        
        float slideSpeed = isSlipping ? speed * slipMultiplier : speed;
        float acceleration = isSlipping ? slipAcceleration : 20f;

        float targetVelocityX = 0f;

        if (isSlipping)
        {
            if (Mathf.Abs(horizontalInput) > 0.01f)
            {
                // Slowly steer while slipping, but still apply full control
                slipMomentum = Mathf.Lerp(slipMomentum, horizontalInput * slideSpeed, Time.deltaTime * slipAcceleration);

                // Reset overslide because input is active
                overslideMomentum = 0f;
            }
            else
            {
                // If no input, slowly decay momentum and apply overslide
                slipMomentum = Mathf.Lerp(slipMomentum, 0f, Time.deltaTime * slipDecayRate);

                if (Mathf.Abs(overslideMomentum) < 0.01f)
                {
                    // Trigger overslide only once when input stops
                    overslideMomentum = Mathf.Sign(slipMomentum) * overslideStrength;
                }

                // Fade out overslide
                overslideMomentum = Mathf.Lerp(overslideMomentum, 0f, Time.deltaTime * overslideDecay);
            }

            // Combine slip momentum and overslide for total movement
            targetVelocityX = slipMomentum + overslideMomentum;
        }
        else
        {
            // If not slipping, reset all momentum and allow full movement control
            overslideMomentum = 0f; // Ensure no lingering overslide
            slipMomentum = 0f; // Reset slip momentum
            targetVelocityX = horizontalInput * slideSpeed;
        }

        float newVelocityX = Mathf.Lerp(body.linearVelocity.x, targetVelocityX, Time.deltaTime * acceleration);
        body.linearVelocity = new UnityEngine.Vector2(newVelocityX, body.linearVelocity.y);

        animator.SetFloat("speed", Mathf.Abs(horizontalInput));

        //flip player when moving left and right (for animations/sprites) + movement
        if(horizontalInput > 0.01f)
            transform.localScale = UnityEngine.Vector3.one;
        else if (horizontalInput < -0.01f)
            transform.localScale = new UnityEngine.Vector3(-1, 1, 1);

        //coyote time
        if (isGrounded())
        {
            coyoteTimeCounter = coyoteTime;
            jumpCounter = extraJumps;
            dashCounter = 0;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        //flexible jump height
        isHoldingJump = Input.GetKey(KeyCode.Z);
        if (!isHoldingJump && body.linearVelocity.y > minJumpHeight)
        {
            body.linearVelocity = new UnityEngine.Vector2(body.linearVelocityX, minJumpHeight);
        }
        else if (isHoldingJump && !isGrounded() && !onWall())
        {
            body.gravityScale = jumpHoldMultiplier;
        }
        else{
            body.gravityScale = 5;
        }

        //wall jumping logic
        if(wallJumpCooldown > 0.2f)
        {
            
            if (onWall() && !isGrounded())
            {
                if (body.linearVelocityY < 0)
                {
                    body.linearVelocity = new UnityEngine.Vector2(body.linearVelocityX, -wallSlideSpeed);
                }
                body.gravityScale = 4;
                body.linearVelocity = UnityEngine.Vector2.zero; 
            }
            
            else
                body.gravityScale = 5;

            if (Input.GetKeyDown(KeyCode.Z))
            {
                Jump();
            }

        }
        else
            wallJumpCooldown += Time.deltaTime;

        if (isGrounded()) { }
        animator.SetBool("isJump", !isGrounded());
        // dash logic

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (!isGrounded())
            {
                dashCounter++;
                if (dashCounter <= dashNumber)
                {
                    animator.SetTrigger("Dash");
                    StartCoroutine(Dash());
                }
            }
            else
            {
                StartCoroutine(Dash());
                animator.SetTrigger("Dash");
            }

        } 
        animator.SetBool("isDash", isDashing);

        //ladder movement
        if (onLadderZone)
        {
            ladderInput = Input.GetAxisRaw("Vertical");
            isOnLadder = Mathf.Abs(ladderInput) > 0.1f;
        }
        else
        {
            ladderInput = 0f;
            isOnLadder = false;
        }

    }

    private void FixedUpdate()
    {
        //ladder movement
        if (isOnLadder)
        {
            body.gravityScale = 0f;
            body.linearVelocity = new UnityEngine.Vector2(body.linearVelocity.x, ladderInput * ladderClimbSpeed);
        }
        else if (!onLadderZone)
        {
            body.gravityScale = defaultGravity;
        }
    }
    //ladder
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            onLadderZone = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            onLadderZone = false;
            isOnLadder = false;
        }
    }

    //its literally jumping and all its variations duh. its called the jump method for a reason
    private void Jump()
    {
        if (coyoteTimeCounter <= 0 && !onWall() && jumpCounter <= 0) return;

        if (isGrounded())
        {

            body.linearVelocity = new UnityEngine.Vector2(body.linearVelocityX, jumpPower);
        }
        else if (onWall() && !isGrounded())
        {
            float pushDirection = -lastWallDirection;
            body.linearVelocity = new UnityEngine.Vector2(pushDirection * wallJumpHorizontalForce, jumpPower);
            transform.localScale = new UnityEngine.Vector3(pushDirection, 1, 1);

            isWallJumping = true;
            Invoke(nameof(ResetWallJump), 0.2f);

        } else
        {
            if (jumpCounter > 0)
            {
                body.linearVelocity = new UnityEngine.Vector2(body.linearVelocityX, jumpPower);
                jumpCounter--;
                dashCounter = 0;
            }
        }
        coyoteTimeCounter = 0;
    }

    private void ResetWallJump() => isWallJumping = false;

    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, UnityEngine.Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }

    private bool onWall()
    {
        float rayLength = boxCollider.bounds.extents.x + 0.1f;
    
        bool leftWall = Physics2D.Raycast(boxCollider.bounds.center, UnityEngine.Vector2.left, rayLength, groundLayer);
        bool rightWall = Physics2D.Raycast(boxCollider.bounds.center, UnityEngine.Vector2.right, rayLength, groundLayer);

        bool justTouchedWall = (leftWall || rightWall) && !wasOnWallLastFrame;
        wasOnWallLastFrame = (leftWall || rightWall);

        if (justTouchedWall)
        {
            jumpCounter = extraJumps;
            dashCounter = 0;
        }

        if (leftWall) lastWallDirection = -1;
        if(rightWall) lastWallDirection = 1;

        return leftWall || rightWall;
    }

    //Slipping
    public void SetSlipping(bool slip)
    {
        isSlipping = slip;
    }

    //help wall no go kaboom
    private UnityEngine.Vector2 GetWallNormal()
    {
        RaycastHit2D leftHit = Physics2D.Raycast(boxCollider.bounds.center, UnityEngine.Vector2.left, 
            boxCollider.bounds.extents.x + wallStickThreshold, wallLayer);
        RaycastHit2D rightHit = Physics2D.Raycast(boxCollider.bounds.center, UnityEngine.Vector2.right, 
            boxCollider.bounds.extents.x + wallStickThreshold, wallLayer);

        if (leftHit) return leftHit.normal;
        if (rightHit) return rightHit.normal;
        return new UnityEngine.Vector2(-Mathf.Sign(transform.localScale.x), 0);
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        float originalGravity = body.gravityScale;
        body.gravityScale = 0f;
        body.linearVelocity = new UnityEngine.Vector2(transform.localScale.x * dashingPower, 0);
        yield return new WaitForSeconds(dashingTime);
        body.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
    }

    public void DisableMovement(float duration)
    {
        StartCoroutine(DisableMovementRoutine(duration));
    }

    private IEnumerator DisableMovementRoutine(float duration)
    {
        canMove = false;
        yield return new WaitForSeconds(duration);
        canMove = true;
    }
}
