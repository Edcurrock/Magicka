using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RotationDemo : MonoBehaviour
{

    public Transform armIK;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var mouse = Mouse.current;

        Vector3 screenPos = Camera.main.ScreenToWorldPoint(mouse.position.ReadValue());
        Vector2 aimDirection = screenPos - armIK.position;
        armIK.right = aimDirection;
    }
}
