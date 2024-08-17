using UnityEngine;

public class Outpost : MonoBehaviour
{

    [SerializeField] private Transform cameraPostion;
    private bool movingCamera;
    private Transform cameraTransform;


    [Header("Camera Movement")]
    [SerializeField] private float cameraMoveSpeed;
    [SerializeField] private float camearRotationSpeed;

    private CameraFollow truckCamera;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cameraTransform = ReferenceManager.Instance.CameraTransform;
        truckCamera = ReferenceManager.Instance.Camera;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (movingCamera) CameraToOutpost();
    }



    public void ParkCar(Cargo[] cargos)
    {
        var truck = ReferenceManager.Instance.Truck;
        truck.canDrive = false;

        int values = 0;

        foreach (var c in cargos)
        {
            values += c.cargoValue; 
            c.DestroyCargo(true);
        }

        print($"Brough {values} of cargo");

        movingCamera = true;
    }

    private void CameraToOutpost()
    {
        if (truckCamera.FollowCar)
            truckCamera.FollowCar = false;
        
        cameraTransform.position = Vector3.MoveTowards(cameraTransform.position, cameraPostion.position, cameraMoveSpeed * Time.deltaTime);
        cameraTransform.rotation = Quaternion.Slerp(cameraTransform.rotation, cameraPostion.rotation, camearRotationSpeed * Time.deltaTime);

        if (Vector3.Distance(cameraTransform.position, cameraPostion.position) < 0.1f)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None; 
            movingCamera = false;
        }

    }


    private void CameraToCar()
    {
        truckCamera.FollowCar = true;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
