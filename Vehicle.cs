using UnityEngine;
using System.Collections;

public class Vehicle : MonoBehaviour {

    public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
    public RotationAxes axes = RotationAxes.MouseXAndY;
    public float sensitivityX = 15F;
    Quaternion originalRotation;
    float rotationX = 0F;
    //public float sensitivityY = 15F;

    public float minimumX = -360F;
    public float maximumX = 360F;

	// Use this for initialization
    Rigidbody rigidBody;
    public int speed = 50;

    public bool onVehicle; 
    void Start ()
    {
       // onVehicle = true; 

        rigidBody = GetComponent<Rigidbody>();
        if (rigidBody)
            rigidBody.freezeRotation = true;
        originalRotation = transform.localRotation;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (onVehicle)
        {
            Movement();
            Cam();
        }

        

	}

    void Movement()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            transform.DetachChildren(); 
        }

        if (Input.GetKey(KeyCode.D))
        {
            rigidBody.position += (-1 * this.transform.right) * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.W))
        {
            rigidBody.position += this.transform.up * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            rigidBody.position += this.transform.right * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            rigidBody.position += (-1 * this.transform.up) * speed * Time.deltaTime;
        }

        

    }

    void Cam()
    {
        if (axes == RotationAxes.MouseXAndY)
        {
            // Read the mouse input axis
            rotationX += Input.GetAxis("Mouse X") * sensitivityX;
            //   rotationY += Input.GetAxis("Mouse Y") * sensitivityY;

            rotationX = ClampAngle(rotationX, minimumX, maximumX);
            //   rotationY = ClampAngle(rotationY, minimumY, maximumY);

            Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.forward);
            //  Quaternion yQuaternion = Quaternion.AngleAxis(rotationY, Vector3.left);

            transform.localRotation = originalRotation * xQuaternion;
        }
        else if (axes == RotationAxes.MouseX)
        {
            rotationX += Input.GetAxis("Mouse X") * sensitivityX;
            rotationX = ClampAngle(rotationX, minimumX, maximumX);

            Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.forward);
            transform.localRotation = originalRotation * xQuaternion;
        }
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}
