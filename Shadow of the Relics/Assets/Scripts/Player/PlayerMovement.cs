using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : PlayerBehaviour
{
    public Rigidbody2D rb;
    public Camera cam;

    public float speed, accel, airAccel, jumpHeight, airJumpHeight, jumpGravity, fallGravity, diveGravity, JumpCooldown, damageParalyzedTime;
    public Vector2 damageKnockback;
    public int airJumps, wallJumps;
    public Vector2 WallCheckPoint, WallCheckSize, WallJumpDistance;
    public float WallSlideSpeed, MinWallSpeed, WallJumpStopMoveTime;
    public float grappleCastSpeed, grappleStartSpeed, grappleMinSpeed, grappleAccel, grappleDecel, maxPerchTime, minGrappleInputDistance;
    public Vector2 grapplePerchOffset;
    public LineRenderer grapple;
    public int airGrapples, airDashes;
    public float DashDist, DashCooldown, DashStartSpeed, DashEndSpeed;
    public float groundCheckCooldown;
    public Vector2 groundCheckOffset, groundCheckSize;
    public LayerMask groundMask, wallMask;
    public AudioPlayer RunAudio, GrappleHitAudio;

    [HideInInspector] public bool isGrounded;
    [HideInInspector] public Vector2 velocity;

    Transform groundTrans;
    IGround currentGround;

    Vector2 groundVelocity;
    
    float direction, directionY, groundCooldown, dashCooldown, jumpCooldown, wallJumpStopMove, perchTime;
    int airJump, wallJump, airDash, grapples;

    float activeDir{get=>(player.sprite.flipX?-1f:1f); set=>player.sprite.flipX = (value < 0f);}

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

    bool isJumping;
    public void JumpInput(InputAction.CallbackContext ctx)
    {
        if(ctx.performed)
            return;
        if(!ctx.started)
        {
            if(isJumping)
            {
                isJumping = false;
                if(jumpCooldown <= 0f)
                    rb.velocity = new Vector2(rb.velocity.x, Mathf.Min(Mathf.Sqrt(jumpGravity), rb.velocity.y));
            }
            return;
        }
        if(jumpCooldown > 0f || paralyzed > 0f || Time.timeScale == 0f)
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
            rb.velocity = new Vector2((float)onWall * -Mathf.Sqrt(2f * airAccel * WallJumpDistance.x), Mathf.Sqrt(2f * jumpGravity * WallJumpDistance.y));
            wallJumpStopMove = WallJumpStopMoveTime;
            wallJump++;
            return;
        }
        if(!isGrounded)
        {
            if(velocity.y > 0f || airJump == airJumps)
                return;
            airJump++;
            rb.velocity = new Vector3(rb.velocity.x, Mathf.Sqrt(2 * jumpGravity * airJumpHeight), 0f);
            return;
        }
        isJumping = true;
        jumpCooldown = JumpCooldown;
        rb.velocity = new Vector3(rb.velocity.x, Mathf.Sqrt(2 * jumpGravity * jumpHeight), 0f);
        groundCooldown = groundCheckCooldown;
        isGrounded = false;
    }

    bool dashing;
    Vector2 dashVel, dashDir, dashDrag;
    public void DashInput(InputAction.CallbackContext ctx)
    {
        if(!ctx.started || paralyzed > 0f || Time.timeScale == 0f)
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
        if(!ctx.started || paralyzed > 0f || Time.timeScale == 0f)
            return;

        if(grappling)
        {
            CancelGrapple();
            return;
        }
        if(grapples == airGrapples)
            return;

        float sqrDist = minGrappleInputDistance * minGrappleInputDistance;
        grapplePoint = null;
        Vector2 cursorPosition = cam.ScreenToWorldPoint(Input.mousePosition);
        foreach(GrapplePoint p in GrapplePoint.Points)
        {
            float newSqrDist = Vector2.SqrMagnitude(cursorPosition - p.position);
            if(newSqrDist < sqrDist && !Physics2D.Linecast(transform.position, p.position, groundMask | wallMask))
            {
                grapplePoint = p;
                sqrDist = newSqrDist;
            }
        }

        if(grapplePoint == null)
            return;

        grappling = true;
        grapple.gameObject.SetActive(true);
        player.animator.Play(player.animator.grappleAnim);
        GrappleHitAudio.Play();
        grappleCast = 0f;
    }

    void CancelGrapple()
    {
        grappling = false;
        grapplePoint = null;
        grappleCast = 0f;
        grapple.gameObject.SetActive(false);
        player.animator.Stop();
        player.animator.SetRotate(Vector2.zero);
        if(direction != 0f)
            activeDir = direction;
    }

    float paralyzed;
    public override void TakeDamage(float damage, Vector2 origin)
    {
        Vector2 knockback = damageKnockback;
        knockback.x *= Mathf.Sign(transform.position.x - origin.x);
        paralyzed = damageParalyzedTime;
        if(grappling)
            CancelGrapple();
        rb.velocity = knockback;
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

        if(isGrounded && velocity.x != 0f)
            RunAudio.Play();

        if(jumpCooldown > 0f)
        {
            jumpCooldown -= Time.fixedDeltaTime;
            if(jumpCooldown <= 0f && !isJumping)
                velocity = new Vector2(rb.velocity.x, Mathf.Min(Mathf.Sqrt(jumpGravity), velocity.y));
        }
        if(paralyzed > 0f)
            paralyzed -= Time.fixedDeltaTime;
        
        groundVelocity = (currentGround == null?Vector2.zero:currentGround.velocity);
        rb.velocity = velocity + groundVelocity;
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
                    grappleDist = (grapplePoint.position + grapplePerchOffset - player.position).magnitude;
                    grapplingSpeed = Mathf.Min(grappleStartSpeed, Mathf.Sqrt(grappleDist * 2f * grappleDecel));
                    grappleAcceling = grapplingSpeed == grappleStartSpeed;
                    grapples++;
                    perchTime = 0f;
                    if(grappleAcceling)
                    {
                        float accelingDist = grappleDist - grapplingSpeed * 0.5f * (grapplingSpeed / grappleDecel);
                        player.animator.GrappleRotateInit(accelingDist);
                    }
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

        if(grappling && grappleCast == 1f)
        {
            isGrounded = false;
            return;
        }

        Collider2D newGround = Physics2D.OverlapBox((Vector2)transform.position + groundCheckOffset, groundCheckSize, 0f, groundMask);
        ChangeGround((newGround==null?null:newGround.transform));
        if(isGrounded != newGround)
        {
            isGrounded = newGround;
            if(isGrounded)
            {
                airJump = wallJump = airDash = grapples = 0;
            }
        }
    }

    void Move()
    {
        if(paralyzed > 0f)
            return;
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
            if(velocity.y > 0f)
            {
                // float grav = jumpGravity;
                // if(isJumping)
                //     grav = (jumpGravity * jumpHeight) / maxJumpHeight;
                // velocity.y -= grav * Time.fixedDeltaTime;
                velocity.y -= jumpGravity * Time.fixedDeltaTime;
            }
            else
            {
                velocity.y -= (directionY<0f?diveGravity:fallGravity) * Time.fixedDeltaTime;
                isJumping = false;
            }
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
            transform.position = (Vector2)grapplePoint.position + grapplePerchOffset;
            velocity = Vector2.zero;
            player.animator.SetRotate(Vector2.zero);
            perchTime += Time.fixedDeltaTime;
            player.animator.Play(player.animator.perchAnim);
            if(perchTime >= maxPerchTime)
                CancelGrapple();
            return;
        }

        Vector2 grappleVec = (grapplePoint.position + grapplePerchOffset - player.position);
        grappleDist = grappleVec.magnitude;

        float accelingDist = grappleDist - grapplingSpeed * 0.5f * (grapplingSpeed / grappleDecel);
        bool shouldDecel = accelingDist <= 0f;
        if(grappleAcceling && shouldDecel)
            grappleAcceling = false;

        player.animator.GrappleRotate(grappleVec, accelingDist);

        grapplingSpeed += (grappleAcceling?grappleAccel:(shouldDecel?-grappleDecel:0f)) * Time.fixedDeltaTime;
        grapplingSpeed = Mathf.Max(grappleMinSpeed, grapplingSpeed);
        velocity = grappleVec.normalized * grapplingSpeed;
        grappleDist -= grapplingSpeed * Time.fixedDeltaTime;

        

        if(velocity.x != 0f)
            activeDir = Mathf.Sign(velocity.x);
    }

    [HideInInspector] public int onWall;
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