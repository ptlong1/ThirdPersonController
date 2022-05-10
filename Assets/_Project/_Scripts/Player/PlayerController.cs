using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;
using Cinemachine;
using System.Runtime.InteropServices;
using System.Linq;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : NetworkBehaviour 
{
#if UNITY_WEBGL && !UNITY_EDITOR
	[DllImport("__Internal")]
	private static extern void ConnectRoom(string str);
#endif
    [Header("Player")]
    public float moveSpeed = 3f;
    public float sprintSpeed = 6f;
    [Range(0f,3f)]
    public float rotationSmoothTime = 0.12f;
    public float speedChangeRate = 10f;

    [Space(10f)]
    public float jumpHeight = 1.2f;
    public float gravity = -15f;

    [Tooltip("Time before jump again. Set to 0 for instantly jumping again")]
    public float jumpTimeout = 0.5f;
    [Tooltip("Time required to enter fall state")]
    public float fallTimeout = 0.15f;

    public bool grounded;
    [Tooltip("Useful for rough ground")]
    public float groundOffset = -0.14f;
    public float groundRadius = 0.28f;  
    public LayerMask groundLayerMask;

    [Header("Camera Settings")]
    public Transform playerCameraRoot;
    CinemachineFreeLook freeLookCamera;

	[Header("Voice Room")]
	[SyncVar] public string roomName;

    //Player 
    float speed;
    float animationBlend;
    float targetRotation;
    float rotationVelocity;
    float verticalVelocity;
    float terminalVelocity = 53f;

    Camera mainCamera;
    Animator animator;
    bool hasAnimator;

    CharacterController characterController;
    ThirdPersonControllerInputs inputs;
    PlayerInput playerInput;
    //animation IDs
    int animIDSpeed;
    int animIDJump;
    int animIDGrounded;
    int animIDFreeFall;
    int animIDMotionSpeed;

    //timeout deltatime
    float fallTimeoutDelta;
    float jumpTimeoutDelta;

    //results collider
    Collider[] results = new Collider[10];

    private void Awake() {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        hasAnimator = TryGetComponent(out animator);
        characterController = GetComponent<CharacterController>();
        inputs = GetComponent<ThirdPersonControllerInputs>();
        playerInput = GetComponent<PlayerInput>();

        //camera;

        freeLookCamera = FindObjectOfType<CinemachineFreeLook>();
        AssignAnimationIDs();
    }
    public override void OnStartClient()
    {
        if (isLocalPlayer)
        {
		#if UNITY_WEBGL && !UNITY_EDITOR
			string shortName = roomName.Split('/').Last();
			ConnectRoom(shortName);
		#endif
            freeLookCamera.Follow = playerCameraRoot;
            freeLookCamera.LookAt = playerCameraRoot;
        }
        else
        {
            playerInput.enabled = false;
        }
    }

    void AssignAnimationIDs()
    {
        animIDFreeFall = Animator.StringToHash("FreeFall");
        animIDSpeed = Animator.StringToHash("Speed");
        animIDGrounded = Animator.StringToHash("Grounded");
        animIDJump = Animator.StringToHash("Jump");
        animIDMotionSpeed= Animator.StringToHash("MotionSpeed");
    }

    [ClientCallback]
    void Update()
    {
        if (isLocalPlayer)
        {
            JumpAndGravity();
            GroundedCheck();
            Move(); 
        }
    }

    void JumpAndGravity()
    {
        if (grounded)
        {
            //reset fall timeout timer
            fallTimeoutDelta = fallTimeout;

            //if has animator, update 
            if (hasAnimator)
            {
                animator.SetBool(animIDFreeFall, false);
                animator.SetBool(animIDJump, false);
            }

            //stop velocity drop to infinity
            if (verticalVelocity < 0f)
            {
                verticalVelocity = -2f;
            }

            //jump
            if (inputs.jump && jumpTimeoutDelta <= 0f)
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight*-2f*gravity);

                //update animation if has one
                if (hasAnimator)
                {
                    animator.SetBool(animIDJump, true);
                }
            }

            if (jumpTimeoutDelta >= 0f)
            {
                jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            jumpTimeoutDelta = jumpTimeout;

            if (fallTimeoutDelta >= 0f)
            {
                fallTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                if (hasAnimator)
                {
                    animator.SetBool(animIDFreeFall, true);
                }
            }
        }
        if (verticalVelocity < terminalVelocity)
        {
            verticalVelocity += gravity*Time.deltaTime;
        }
    }

    void GroundedCheck()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - groundOffset, transform.position.z);
        PhysicsScene physicsScene = gameObject.scene.GetPhysicsScene();
        int numColliders = physicsScene.OverlapSphere(
            spherePosition, 
            groundRadius, 
            results,
            groundLayerMask, 
            QueryTriggerInteraction.UseGlobal);
        grounded = numColliders > 0;
        bool collided = physicsScene.Raycast(spherePosition, Vector3.down, groundRadius, groundLayerMask);
        // Debug.Log("Number Colliders" +numColliders);
        // Debug.Log("Check Grounded " +grounded);
        // Debug.Log("Raycast " +collided);
        // Debug.Log("Charactor controller "+ characterController.isGrounded);
        if (hasAnimator)
        {
            animator.SetBool(animIDGrounded, grounded);
        }
    }

    void Move()
    {
        //Calculate targetspeed
        float targetSpeed = inputs.sprint?sprintSpeed:moveSpeed;
        //if no inputs, targetspeed = 0
        if (inputs.move == Vector2.zero) targetSpeed = 0f;

        //referece to current horizontal speed
        float currentHorizontalSpeed = new Vector3(characterController.velocity.x, 0f, characterController.velocity.z).magnitude;

        float speedOffset = 0.1f;
        float inputMagnitude = inputs.analogMovement?inputs.move.magnitude:1f;
        if (currentHorizontalSpeed <targetSpeed - speedOffset || currentHorizontalSpeed >targetSpeed + speedOffset)
        {
            speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed, speedChangeRate*Time.deltaTime);
            speed = Mathf.Round(speed*1000f)/1000f;
        }
        else
        {
            speed = targetSpeed;
        }
        animationBlend = Mathf.Lerp(animationBlend, targetSpeed, Time.deltaTime*speedChangeRate);

        //normalize input direction
        Vector3 normalizedDirection = new Vector3(inputs.move.x, 0f, inputs.move.y);

        //if there is a input, rotate player to where it's going when player moving
        if (inputs.move != Vector2.zero)
        {
            //rotation relative to camera rotation
            targetRotation = Mathf.Atan2(normalizedDirection.x, normalizedDirection.z)*Mathf.Rad2Deg + mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, rotationSmoothTime);

            transform.eulerAngles = new Vector3(0f, rotation, 0f);
        }

        Vector3 targetDirection = Quaternion.Euler(0f, targetRotation, 0f)*Vector3.forward;
        characterController.Move(targetDirection.normalized*(speed*Time.deltaTime) + new Vector3(0f, verticalVelocity, 0f)*Time.deltaTime);

        if (hasAnimator)
        {
            animator.SetFloat(animIDSpeed, animationBlend);
            animator.SetFloat(animIDMotionSpeed, inputMagnitude);
        }

    }

    // [Server]
    // public string GetPlayerName()
    // {
    //     return connectionToClient.connectionId.ToString();
    // }
}
