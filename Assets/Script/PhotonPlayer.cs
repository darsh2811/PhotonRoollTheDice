using System.Collections;
using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using UnityEngine;

public class PhotonPlayer : MonoBehaviour
{
    public static PhotonPlayer PP;

	private PhotonView PV;

	public GameObject myDice;


    private void Awake()
    {
        PP = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        int randomSpawn = Random.Range(0, 2);
        if (PV.IsMine)
        {
            Debug.Log(PV.ViewID);
            if (PV.ViewID == 1001)
            {
                myDice = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PhotonDicePlayer"),
                   GameController.GC.spawnPoints[0].position, GameController.GC.spawnPoints[randomSpawn].rotation, 0);
            }
            else
            {
                myDice = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PhotonDicePlayer"),
                  GameController.GC.spawnPoints[1].position, GameController.GC.spawnPoints[randomSpawn].rotation, 0);
            }
        }
        
    }
		// Update is called once per frame
		void Update()
    {
        
    }
}
