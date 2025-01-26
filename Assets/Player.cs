using Unity.Netcode;
using UnityEditor.Rendering.Universal;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

enum State 
{
    Nothing,
    Crank,
    Wheel,
    Lever,
    Gun
}
public struct UserInputStruct : INetworkSerializeByMemcpy {
    public float horizontalInput;
    public float verticalInput;
    public bool jump;
    public bool fire;
    public float mouseX;
    public float mouseY;
    public Vector3 mousePosition;

    public UserInputStruct(float horizontalInput, float verticalInput, bool jump, bool fire, float mouseX, float mouseY, Vector3 mousePosition) {
        this.horizontalInput = horizontalInput;
        this.verticalInput = verticalInput;
        this.jump = jump;
        this.fire = fire;
        this.mouseX = mouseX;
        this.mouseY = mouseY;
        this.mousePosition = mousePosition;
    }
}


public class Player : NetworkBehaviour
{
    public float speed = 10.0f;
    public float rotationSpeed = 10.0f;
    public float drag = 0.01f;

    public Transform cameraSlot;
    public Camera playerCamera;
    public ShipControl ship;
    
    private State state = State.Nothing;
    private Anchor targetedAnchor;
    private GunControl targetedGun;
    private Vector3 previousShipVelocity = Vector3.zero;
    private Vector3 previousShipAngularVelocity = Vector3.zero;
    
    Rigidbody rb;
    
    public NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>();
    public bool loaded = false;
    
    void Start() {
        rb = GetComponent<Rigidbody>();
        playerCamera = Camera.main;
    }

    void FixedUpdate()
    {
        playerCamera.transform.position = cameraSlot.position;
        playerCamera.transform.parent = cameraSlot;
        //&& GameNetworkManager.instance.raceTime > 6f
        if (!GameNetworkManager.instance.isFreeroam ) {
            var horizontalInput = Input.GetAxis("Horizontal");
            var verticalInput = Input.GetAxis("Vertical");
            var mouseX = Input.GetAxis("Mouse X");
            var mouseY = Input.GetAxis("Mouse Y");
            var handbrakeInput = Input.GetButton("Jump");
            var fire = Input.GetButton("Fire1");
            var mousePosition = Input.mousePosition;
            if (IsOwner && Application.isFocused)
                ControlCarServerRpc(new UserInputStruct(horizontalInput, verticalInput, handbrakeInput,fire, mouseX, mouseY, mousePosition));
        }
    }

    [ServerRpc]
    void ControlCarServerRpc(UserInputStruct t, ServerRpcParams rpcParams = default) {
        if (state != State.Gun)
        {
            rb.AddRelativeForce(Vector3.forward * t.verticalInput * speed, ForceMode.Acceleration);
            rb.AddRelativeTorque(Vector3.up * t.horizontalInput * rotationSpeed, ForceMode.Acceleration);
            Controls(t);
        }
        
        previousShipAngularVelocity = ship.rb.angularVelocity;
        previousShipVelocity = ship.rb.GetPointVelocity(transform.position);
        
        rb.linearVelocity = ship.rb.GetPointVelocity(transform.position) + (previousShipVelocity - rb.linearVelocity) * drag;
        rb.angularVelocity = ship.rb.angularVelocity + (previousShipAngularVelocity - rb.angularVelocity) * drag;
    }
    
    void Controls(UserInputStruct t)
    {
        if (t.fire)
        {
            Ray ray = playerCamera.ScreenPointToRay(t.mousePosition);
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(ray, out RaycastHit hit, 5f))
            { 
                //Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.yellow);
                if (hit.collider.gameObject.CompareTag("Crank")) state = State.Crank;
                if (hit.collider.gameObject.CompareTag("Wheel")) state = State.Wheel;
                if (hit.collider.gameObject.CompareTag("Lever"))
                {
                    targetedAnchor = hit.collider.gameObject.GetComponentInParent<Anchor>();
                    state = State.Lever;
                }
                if (hit.collider.gameObject.CompareTag("Gun"))
                {
                    targetedGun = hit.collider.gameObject.GetComponent<GunControl>();
                    playerCamera.enabled = false;
                    targetedGun.startShooting((() => {
                        playerCamera.enabled = true;
                        state = State.Nothing;
                    }));
                    state = State.Gun;
                }
            }
            else
            { 
                //Debug.DrawRay(ray.origin, ray.direction * 1000, Color.white); 
            }
            
            if (state == State.Crank)
            {
                ship.RotateCrank(t.mouseX);
            }
            if (state == State.Wheel)
            {
                ship.RotateWheel(ray);
            }
            if (state == State.Lever)
            {
                targetedAnchor.MoveLever((playerCamera.ScreenToViewportPoint(t.mousePosition).y-0.5f)*2f);
            }
            if (state == State.Gun)
            {
                targetedGun.MyFixedUpdate(t);
            }
        }
        else if (state != State.Gun)
        {
            state = State.Nothing;
        }
        
    }
    
    
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            Move();
            
        }

        loaded = true;
    }

    public void Move()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            var randomPosition = GetNextPositionOnPlane();
            ship.transform.position = randomPosition;
            Position.Value = randomPosition;
        }
        else
        {
            SubmitPositionRequestServerRpc();
        }
        //transform.eulerAngles = new Vector3(0,0,0);
    }

    [ServerRpc]
    void SubmitPositionRequestServerRpc(ServerRpcParams rpcParams = default)
    {
        Position.Value = GetNextPositionOnPlane();
        ship.transform.position = Position.Value;
    }

    Vector3 GetNextPositionOnPlane() {
        //TODO
        return GameNetworkManager.instance.spawnPoints[NetworkManager.Singleton.ConnectedClients.Count-1].position;
    }

}
