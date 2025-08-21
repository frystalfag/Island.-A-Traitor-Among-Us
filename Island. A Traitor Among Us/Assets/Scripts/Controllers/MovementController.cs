using Mirror;
using UnityEngine;

public class MovementController : NetworkBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed = 9f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 2f;
    
    [Header("Network Settings")]
    [SerializeField] private float sendRate = 20f;
    [SerializeField] private float interpolationRate = 15f;
    [SerializeField] private float interpolationThreshold = 0.1f;
    
    private CharacterController characterController;
    private Vector3 velocity;
    private Vector3 lastSentPosition;
    private Vector3 lastMoveDirection;
    private float lastSendTime;
    private bool isGrounded;
    
    [SyncVar]
    private Vector3 networkPosition;
    [SyncVar] 
    private Vector3 networkVelocity;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        if (isServer) { networkPosition = transform.position; }
    }
    
    
    
}
