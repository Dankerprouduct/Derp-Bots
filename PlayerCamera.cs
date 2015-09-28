using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour
{

    Camera cam;
    public bool enableCam = true; 
    public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
    public RotationAxes axes = RotationAxes.MouseXAndY;
    //   public float sensitivityX = 15F;
    public float sensitivityY = 5F;

    //  public float minimumX = -360F;
    //   public float maximumX = 360F;

    public float minimumY = -60F;
    public float maximumY = 60F;

    //   float rotationX = 0F;
    float rotationY = 0F;

    Quaternion originalRotation;

    bool showSens = false;

    public void EnableCamera(bool cambl)
    {
        enableCam = cambl; 
    }

    public bool inVeh;
    public void InVehicle()
    {
        inVeh = !inVeh;
    }
    void Update()
    {
        if (GetComponent<NetworkView>().isMine)
        {
            if (!inVeh)
            {
                Cam();
            }
            GetComponent<Camera>().enabled = enableCam; 
        }
        else
        {
            GetComponent<Camera>().enabled = false;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            showSens = !showSens; 
        }
        if (inVeh)
        {
            transform.localRotation = Quaternion.Euler(6.4888f, 278.3469f, 263.3588f);
        }
    }

    void OnGUI()
    {
        if (GetComponent<NetworkView>().isMine)
        {
            if (showSens)
            {
                MouseYSensitivity();
            }
        }
    }

    void MouseYSensitivity()
    {
        sensitivityY = CompundControls.LabelSlider(new Rect(10, 400, 200, 20), sensitivityY, 5f, "Y Sensitivity");
    }

    void Start()
    {
        cam = this.GetComponent<Camera>(); 
        originalRotation = transform.localRotation;
    }


    void Cam()
    {
        if (showSens == false)
        {
            if (axes == RotationAxes.MouseXAndY)
            {
                // Read the mouse input axis
                //  rotationX += Input.GetAxis("Mouse X") * sensitivityX;
                rotationY += Input.GetAxis("Mouse Y") * sensitivityY;

                //    rotationX = ClampAngle(rotationX, minimumX, maximumX);
                rotationY = ClampAngle(rotationY, minimumY, maximumY);

                //   Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
                Quaternion yQuaternion = Quaternion.AngleAxis(rotationY, Vector3.left);

                transform.localRotation = originalRotation * yQuaternion;
            }
            else
            {
                rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
                rotationY = ClampAngle(rotationY, minimumY, maximumY);

                Quaternion yQuaternion = Quaternion.AngleAxis(rotationY, Vector3.left);
                transform.localRotation = originalRotation * yQuaternion;
            }
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
