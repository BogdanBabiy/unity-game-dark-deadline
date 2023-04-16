using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }
    
    [SerializeField] private float moveSpeed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private Transform mouseTarget;
    [SerializeField] private Rigidbody rigidBody;
    [SerializeField] private bool moveWithForce;
    [SerializeField] private LayerMask layerMask;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one instance of type Player");
        }
        Instance = this;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!moveWithForce) HandleMovement();
    }

    private void FixedUpdate()
    {
        if(moveWithForce) HandleMovementWithForce();
    }

    private void HandleMovement()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalised();
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);
        Vector3 lookDir = (mouseTarget.position - transform.position).normalized;
        
        float playerRadius = .7f;
        float playerHeight = 2f;
        float moveDistance = moveSpeed * Time.deltaTime;
        bool canMove = !Physics.CapsuleCast(transform.position,transform.position + Vector3.up * playerHeight, playerRadius,moveDir, moveDistance,layerMask);
        
        if (!canMove)
        {
            // Attempt only X-axis movement
        
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
            canMove = (moveDir.x < -.5f || moveDir.x > +.5f) && !Physics.CapsuleCast(transform.position,transform.position + Vector3.up * playerHeight, playerRadius,moveDirX, moveDistance,layerMask);
        
            if (canMove)
            {
                moveDir = moveDirX;
            }
            else
            {
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
                canMove = (moveDir.z < -.5f || moveDir.z > +.5f) && !Physics.CapsuleCast(transform.position,transform.position + Vector3.up * playerHeight, playerRadius,moveDirZ, moveDistance,layerMask);
        
                if (canMove)
                {
                    moveDir = moveDirZ;
                }
            }
        }
        if (canMove)
        {
            transform.position += moveDir * moveDistance;
        }
        

        // isWalking = moveDir != Vector3.zero;
        float rotateSpeed = 10f;
        lookDir.y = 0f;
        transform.forward =Vector3.Slerp(transform.forward, lookDir,Time.deltaTime * rotateSpeed);
    }

    private void HandleMovementWithForce()
    {       
        Vector2 inputVector = gameInput.GetMovementVectorNormalised();
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);
        Vector3 lookDir = (mouseTarget.position - transform.position).normalized;
        
        float moveDistance = moveSpeed * Time.deltaTime;
        Vector3 velocity = (moveDir * moveDistance) - rigidBody.velocity;

        Vector3.ClampMagnitude(velocity, maxSpeed);

        
        rigidBody.AddForce(velocity,ForceMode.VelocityChange);
        
        float rotateSpeed = 10f;
        lookDir.y = 0f;
        transform.forward =Vector3.Slerp(transform.forward, lookDir,Time.deltaTime * rotateSpeed);
    }
}
