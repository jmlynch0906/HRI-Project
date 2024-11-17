using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonControls : MonoBehaviour
{
    public float WalkSpeed = 5;
    public float GroundCheckDistance = 1.5f;
    public float SensitivityX = 1f;
    public float SensitivityY = 1f;
    public float MinY = -90f;
    public float MaxY = 90f;
    public Transform PlayerCam;
    public float gravity = -9.8f;
    private CharacterController characterController;
    private float verticalRotatiton = 0f;
    private Vector3 velocity;

    private bool m_CanRotate = false;


    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        //Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        UpdateMovement();
        UpdateLookRotation();
    }

    private void UpdateMovement()
    {
        //movement
        float horizontalMovement = Input.GetAxis("Horizontal");
        float verticalMovement = Input.GetAxis("Vertical");
        Vector3 moveDirection = transform.forward * verticalMovement + transform.right * horizontalMovement;

        characterController.Move(moveDirection*WalkSpeed*Time.deltaTime);

        if (!IsGrounded())
        {
            velocity.y += gravity * Time.deltaTime;
        }
        else
        {
            velocity.y = 0f;
        }

        characterController.Move(velocity * Time.deltaTime);
    }

    private void UpdateLookRotation()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            m_CanRotate = !m_CanRotate;
        }

        //cameraMovement
        if (PlayerCam != null)
        {
            if (!m_CanRotate)
            {
                return;
            }

            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");
            verticalRotatiton -= mouseY;
            verticalRotatiton = Mathf.Clamp(verticalRotatiton, MinY, MaxY);
            PlayerCam.localRotation = Quaternion.Euler(verticalRotatiton, 0f, 0f);
            transform.Rotate(Vector3.up * mouseX);
        }
    }

    bool IsGrounded(){
        RaycastHit hit;
        if(Physics.Raycast(transform.position,Vector3.down,out hit,GroundCheckDistance)){
            return true;
        }
        return false;
    }

}
