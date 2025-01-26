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
    private float verticalInput;
    private Vector3 movedirection;
    
    
    private float resetTimer = 0f;
    
    void Start() {

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
        return GameNetworkManager.instance.spawnPoints[0].position;
    }

    void Update() {
        //&& GameNetworkManager.instance.raceTime > 6f
        if (!GameNetworkManager.instance.isFreeroam ) {
            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");
            var mouseX = Input.GetAxis("Mouse X");
            var mouseY = Input.GetAxis("Mouse Y");
            var handbrakeInput = Input.GetButton("Jump");
            var reset = Input.GetButton("Fire1");
            if (IsOwner && Application.isFocused)
                ControlCarServerRpc(new UserInputStruct(horizontalInput, verticalInput, handbrakeInput,reset, mouseX, mouseY));
        }
    }

    struct UserInputStruct : INetworkSerializeByMemcpy {
        public float horizontalInput;
        public float verticalInput;
        public bool handbrakeInput;
        public bool jump;
        public float mouseX;
        public float mouseY;


        public UserInputStruct(float horizontalInput, float verticalInput, bool handbrakeInput, bool jump, float mouseX, float mouseY) {
            this.horizontalInput = horizontalInput;
            this.verticalInput = verticalInput;
            this.handbrakeInput = handbrakeInput;
            this.jump = jump;
            this.mouseX = mouseX;
            this.mouseY = mouseY;
        }
    }


    [ServerRpc]
    void ControlCarServerRpc(UserInputStruct t, ServerRpcParams rpcParams = default) {

    }
    
    
}

