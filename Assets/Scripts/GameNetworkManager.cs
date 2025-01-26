using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Multiplayer.Playmode;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public class GameNetworkManager : NetworkBehaviour {
  // Start is called before the first frame update
  public List<GameObject> playerPrefabs; 
  public GameObject playerClientInstance;
  public static GameNetworkManager instance;
  public List<GameObject> otherPlayerObjects;
  public List<NetworkObject> playerInstances;
  public List<Transform> spawnPoints;
  public UI_Manager UIManagerScript;
  public float raceTime = 0;
  public NetworkVariable<bool> raceStarted = new NetworkVariable<bool>(false);
  public bool isFreeroam = false;

  private Action<NetworkManager.ConnectionApprovalRequest, NetworkManager.ConnectionApprovalResponse> defaultAprovalCallback;
  void Start() {
    instance = this;
    UIManagerScript.loadDefaults();
    var mppmTag = "";
    if (CurrentPlayer.ReadOnlyTags().Length>0)mppmTag = CurrentPlayer.ReadOnlyTags().First();
    Debug.Log(mppmTag);
    var networkManager = NetworkManager.Singleton;
    
    if (mppmTag.Contains("Server")) {
      networkManager.StartServer();
    }
    else if (mppmTag.Contains("Host")) {
      UIManagerScript.OnHost();
    }
    else if (mppmTag.Contains("Client")) {
      UIManagerScript.OnJoin();
    }
    
  }
  
  public override void OnNetworkSpawn()
  {
    if (IsServer) {
      defaultAprovalCallback = NetworkManager.Singleton.ConnectionApprovalCallback;
      NetworkManager.Singleton.ConnectionApprovalCallback = ConnectionApprovalCallback;
    }
    if (IsClient) {
      if (isFreeroam) {
        //TODO
      }
      else {
        //SpawnServerRpc(NetworkManager.LocalClient.ClientId);
      }
      SpawnServerRpc(NetworkManager.LocalClient.ClientId);
      
    }
  }

  public void RaceWin(ulong winnerID) {
    WinClientRpc(winnerID);
  }
  
  
  [ClientRpc]
  void WinClientRpc(ulong winnerID) {
    Debug.Log(winnerID);
    Debug.Log(NetworkManager.LocalClientId);
    var txt = winnerID == NetworkManager.LocalClientId;
    UIManagerScript.ShowWinner(txt);
  }

  void StartRace() {
    raceStarted.Value = true;
    
    StartClientRpc();
  }
  [ClientRpc]
  void StartClientRpc() {
    UIManagerScript.timeLeft = 6f;
  }
  
  [ServerRpc(RequireOwnership = true)]
  void CloseServerRpc(ServerRpcParams rpcParams = default) {
    NetworkManager.DisconnectClient(rpcParams.Receive.SenderClientId);
  }

  public void Close() {
    CloseServerRpc();
  }

  [ServerRpc(RequireOwnership = false)]
  void SpawnServerRpc(ulong id, ServerRpcParams rpcParams = default) {
    var playerPrefab = playerPrefabs[Random.Range(0, playerPrefabs.Count)];
    GameObject go = (GameObject)Instantiate(playerPrefab, Vector3.zero, Quaternion.Euler(Vector3.left));
    go.GetComponent<NetworkObject>().SpawnAsPlayerObject(id, true);
    Debug.Log("Spawned player");
  }
  private void ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
  {
    Debug.Log("conection aproved");
    
    
    if (defaultAprovalCallback!=null) defaultAprovalCallback.Invoke(request,response);

  }
  
  // Update is called once per frame
  void Update() {
    
      var _otherPlayerObjects = FindObjectsOfType<Player>().Select(x=>x.gameObject);
      if (otherPlayerObjects == null || _otherPlayerObjects.Count() != otherPlayerObjects.Count()) otherPlayerObjects = new List<GameObject>(_otherPlayerObjects);
    
    if (IsServer && NetworkManager.Singleton) {
      var _playerInstances = NetworkManager.Singleton.ConnectedClients
        .Select(x => x.Value.PlayerObject).Where(x=>x!=null).AsReadOnlyList();
      if (playerInstances == null || playerInstances.Count != _playerInstances.Count()) playerInstances = new List<NetworkObject>(_playerInstances);
    }
    
    //TODO
    if (playerInstances.Count >= 1) {
      if(!raceStarted.Value) StartRace();
    }

    if (raceStarted.Value == true) raceTime += Time.deltaTime;

    if (IsClient && playerClientInstance == null && NetworkManager.LocalClient.PlayerObject!=null) {
      playerClientInstance = NetworkManager.Singleton.LocalClient.PlayerObject.gameObject;
    }
  }
  
}