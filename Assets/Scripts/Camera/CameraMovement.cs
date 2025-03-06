using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    #region Singleton
    public static CameraMovement Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.Log("Deleted PlayerInteract dublicate");
            Destroy(gameObject);
        }
    }
    #endregion

    //[Header("Movement")]
    public float moveSpeed;
    public float riseFallSpeed;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;
    float elevationInput;
    bool fastInput;


    Vector3 moveDirection;

    Rigidbody rb;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        
    }

    // Update is called once per frame
    void Update()
    {

        Inputs();

        if (fastInput == true)
        {
            moveSpeed = 16f;
        }
        else
        {
            moveSpeed = 8f;
        }

        MovePlayer();
        Elevation();
        
    }

    private void FixedUpdate()
    {
        //MovePlayer();

    }
    void Inputs()
    {


        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        fastInput = Input.GetButton("Fast");

        if (Input.GetButtonUp("Fast"))
        {
            rb.linearVelocity = Vector3.zero;
        }

        elevationInput = 0;
        if (Input.GetButton("Down"))
        {
            elevationInput = -2f;
        }
        
        if (Input.GetButton("Jump"))
        {
            elevationInput = 2f;
        }
        

        
    }

    void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        transform.position = transform.position + moveDirection * (moveSpeed * Time.deltaTime);

    }

    void Elevation()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y + elevationInput * (moveSpeed * Time.deltaTime), transform.position.z);
    }



}
