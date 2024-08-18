using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.UI;

public class Outpost : MonoBehaviour
{

    public string OutpostName; 
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


    [SerializeField] private bool hasTakenCargo; 
    [SerializeField] private bool hasBroughtCargo; 

    [SerializeField] private Transform truckRespawn; 

    private Transform dialougeTransform;
    private bool inDialouge;

    private int moneyBrought; 

    
    void Start()
    {
        cameraTransform = ReferenceManager.Instance.CameraTransform;
        truckCamera = ReferenceManager.Instance.Camera;
        truck = ReferenceManager.Instance.Truck;

        //leaveOutpost.gameObject.SetActive(false);
        //leaveOutpost.onClick.AddListener(() => LeaveTown());
    }

    void LateUpdate()
    {
        if (movingCamera) CameraToOutpost();
        if (inDialouge) CameraToDialogue();
    }


    public void ParkCar(Cargo[] cargos)
    {
        truck.ParkCar();
        int values = 0;
        float weight = 0; 

        if (!hasBroughtCargo)
        {
            foreach (var c in cargos)
            {
                print ($"adding {c.name} with value of {c.cargoValue}"); 
                values += c.cargoValue; 
                weight += c.Weight; 
                c.DestroyCargo(true);
            }
            hasBroughtCargo = true;
        }

        print($"Brought {values}$ worth of cargo");

        leaveOutpost.onClick.AddListener(() => LeaveOutpost());
        moneyBrought = values; 
        ReferenceManager.Instance.gameManager.AddMoney(values, weight);
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
            ReferenceManager.Instance.statusPanel.ShowPanel(moneyBrought, this); 
            
        }
    }

    private void CameraToDialogue()
    {
        if (Vector3.Distance(cameraTransform.position, cameraPostion.position) > 0.05f)
        {
            cameraTransform.position = Vector3.MoveTowards(cameraTransform.position, dialougeTransform.position, cameraMoveSpeed * Time.deltaTime);
            cameraTransform.rotation = Quaternion.Slerp(cameraTransform.rotation, dialougeTransform.rotation, camearRotationSpeed * Time.deltaTime);
        }
    }

    public void StartDialogue(Transform cameraLocation)
    {
        dialougeTransform = cameraLocation; 
        inDialouge = true; 
        ToggleUI(false);
    }

    public void EndDialogue()
    {
        inDialouge = false;
        movingCamera = true;
    }

    private void CameraToCar()
    {
        truckCamera.FollowCar = true;

        if (!hasTakenCargo)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void LeaveOutpost()
    {
        var manager = ReferenceManager.Instance.gameManager; 
        manager.StoreTruckLocation(truckRespawn, true); 

        if (!hasTakenCargo)
        {
            CargoPanel.SetActive(true);
            hasTakenCargo = true;
            Time.timeScale = 0; 
        }

        truck.UnParkCar();
        ToggleUI(false);
        CameraToCar();
    }

    public void ToggleUI(bool value)
    {
        leaveOutpost.gameObject.SetActive(value);
        parkUi.SetActive(!value);

        foreach(var h in houseUi)
        {
            if (h != null)
                h.SetActive(value);
        }
    }

}
