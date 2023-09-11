using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerMovement : MonoBehaviour
{
    private float horizontal;
    [SerializeField]
    private float speed = 8f;
    [SerializeField]
    private float jumpingPower = 12f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LineRenderer line;
    [SerializeField] private Rigidbody2D head, body;
    [SerializeField] private Transform mainTransform;
    private int rayAmount = 10;
    [SerializeField] private float rayDist = 1f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float gravityScale = 1f;
    private Vector2 groundNormal = Vector2.up; // Initialize with the player's up direction
    public float tailDist = 0.8f;
    private bool isJump = false, ignoreGravityIsTicking = false;
    private void Update()
    {
        GetInput();
        Vector2 globalUp = body.transform.TransformDirection(Vector2.up);
        Debug.DrawRay(body.position, globalUp * jumpingPower, Color.blue);
        if (Input.GetButtonDown("Jump"))
        {
            isJump = true;
        }

        line.SetPosition(0, head.position);
        line.SetPosition(1, body.position);
    }
    private void FixedUpdate()
    {
        Move();
        ClampBodyPositions();
        RotatePlayerToGround();
    }
    private void Move()
    {
        body.gravityScale = 0;
        if (IsGrounded())
        {
            Vector2 globalForward = body.transform.TransformDirection(Vector2.right);
            Vector2 moveDir = globalForward * horizontal * speed;
            Debug.DrawRay(body.position, moveDir, Color.green);
            body.velocity = moveDir;
            if (isJump)
            {
                Vector2 globalUp = body.transform.TransformDirection(Vector2.up);
                body.AddForce(globalUp * jumpingPower, ForceMode2D.Impulse);
                isJump = false;
            }
        }
        else
        {
            Vector2 moveDir = Vector2.right * horizontal * speed * 0.5f;
            Debug.DrawRay(body.position, moveDir, Color.yellow);
            body.AddForce(moveDir, ForceMode2D.Force);
            Vector2 gravityForce = Vector2.up * Physics2D.gravity.y * gravityScale;
            body.AddForce(gravityForce, ForceMode2D.Force);
        }

    }
    private void GetInput()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
    }
    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }
    private void ClampBodyPositions()
    {
        // head to body
        Vector2 dir = body.position - head.position;
        float dist = dir.magnitude;
        if (dist > tailDist)
        {
            head.position = body.position - dir.normalized * tailDist;
        }
    }
    private void RotatePlayerToGround()
    {
        List<RaycastHit2D> hits = new List<RaycastHit2D>();
        float angleStep = 360 / rayAmount;
        for (int ray = 0; ray < rayAmount; ray++)
        {
            float currentAngle = ray * angleStep;
            Vector2 rayDir = Quaternion.Euler(0, 0, currentAngle) * Vector2.up;
            RaycastHit2D hit = Physics2D.Raycast(body.position, rayDir, rayDist, groundLayer);
            Debug.DrawRay(body.position, rayDir * rayDist, Color.red);
            if (hit.collider != null)
                hits.Add(hit);
        }
        if (hits.Count > 0)
        {
            RaycastHit2D closestHit = hits.OrderBy(hit => hit.distance).First();
            Vector2 normalVector = closestHit.normal;

            // Calculate the target rotation based on the normal vector
            Quaternion targetRotation = Quaternion.FromToRotation(Vector3.up, normalVector);

            // Gradually interpolate the current rotation towards the target rotation
            body.transform.rotation = Quaternion.Slerp(body.transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }

}

