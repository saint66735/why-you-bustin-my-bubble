using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.VFX;
using Random = UnityEngine.Random;
using UnityEditor;

public class PlayerScript : NetworkBehaviour
{
    public NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>();
    public bool loaded = false;

    private float horizontalInput;
    [HideInInspector]
    public float verticalInput;
    private Vector3 movedirection;
    //NetworkVariable<WheelScript[]> wheels = new NetworkVariable<WheelScript[]>();
    private Rigidbody rigidBody;
    
    
    private float resetTimer = 0f;
    
    void Start() {
        rigidBody = GetComponent<Rigidbody>();
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
            transform.position = randomPosition;
            Position.Value = randomPosition;
        }
        else
        {
            SubmitPositionRequestServerRpc();
        }
        transform.eulerAngles = new Vector3(0,0,0);
    }

    [ServerRpc]
    void SubmitPositionRequestServerRpc(ServerRpcParams rpcParams = default)
    {
        Position.Value = GetNextPositionOnPlane();
        transform.position = Position.Value;
    }

    Vector3 GetNextPositionOnPlane() {
        return GameNetworkManager.instance.spawnPoints[NetworkManager.Singleton.ConnectedClients.Count-1].position;
    }

    void Update() {
        if (!GameNetworkManager.instance.isFreeroam && GameNetworkManager.instance.raceTime > 6f) {
            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");
            var handbrakeInput = Input.GetButton("Jump");
            var reset = Input.GetButton("Fire1");
            if (IsOwner && Application.isFocused)
                ControlCarServerRpc(new UserInputStruct(horizontalInput, verticalInput, handbrakeInput,reset));
        }
    }

    struct UserInputStruct : INetworkSerializeByMemcpy {
        public float horizontalInput;
        public float verticalInput;
        public bool handbrakeInput;
        public bool reset;


        public UserInputStruct(float horizontalInput, float verticalInput, bool handbrakeInput, bool reset) {
            this.horizontalInput = horizontalInput;
            this.verticalInput = verticalInput;
            this.handbrakeInput = handbrakeInput;
            this.reset = reset;
        }
    }


    [ServerRpc]
    void ControlCarServerRpc(UserInputStruct t, ServerRpcParams rpcParams = default) {

    }
    
    
}

