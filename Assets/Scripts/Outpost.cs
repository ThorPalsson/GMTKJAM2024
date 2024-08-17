using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.UI;

public class Outpost : MonoBehaviour
{

    [SerializeField] private Transform cameraPostion;
    private bool movingCamera;
    private Transform cameraTransform;

    [SerializeField] private GameObject[] houseUi; 
    [SerializeField] private GameObject parkUi; 
    [Header("Camera Movement")]
    [SerializeField] private float cameraMoveSpeed;
    [SerializeField] private float camearRotationSpeed;

    private CameraFollow truckCamera;

    private TruckController truck;

    [SerializeField] private Button leaveOutpost;
    [SerializeField] private GameObject CargoPanel;


    private bool hasTakenCargo; 

    
    void Start()
    {
        cameraTransform = ReferenceManager.Instance.CameraTransform;
        truckCamera = ReferenceManager.Instance.Camera;
        truck = ReferenceManager.Instance.Truck;

        //leaveOutpost.gameObject.SetActive(false);
        leaveOutpost.onClick.AddListener(() => LeaveTown());
    }

    void LateUpdate()
    {
        if (movingCamera) CameraToOutpost();
    }


    public void ParkCar(Cargo[] cargos)
    {
        truck.ParkCar();
        int values = 0;

        foreach (var c in cargos)
        {
            values += c.cargoValue; 
            c.DestroyCargo(true);
        }

        print($"Brought {values}$ worth of cargo");

        ReferenceManager.Instance.gameManager.AddMoney(values);
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
            ToggleUI(true);
        }

    }

    private void CameraToCar()
    {
        truckCamera.FollowCar = true;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void LeaveTown()
    {
        if (!hasTakenCargo)
        {
            CargoPanel.SetActive(true);
            hasTakenCargo = true;
        }
        
        truck.UnParkCar();
        ToggleUI(false);
        CameraToCar();

        Time.timeScale = 0; 
           
    }

    private void ToggleUI(bool value)
    {
        leaveOutpost.gameObject.SetActive(value);
        parkUi.SetActive(value);

        foreach(var h in houseUi)
        {
            h.SetActive(value);
        }
    }

}
