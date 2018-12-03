using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    public float walkAcceleration = 30;
    public float maxRunSpeed = 7;
    public float maxWalkSpeed = 4;
    public float gravity = 9.81f;
    public float jumpSpeed = 8;
    public float inAirDamping = 0.2f;
    public float wallImpactDamping = 0.8f;
    public float wallKickoffPercentage = 0.75f;
    public float jumpGravityDamping = 0.8f;
    public float gravityAcceleration = 0.5f * 9.81f;

    public bool hasJump = true;
    public bool hasLongJump = true;
    public bool hasRun = true;
    public bool hasWallJump = true;

    public bool debugDrawing = false;

    SpriteRenderer sr;
    Rigidbody2D rb;
    AudioSource audio;
    Vector3 startPos;
    
    bool inSpawnState;
    bool falling;
    bool wallJumpEnabled = false;
    bool jumpRemainsPressed = false;
    Vector2 wallNormal;
    float currentGravity;

    public void StandStill() {
        SetVelocity(0);
    }

    public void Respawn() {
        inSpawnState = true;
        transform.position = startPos;

        falling = false;
        wallJumpEnabled = false;
        jumpRemainsPressed = false;
    }

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        audio = GetComponent<AudioSource>();

        startPos = transform.position;

        currentGravity = gravity;

        Respawn();
    }

    void SetVelocity(float? x = null, float? y = null) {
        Vector2 velocity = rb.velocity;
        velocity.x = x ?? velocity.x;
        velocity.y = y ?? velocity.y;
        rb.velocity = velocity;
    }

    // Update is called once per frame
    void Update () {
        flipHeadProperly();

        float xVel = rb.velocity.x;
        float xInput = Input.GetAxisRaw("Horizontal");
        if (xInput != 0) {
            float acceleration = xInput * walkAcceleration * Time.deltaTime;

            if (falling) {
                acceleration *= inAirDamping;
            }

            float newXVel = xVel + acceleration;

            var maxMovementSpeed = maxWalkSpeed;
            if (hasRun && Input.GetButton("Sprint")) {
                maxMovementSpeed = maxRunSpeed;
            }

            SetVelocity(Mathf.Clamp(newXVel, -maxMovementSpeed, maxMovementSpeed));

            if (inSpawnState) {
                SetVelocity(maxMovementSpeed * 2);
                falling = true;
                inSpawnState = false;
            }
        } else if (xVel != 0){
            float deceleration = -Mathf.Sign(xVel) * walkAcceleration * Time.deltaTime;

            if (falling) {
                deceleration *= inAirDamping;
            }

            float newXVel = xVel + deceleration;
            if (Mathf.Sign(newXVel) == Mathf.Sign(xVel)) {
                SetVelocity(newXVel);
            } else {
                SetVelocity(0);
            }
        }
        
        if (falling && rb.velocity.y > 0) {
            if (jumpRemainsPressed) {
                SetVelocity(y: rb.velocity.y - currentGravity * Time.deltaTime);
            } else {
                currentGravity += gravityAcceleration * Time.deltaTime;
                SetVelocity(y: rb.velocity.y - currentGravity * Time.deltaTime);
            }
        } else if (falling && rb.velocity.y <= 0) {
            currentGravity += gravityAcceleration * Time.deltaTime;
            SetVelocity(y: rb.velocity.y - currentGravity * Time.deltaTime);
        } 

        currentGravity = Mathf.Min(gravity, currentGravity);

        if ((!falling || (wallJumpEnabled && hasWallJump)) && Input.GetButtonDown("Jump") && hasJump) {
            falling = true;

            if (hasLongJump) {
                jumpRemainsPressed = true;
            }

            currentGravity = gravity * jumpGravityDamping;
            SetVelocity(y: jumpSpeed);
            //AudioSource.PlayClipAtPoint(audio.clip, transform.position);
            audio.pitch = Random.Range(0.8f, 1.2f);
            audio.Play();

            if (wallJumpEnabled) {
                SetVelocity(maxWalkSpeed * wallKickoffPercentage * wallNormal.x);
                wallJumpEnabled = false;
            }
        }

        if (Input.GetButtonUp("Jump") && jumpRemainsPressed) {
            jumpRemainsPressed = false;
        }

        if (Input.GetKeyDown(KeyCode.R)) {
            transform.position = startPos;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            hasJump ^= true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            hasLongJump ^= true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3)) {
            hasRun ^= true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4)) {
            hasWallJump ^= true;
        }

        if (debugDrawing) {
            if (falling) {
                var offset = new Vector3(0, -0.25f, 0);
                Debug.DrawLine(offset + transform.position, offset + transform.position + Vector3.right * 0.25f, Color.red, 0.05f, false);
            }
            if (wallJumpEnabled) {
                Debug.DrawLine(transform.position, (Vector2) transform.position + wallNormal * 0.5f, Color.green, 0.05f, false);
            }
        }
    }

    void flipHeadProperly() {
        if (!wallJumpEnabled) {
            float xInput = Input.GetAxisRaw("Horizontal");
            if (xInput > 0) {
                sr.flipX = false;
            } else if (xInput < 0) {
                sr.flipX = true;
            }
        } else {
            if (wallNormal.x > 0) {
                sr.flipX = false;
            } else {
                sr.flipX = true;
            }
        }   
    }
    
    void OnCollisionEnter2D(Collision2D collider) {
        foreach (var contact in collider.contacts) {
            var normal = contact.normal;
            if (normal.IsAxisAligned()) {
                if (Mathf.Approximately(normal.x, 1) || Mathf.Approximately(normal.x, -1)) {
                    SetVelocity(y: rb.velocity.y * wallImpactDamping);
                    wallJumpEnabled = true;
                    wallNormal = normal;
                }
            }
        }
    }

    void OnCollisionStay2D(Collision2D collider) {
        if (debugDrawing) {
            foreach (var contact in collider.contacts) {
                DrawCross(contact.point);
                Debug.DrawLine(contact.point, contact.point + contact.normal * 0.25f, Color.white, 0.05f, false);
            }
        }

        bool floorSeen = false;

        foreach (var contact in collider.contacts) {
            var normal = contact.normal;
            if (normal.IsAxisAligned()) {
                if (Mathf.Approximately(normal.y, 1)) {
                    floorSeen = true;
                    falling = false;
                    wallJumpEnabled = false;
                }
                if (Mathf.Approximately(normal.x, 1) || Mathf.Approximately(normal.x, -1)) {
                    wallJumpEnabled = true;
                    wallNormal = normal;
                }
            }
        }

        if (!falling) wallJumpEnabled = false;

        if (!floorSeen) {
            falling = true;
        }
    }

    public void SetLookLeft(bool direction) {
        sr.flipX = direction;
    }
    
    public static void DrawCross(Vector2 pos) {
        Debug.DrawLine(pos - Vector2.one * 0.25f, pos + Vector2.one * 0.25f, Color.white, 0.05f, false);
        var v = new Vector2(-1, 1);
        Debug.DrawLine(pos - v * 0.25f, pos + v * 0.25f, Color.white, 0.05f, false);
    }

    void OnCollisionExit2D(Collision2D collider) {
        falling = true;
        wallJumpEnabled = false;
    }
}

public static class Vector2Extensions {
    public static bool IsAxisAligned(this Vector2 v) {
        return Mathf.Approximately(v.x, 0) || Mathf.Approximately(v.y, 0);
    }
}
