using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseTarget : MonoBehaviour
{
    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float maxDistanceFromPlayer;

    [SerializeField] private bool useControllerMovement = false;

    private Vector3 currentMousePosition;
    private Vector2 currentInputVector;
    
    // Start is called before the first frame update
    void Start()
    {
        currentMousePosition = Input.mousePosition;
    }

    // Update is called once per frame
    void Update()
    {
        if ((useControllerMovement || gameInput.IsUsingController()) && Input.mousePosition == currentMousePosition)
        {
            HandleMovementWithController();
            useControllerMovement = true;
            ClampTargetToRadiusFromPlayer();

        }
        else
        {
            useControllerMovement = false;
            currentMousePosition = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(currentMousePosition);
   
            if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, layerMask))
            {
                var raycastHitPoint = raycastHit.point;
                raycastHitPoint.y += 1.6f;
                transform.position = raycastHitPoint;
            }
        }
    }
    
    private void HandleMovementWithController()
    {
        Vector2 inputVector = gameInput.GetTargetMovementVectorNormalised();
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        float moveDistance = moveSpeed * Time.deltaTime;
        
        transform.position += moveDir * moveDistance;
    }

    private void ClampTargetDistanceFromPlayer()
    {
        Vector3 direction = Player.Instance.transform.position - transform.position;
        direction.y = 0f; // Keep the target on the same level as the player

        float distance = direction.magnitude;
        
        if (distance > maxDistanceFromPlayer)
        {
            direction.Normalize();
            transform.position = Player.Instance.transform.position - direction * maxDistanceFromPlayer;
        }
    }

    private void ClampTargetToRadiusFromPlayer()
    {
        Vector2 inputVector = gameInput.GetTargetMovementVectorNormalised();
        if(inputVector != Vector2.zero) currentInputVector = inputVector;
        
        float magnitude = currentInputVector.magnitude;
        Vector2 direction = currentInputVector.normalized;

        float x =  Player.Instance.transform.position.x + direction.x * maxDistanceFromPlayer * magnitude;
        float z =  Player.Instance.transform.position.z + direction.y * maxDistanceFromPlayer * magnitude;

        transform.position = new Vector3(x, transform.position.y, z);
    }
}
