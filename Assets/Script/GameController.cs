using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController GC;

    public Transform[] spawnPoints;

    public Text myScore,OtherScore,roundNo ,winLoseText;

    public bool isMyTurn,isReadyToRoll;

    public GameObject RollButton,winLosePanel;

    public int roundCount = 1;

    private void Awake()
    {
        if (GC == null)
        {
            GC = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnRollButtonClicked()
    {
        isReadyToRoll = true;
    }

    public void LeaveAndRestart()
    {
        StartCoroutine(WaitforLeaveRoomAndRestart());
    }

    IEnumerator WaitforLeaveRoomAndRestart()
    {
        PhotonNetwork.LeaveRoom();
        while (PhotonNetwork.InRoom)
            yield return null;

        SceneManager.LoadScene(MultiplayerSettings.multiPlayerSetting.menuScene);

    }
}
