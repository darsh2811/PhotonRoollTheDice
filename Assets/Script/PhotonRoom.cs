using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using ExitGames.Client.Photon;

public class PhotonRoom : MonoBehaviourPunCallbacks,IInRoomCallbacks
{
    public static PhotonRoom room;
    private PhotonView pv;

    public bool isGameLoaded;
    public int currentScene;

    //Player Info
    Player[] photonPlayers;
    public int playersInRoom;
    public int myNumberInRoom;

    public int playersInGame;

    //Delayed Start
    private bool readyToCount;
    private bool readyToStart;
    public float startingTime;
    private float lessThanMaxPlayers;
    private float atMaxPlayer;
    private float timeToStart;

    private void Awake()
    {
        if (PhotonRoom.room == null)
        {
            PhotonRoom.room = this;
        }
        else
        {
            if (PhotonRoom.room != this)
            {
                Destroy(PhotonRoom.room.gameObject);
                PhotonRoom.room = this;
            }
        }
        DontDestroyOnLoad(this.gameObject);
    }

    public override void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.AddCallbackTarget(this);
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.RemoveCallbackTarget(this);
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }

    // Start is called before the first frame update
    void Start()
    {
        pv = GetComponent<PhotonView>();
        readyToCount = false;
        readyToStart = false;
        lessThanMaxPlayers = startingTime;
        atMaxPlayer = 6;
        timeToStart = startingTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (MultiplayerSettings.multiPlayerSetting.delayStart)
        {
            if (playersInRoom == 1)
            {
                RestartTimer();
            }

            // Debug.Log("Update IsGaMeLoaded  " + isGameLoaded);

            if (!isGameLoaded)
            {
                //Debug.Log("Update   " + isGameLoaded);
                if (readyToStart)
                {
                    atMaxPlayer -= Time.deltaTime;
                    lessThanMaxPlayers = atMaxPlayer;
                    timeToStart = atMaxPlayer;
//                    Debug.Log("Update  Reday toCount  " + readyToStart);
                }
                else if (readyToCount)
                {
                    lessThanMaxPlayers -= Time.deltaTime;
                    timeToStart = lessThanMaxPlayers;
                //    Debug.Log("Update  Readytocount  " + readyToCount);
                }
                //Debug.Log("Display time to start to the players" + timeToStart);
                if (timeToStart <= 0)
                {
                    //Debug.Log("Whats Wrong ppl");
                    StartGame();
                }
            }
        }

    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
       // Debug.Log("We Are in a room");
        photonPlayers = PhotonNetwork.PlayerList;
        playersInRoom = photonPlayers.Length;
        myNumberInRoom = playersInRoom;
        PhotonNetwork.NickName = myNumberInRoom.ToString();

        if (MultiplayerSettings.multiPlayerSetting.delayStart)
        {
          //  Debug.Log("Displayer Players in Room Out of Max Players possible (" + playersInRoom + ";" + MultiplayerSettings.multiPlayerSetting.maxPlayer + ")");

            if (playersInRoom > 1)
            {
                readyToCount = true;
            }
            if (playersInRoom == MultiplayerSettings.multiPlayerSetting.maxPlayer)
            {
                readyToStart = true;
                if (!PhotonNetwork.IsMasterClient)
                    return;
                PhotonNetwork.CurrentRoom.IsOpen = false;
            }
        }
        else
        {

            StartGame();
        }
    }


    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
       // Debug.Log("A New Plyare has joined the Room");
        photonPlayers = PhotonNetwork.PlayerList;
        playersInRoom++;

        if (MultiplayerSettings.multiPlayerSetting.delayStart)
        {
          //  Debug.Log("Displayer Players in Room Out of Max Players possible (" + playersInRoom + ";" + MultiplayerSettings.multiPlayerSetting.maxPlayer + ")");

            if (playersInRoom > 1)
            {
                readyToCount = true;
            }
            if (playersInRoom == MultiplayerSettings.multiPlayerSetting.maxPlayer)
            {
                readyToStart = true;
                if (!PhotonNetwork.IsMasterClient)
                    return;
                PhotonNetwork.CurrentRoom.IsOpen = false;
            }
        }
    }


    void StartGame()
    {
       // Debug.Log("WHy : Please");
        isGameLoaded = true;
        if (!PhotonNetwork.IsMasterClient)
            return;

        if (MultiplayerSettings.multiPlayerSetting.delayStart)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }
        PhotonNetwork.LoadLevel(MultiplayerSettings.multiPlayerSetting.multiPlaryerScene);
    }

    void RestartTimer()
    {
        lessThanMaxPlayers = startingTime;
        timeToStart = startingTime;
        atMaxPlayer = 2;
        readyToCount = false;
        readyToStart = false;
        // Debug.Log("Reset Timer");
    }

    void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        currentScene = scene.buildIndex;
        if (currentScene == MultiplayerSettings.multiPlayerSetting.multiPlaryerScene)
        {
            //Debug.Log("Erro rr hai");
            isGameLoaded = true;

            if (MultiplayerSettings.multiPlayerSetting.delayStart)
            {
                pv.RPC("RPC_LoadedGameScene", RpcTarget.MasterClient);
            }
            else
            {
                RPC_CreatePlayer();
            }
        }
    }

    [PunRPC]
    private void RPC_LoadedGameScene()
    {
        playersInGame++;
        if (playersInGame == PhotonNetwork.PlayerList.Length)
        {
            pv.RPC("RPC_CreatePlayer", RpcTarget.All);
        }
    }

    [PunRPC]
    private void RPC_CreatePlayer()
    {
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PhotonNetworkPlayer"), transform.position, Quaternion.identity);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        Debug.Log(otherPlayer.NickName + " Has Left the Game");
        playersInGame--;
    }

}
