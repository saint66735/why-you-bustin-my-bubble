using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode.Transports.UTP;
using Unity.VisualScripting;
using UnityEngine.Rendering;

public class UI_Manager : MonoBehaviour
{
    public GameObject mainPanel;
    public GameObject additionalPanel;
    public TMP_InputField ipInput;
    public TMP_InputField portInput;
    //public Image countdown;
    public Image Losing;
    public Image Winning;
    UnityTransport transport;
    public float timeLeft = 0;

    public List<Sprite> countdownImages;
    // Start is called before the first frame update
    void Start() {
        //loadDefaults();
    }

    public void loadDefaults() {
        var tp = GameNetworkManager.instance.NetworkManager.GetComponent<UnityTransport>();
        //ipInput.SetText(tp.ConnectionData.Address);
        //portInput.SetText(Convert.ToString((int)tp.ConnectionData.Port));
        //Debug.Log(tp.ConnectionData.Port);
    }
    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (additionalPanel.activeInHierarchy)
            {
                additionalPanel.SetActive(false);
                //Cursor.visible = false;
            }
            else
            {
                additionalPanel.SetActive(true);
                //Cursor.visible = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.P)) {
            GameNetworkManager.instance.isFreeroam = true;
            OnJoin();
        }
        if (Input.GetKeyDown(KeyCode.O)) {
            //TODO
            //GameNetworkManager.instance.isFreeroam = true;
            OnHost();
        }
        if (Input.GetKeyDown(KeyCode.L)) {
            GameNetworkManager.instance.isFreeroam = !GameNetworkManager.instance.isFreeroam;
            //TODO
            //Camera.main.GetComponent<FreeCamera>().enabled = GameNetworkManager.instance.isFreeroam;
            //Camera.main.GetComponent<CameraScript>().enabled = !GameNetworkManager.instance.isFreeroam;
        }
    }
    public void OnQuit()
    {
        GameNetworkManager.instance.Close();
        Application.Quit();
        Debug.Log("I WANNA QUIT");
    }
    public void closePanels()
    {
        mainPanel.SetActive(false);
        additionalPanel.SetActive(false);
        //Cursor.visible = false;
    }
    void updateAddress()
    {
        transport = GameNetworkManager.instance.NetworkManager.GetComponent<UnityTransport>();
        transport.ConnectionData.Address = ipInput.text;
        string temp = portInput.text.Substring(0, portInput.text.Length);
        //Debug.Log(portInput.text);
        try {
            transport.ConnectionData.Port = ushort.Parse(temp);
        }
        catch {
            Debug.Log("wrong port format");
        }

        Debug.Log(transport.ConnectionData.Port);
        Debug.Log(transport.ConnectionData.Address);
    }
    public void OnHost()
    {
        GameNetworkManager.instance.NetworkManager.StartHost();
        closePanels();
        
    }
    public void OnJoin()
    {
        GameNetworkManager.instance.NetworkManager.StartClient();
        closePanels();
    }

    public void TextEdited() {
        updateAddress();
    }

    public void ShowWinner(bool won) {
        if (won) Winning.enabled = true; else Losing.enabled = true;
    }
}  
