using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;


public enum Ground
{
    Hard,
    None
}

public class MVPlayerController : MonoBehaviour
{

    readonly Vector3 flippedScale = new Vector3(-1, 1, 1);

    [Header("Movement")]
    [SerializeField] float acceleration = 0.0f;
    [SerializeField] float maxSpeed = 0.0f;
    [SerializeField] float jumpForce = 0.0f;
    [SerializeField] float minFlipSpeed = 0.1f;
    [SerializeField] float jumpGravityScale = 1.0f;
    [SerializeField] float fallGravityScale = 1.0f;
    [SerializeField] float groundedGravityScale = 1.0f;
    [SerializeField] bool resetSpeedOnLand = false;
    [SerializeField] bool isCastingMagic = false;
    

    private Vector2 movementInput;
    private Rigidbody2D controllerRigidbody;
    private Collider2D controllerCollider;
    private LayerMask hardGroundMask;
    private LayerMask launchObjectMask;
    Vector2 prevVelocity;
    Ground ground;
    Camera mainCamera;

    bool jumpInput;
    bool isJumping;

    [SerializeField] Transform arm;
    [SerializeField] Transform puppet;
    private bool isFlipped = false;

    public ObjectOnLaunch objectToLaunch;
    
    public bool CanMove { get; set; }
    void Start()
    {   
        mainCamera = Camera.main;
        CanMove = true;
        controllerRigidbody = GetComponent<Rigidbody2D>();
        controllerCollider = GetComponent<Collider2D>();
        hardGroundMask = LayerMask.GetMask("Ground Hard");
        launchObjectMask = LayerMask.GetMask("Launch");

    }

    void Update()
    {
        var keyboard = Keyboard.current;
        var mouse = Mouse.current;
        if(!CanMove || keyboard == null)
            return;
        
        float moveHor = 0.0f;
        if(keyboard.rightArrowKey.isPressed || keyboard.dKey.isPressed)
           { 
            moveHor = 1.0f;}
        else if (keyboard.leftArrowKey.isPressed || keyboard.aKey.isPressed)
            moveHor = -1.0f;
        
        movementInput = new Vector2(moveHor,0);
        if(!isJumping && keyboard.spaceKey.wasPressedThisFrame)
        {
            jumpInput = true;
        }

        Vector2 screenPoint = mainCamera.ScreenToWorldPoint(mouse.position.ReadValue());
        Vector2 handDir = screenPoint - (Vector2)arm.position;
        if(!isCastingMagic)
            arm.right = isFlipped ? -handDir : handDir;

        if(mouse.leftButton.wasPressedThisFrame)
        {
            WandAnimation();
        }

        if(mouse.rightButton.isPressed)
        {   if(objectToLaunch == null)
            {  
                Collider2D[] objects = Physics2D.OverlapCircleAll(transform.position, 3f, launchObjectMask);
                var obj = FindClosest(objects);
               if(obj)
                    objectToLaunch = obj.GetComponent<ObjectOnLaunch>();
            }
            else 
            {
                
                objectToLaunch.CanLaunch = true;
            }
        }
        else if(objectToLaunch)
        {
            objectToLaunch.CanLaunch = false;
            objectToLaunch = null;
        }
    }

    Sequence WandAnimation()
    {
        isCastingMagic = true;

        Sequence s = DOTween.Sequence();
        s.Append(arm.DOPunchPosition(arm.right / 5, .2f, 10, 1));
        s.Join(arm.DOPunchRotation(new Vector3(0, 0, -45), .2f, 10, 1));
        s.AppendCallback(() => isCastingMagic = false);
        return s;
    }

    private void FixedUpdate() {
        UpdateVelocity();
        UpdateLayerMask();
        UpdateJump();
        UpdateGravityScale();
        UpdateDirection();
        prevVelocity = controllerRigidbody.velocity;
    }

    private void UpdateVelocity()
    {
        Vector2 velocity = controllerRigidbody.velocity;
        velocity += movementInput * acceleration * Time.fixedDeltaTime;
        movementInput = Vector2.zero;
        velocity.x = Mathf.Clamp(velocity.x, -maxSpeed, maxSpeed);
        controllerRigidbody.velocity = velocity;
    }

    private void UpdateLayerMask()
    {
        if(controllerCollider.IsTouchingLayers(hardGroundMask))
            ground = Ground.Hard;
        else
            ground = Ground.None;
    }

    private void UpdateJump()
    {
        if(ground == Ground.Hard && jumpInput)
        {
            controllerRigidbody.AddForce(new Vector2(0,jumpForce),ForceMode2D.Impulse);
            jumpInput = false;
            isJumping = true;
        }
        else if(isJumping && ground != Ground.None)
        {
            isJumping = false;
        }
       
    }

    private void UpdateGravityScale()
    {
        // Use grounded gravity scale by default.
        var gravityScale = groundedGravityScale;

        if (ground == Ground.None)
        {
            // If not grounded then set the gravity scale according to upwards (jump) or downwards (falling) motion.
            gravityScale = controllerRigidbody.velocity.y > 0.0f ? jumpGravityScale : fallGravityScale;
        }

        controllerRigidbody.gravityScale = gravityScale;
    }


    private void UpdateDirection()
    {
        // Use scale to flip character depending on direction
        if (controllerRigidbody.velocity.x > minFlipSpeed && isFlipped)
        {
            isFlipped = false;
            puppet.localScale = Vector3.one;
        }
        else if (controllerRigidbody.velocity.x < -minFlipSpeed && !isFlipped)
        {
            isFlipped = true;
            puppet.localScale = flippedScale;
        }
    }

    private Collider2D FindClosest(Collider2D[] colliders)
    {
        if(colliders.Length ==0)
            return null;
        float mimLeng = Vector2.Distance(colliders[0].transform.position, transform.position);
        Collider2D obj = colliders[0];
        for(int i = 1; i < colliders.Length;i++)
        {
            float lenToCollider = Vector2.Distance(transform.position, colliders[i].transform.position);
            if(lenToCollider < mimLeng)
            {
                mimLeng = lenToCollider;
                obj = colliders[i];
            }
        }

        return obj;
    }
}
