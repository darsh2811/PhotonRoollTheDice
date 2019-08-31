using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PhotonLobby : MonoBehaviourPunCallbacks
{
    public static PhotonLobby lobby;

    public GameObject playButton;
    public GameObject cancelButton;

    private void Awake()
    {
        lobby = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }


    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("Connected to PhotonMaster Server");
        PhotonNetwork.AutomaticallySyncScene = true;
        playButton.SetActive(true);
    }

    public void OnPlayButtonClicked()
    {
        Debug.Log("Play button clicked");
        playButton.SetActive(false);
        cancelButton.SetActive(true);
        PhotonNetwork.JoinRandomRoom();
    }

    public void OnCancelButtonClicked()
    {
        playButton.SetActive(true);
        cancelButton.SetActive(false);
        PhotonNetwork.LeaveRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    { 
        Debug.Log("Fail to join random room");
        CreateRoom();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        
        CreateRoom();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined the ROOM");
    }

    void CreateRoom()
    {
        Debug.Log(" Trying to cReating ROom ");
        int randomRoomName = Random.Range(0, 10000);
        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)MultiplayerSettings.multiPlayerSetting.maxPlayer };
        PhotonNetwork.CreateRoom("Room" + randomRoomName, roomOps);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
