using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMove : MonoBehaviour
{
    public Camera playerCam;
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float jumpPwr = 7f;
    public float gravity = 9.8f;

    public float lookSpeed = 2f;
    public float lookXLim = 45f;

    Vector3 moveDir = Vector3.zero;
    float rotX = 0;

    public bool canMove = true;

    CharacterController charController;

    // Start is called before the first frame update
    void Start()
    {
        charController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0;
        float moveDirY = moveDir.y;
        moveDir = (forward * curSpeedX) + (right * curSpeedY);

        if(Input.GetButton("Jump") && canMove && charController.isGrounded)
        {
            moveDir.y = jumpPwr;
        }
        else
        {
            moveDir.y = moveDirY;
        }

        if(!charController.isGrounded)
        {
            moveDir.y -= gravity * Time.deltaTime;
        }

        charController.Move(moveDir * Time.deltaTime);

        if(canMove)
        {
            rotX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotX = Mathf.Clamp(rotX, -lookXLim, lookXLim);
            playerCam.transform.localRotation = Quaternion.Euler(rotX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
