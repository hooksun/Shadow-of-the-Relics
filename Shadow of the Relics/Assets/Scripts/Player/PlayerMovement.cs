using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : PlayerBehaviour
{
    public Rigidbody2D rb;
    public Camera cam;

    public float speed, accel, airAccel, jumpHeight, jumpGravity, fallGravity, diveGravity, JumpCooldown;
    public int airJumps, wallJumps;
    public Vector2 WallCheckPoint, WallCheckSize, WallJumpForce;
    public float WallSlideSpeed, MinWallSpeed, WallJumpStopMoveTime;
    public float grappleCastSpeed, grappleStartSpeed, grappleMinSpeed, grappleAccel, grappleDecel;
    public LineRenderer grapple;
    public int airDashes;
    public float DashDist, DashCooldown, DashStartSpeed, DashEndSpeed;
    public float groundCheckCooldown;
    public Vector2 groundCheckOffset, groundCheckSize;
    public LayerMask groundMask, wallMask;

    Transform groundTrans;
    IGround currentGround;

    Vector2 velocity, groundVelocity;
    bool isGrounded;
    float direction, directionY, groundCooldown, dashCooldown, jumpCooldown, wallJumpStopMove;
    int airJump, wallJump, airDash;

    float activeDir{get=>transform.localScale.x; set=>transform.localScale = new Vector3(value, transform.localScale.y, transform.localScale.z);}

    public void ChangeDirection(InputAction.CallbackContext ctx)
    {
        direction = ctx.ReadValue<float>();
        if(direction != 0)
            activeDir = direction;
    }

    public void ChangeDirectionY(InputAction.CallbackContext ctx)
    {
        directionY = ctx.ReadValue<float>();
    }

    public void JumpInput(InputAction.CallbackContext ctx)
    {
        if(!ctx.started || jumpCooldown > 0f)
            return;
        if(grappling)
        {
            if(grappleDist <= 0.01f && directionY >= 0f)
                rb.velocity = new Vector3(rb.velocity.x, Mathf.Sqrt(2 * jumpGravity * jumpHeight), 0f);
            CancelGrapple();
            return;
        }
        if(onWall != 0 && wallJump != wallJumps)
        {
            rb.velocity = new Vector2((float)onWall * WallJumpForce.x, WallJumpForce.y);
            wallJumpStopMove = WallJumpStopMoveTime;
            wallJump++;
            return;
        }
        if(!isGrounded)
        {
            if(velocity.y > 0f || airJump == airJumps)
                return;
            airJump++;
        }
        rb.velocity = new Vector3(rb.velocity.x, Mathf.Sqrt(2 * jumpGravity * jumpHeight), 0f);
        groundCooldown = groundCheckCooldown;
        jumpCooldown = JumpCooldown;
        isGrounded = false;
    }

    bool dashing;
    Vector2 dashVel, dashDir, dashDrag;
    public void DashInput(InputAction.CallbackContext ctx)
    {
        if(!ctx.started)
            return;

        if(!dashing && dashCooldown <= 0f)
        {
            if(!isGrounded && onWall == 0)
            {
                if(airDash == airDashes)
                    return;
                airDash++;
            }

            dashing = true;
            //dashDir = (Vector2.right*(onWall==0?(directionY==0f?activeDir:direction):-onWall) + Vector2.up*directionY).normalized;
            dashDir = Vector2.right * activeDir;
            dashVel = dashDir * DashStartSpeed;
            dashDrag = dashDir * (DashStartSpeed - DashEndSpeed) / (2f * DashDist / (DashStartSpeed + DashEndSpeed));
            dashCooldown = DashCooldown;
        }
    }

    bool grappling, grappleAcceling;
    float grapplingSpeed, grappleCast;
    GrapplePoint grapplePoint;
    public void GrappleInput(InputAction.CallbackContext ctx)
    {
        if(!ctx.started)
            return;

        if(grappling)
        {
            CancelGrapple();
            return;
        }

        float sqrDist = Mathf.Infinity;
        grapplePoint = null;
        Vector3 cursorPosition = cam.ScreenToWorldPoint(Input.mousePosition);
        foreach(GrapplePoint p in GrapplePoint.Points)
        {
            float newSqrDist = Vector2.SqrMagnitude(cursorPosition - p.position);
            if(newSqrDist < sqrDist)
            {
                grapplePoint = p;
                sqrDist = newSqrDist;
            }
        }

        if(grapplePoint == null)
            return;

        grappling = true;
        grapple.gameObject.SetActive(true);
        grappleCast = 0f;
    }

    void CancelGrapple()
    {
        grappling = false;
        grapplePoint = null;
        grapple.gameObject.SetActive(false);
    }

    void FixedUpdate()
    {
        velocity = rb.velocity - groundVelocity;

        GroundCheck();
        Move();
        DoGravity();
        Grappling();
        WallJump();
        Dash();

        if(jumpCooldown > 0f)
            jumpCooldown -= Time.fixedDeltaTime;
        
        groundVelocity = (currentGround == null?Vector2.zero:currentGround.velocity);
        rb.velocity = velocity + groundVelocity;

        player.Anim.SetFloat("horizontal velocity", Mathf.Abs(velocity.x));
        player.Anim.SetBool("is grounded", isGrounded);
    }

    void Update()
    {
        if(grappling)
        {
            if(grappleCast != 1f)
            {
                grappleCast = Mathf.MoveTowards(grappleCast, 1f, grappleCastSpeed * Time.deltaTime);
                if(grappleCast == 1f)
                {
                    grappleDist = (grapplePoint.position - transform.position).magnitude;
                    grapplingSpeed = Mathf.Min(grappleStartSpeed, Mathf.Sqrt(grappleDist * 2f * grappleDecel));
                    grappleAcceling = grapplingSpeed == grappleStartSpeed;
                }
            }

            grapple.SetPosition(0, transform.position);
            grapple.SetPosition(1, Vector2.Lerp(transform.position, grapplePoint.position, grappleCast));
        }
    }

    void ChangeGround(Transform newGround)
    {
        if(newGround == groundTrans)
            return;
        groundTrans = newGround;
        if(newGround != null)
            currentGround = newGround.GetComponent<IGround>();
    }

    void GroundCheck()
    {
        if(groundCooldown > 0f)
        {
            groundCooldown -= Time.fixedDeltaTime;
            return;
        }

        Collider2D newGround = Physics2D.OverlapBox((Vector2)transform.position + groundCheckOffset, groundCheckSize, 0f, groundMask);
        ChangeGround((newGround==null?null:newGround.transform));
        if(isGrounded != newGround)
        {
            isGrounded = !isGrounded;
            if(isGrounded)
            {
                airJump = wallJump = airDash = 0;
            }
        }
    }

    void Move()
    {
        if(wallJumpStopMove > 0f)
        {
            wallJumpStopMove -= Time.fixedDeltaTime;
            return;
        }
        if(isGrounded)
        {
            velocity.x = Mathf.Clamp(Mathf.MoveTowards(velocity.x, direction * speed, accel * Time.fixedDeltaTime), -speed, speed);
            return;
        }
        float maxVel = Mathf.Max(speed, Mathf.Abs(velocity.x));
        velocity.x = Mathf.Clamp(velocity.x + direction * airAccel * Time.fixedDeltaTime, -maxVel, maxVel);

    }

    void DoGravity()
    {
        if(!isGrounded)
        {
            velocity.y -= (velocity.y > 0f?jumpGravity:(directionY<0f?diveGravity:fallGravity)) * Time.fixedDeltaTime;
        }
    }

    float grappleDist;
    void Grappling()
    {
        if(grappleCast != 1f || !grappling)
            return;
        
        if(grapplePoint == null)
        {
            grappling = false;
            return;
        }

        if(grappleDist <= 0.01f)
        {
            transform.position = (Vector2)grapplePoint.position;
            velocity = Vector2.zero;
            return;
        }

        Vector2 grappleVec = (grapplePoint.position - transform.position);
        grappleDist = grappleVec.magnitude;

        bool shouldDecel = grappleDist <= grapplingSpeed * 0.5f * (grapplingSpeed / grappleDecel);
        if(grappleAcceling && shouldDecel)
            grappleAcceling = false;

        grapplingSpeed += (grappleAcceling?grappleAccel:(shouldDecel?-grappleDecel:0f)) * Time.fixedDeltaTime;
        grapplingSpeed = Mathf.Max(grappleMinSpeed, grapplingSpeed);
        velocity = grappleVec.normalized * grapplingSpeed;
        grappleDist -= grapplingSpeed * Time.fixedDeltaTime;

        if(velocity.x != 0f)
            activeDir = Mathf.Sign(velocity.x);
    }

    int onWall;
    void WallJump()
    {
        if(dashing || isGrounded || velocity.y > MinWallSpeed || directionY < 0f)
        {
            onWall = 0;
            return;
        }
        
        Collider2D hit;
        onWall = HitWall(out hit);
        if(groundTrans == null && onWall != 0)
        {
            ChangeGround(hit.transform);
        }

        if(onWall != 0)
        {
            velocity.y = WallSlideSpeed;
            return;
        }
    }

    int HitWall(out Collider2D hit)
    {
        hit = null;
        if(direction > 0f)
            hit = Physics2D.OverlapBox((Vector2)transform.position + WallCheckPoint, WallCheckSize, 0f, wallMask);
        if(hit)
            return 1;
        if(direction < 0f)
            hit = Physics2D.OverlapBox((Vector2)transform.position + WallCheckPoint * (Vector2.left + Vector2.up), WallCheckSize, 0f, wallMask);
        if(hit)
            return -1;
        return 0;
    }

    void Dash()
    {
        if(!dashing)
        {
            if(dashCooldown > 0f)
                dashCooldown -= Time.fixedDeltaTime;
            return;
        }
        
        dashVel -= dashDrag * Time.fixedDeltaTime;
        velocity = dashVel;
        if(Vector2.Dot(dashVel, dashDir) <= DashEndSpeed)
        {
            dashing = false;
        }
    }
}

public interface IGround
{
    Vector2 velocity{get;}
}