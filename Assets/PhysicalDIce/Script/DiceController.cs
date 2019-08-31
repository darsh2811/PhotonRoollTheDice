using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DiceController : MonoBehaviour {

    public PhotonView PV;

    Rigidbody rb;

    bool hasLanded;
    bool thrown;

    Vector3 initPosition;

    public int diceValue;

    public DiceSide[] ds;

    Text[] DiceText;

    int previousValue=0;


    // Use this for initialization
    void Start()
    {

        PV = GetComponent<PhotonView>();

        rb = GetComponent<Rigidbody>();
        initPosition = transform.position;
        rb.isKinematic = true;
        rb.useGravity = false;
        GameController.GC.myScore.text = "0";

        if (PV.IsMine)
        {
            BeginTurn();
        }
    }
	
    // Update is called once per frame
    void Update() {

        if (PV.IsMine)
        {
            if (GameController.GC.isReadyToRoll == true)
            {
            Debug.Log(GameController.GC.isReadyToRoll + "   Rollllling DIce  " + PV.IsMine );
           
                Debug.Log("Ready to roll");
                RollDice();
            }

            GameController.GC.isReadyToRoll = false;

        }
      

        if(rb.IsSleeping() && !hasLanded && thrown) {
            hasLanded = true;
            rb.useGravity = false;
            rb.isKinematic = true;
            FindSideValue();
        } else if(rb.IsSleeping() && hasLanded && diceValue == 0) {
            RollAgain();
        }
    }

    void BeginTurn()
    {
        if(PV.ViewID == 1002)
        {
            GameController.GC.isMyTurn = true;
        }
        else
        {
            GameController.GC.isMyTurn = false;
        }

        GameController.GC.RollButton.GetComponent<Button>().interactable = GameController.GC.isMyTurn;

       // 
    }

    

    void RollDice() {
        Debug.Log("In to Roll DIce Function" + thrown + "  " + hasLanded);
        if(!thrown && !hasLanded) {
            thrown = true;
            rb.useGravity = true;
            rb.AddTorque(Random.Range(0, 500), Random.Range(0, 500), Random.Range(0, 500));
            Debug.Log("Rollllling DIce");
        } else if(thrown && hasLanded) {
            ResetDice();
        }
    }

    void ResetDice() {
        transform.position = initPosition;
        thrown = false;
        hasLanded = false;
        rb.useGravity = false;
        rb.isKinematic = false;
        diceValue = 0;
        Debug.Log("Reseting Dice Postiion");
       
    }

    void RollAgain() {
        ResetDice();
        thrown = true;
        rb.useGravity = true;
        rb.AddTorque(Random.Range(0, 500), Random.Range(0, 500), Random.Range(0, 500));
    }

    void FindSideValue() {
       // diceValue = 0;
        foreach(DiceSide side in ds) {
            if (side.OnGround())
            {
                diceValue = previousValue + side.sideValue;
                Debug.Log("dice value" + gameObject.transform.parent.name + "    " + diceValue);
               
                previousValue = diceValue;
                GameController.GC.myScore.text = diceValue.ToString();
       
                if (PV.IsMine)
                {
                    PV.RPC("RPC_SetTurn", RpcTarget.AllBuffered, GameController.GC.isMyTurn, GameController.GC.myScore.text);
                }
            }
        }
    }

    [PunRPC]
    void RPC_SetTurn(bool currentPlayerTurn,string otherScore)
    {
        GameController.GC.isMyTurn = !GameController.GC.isMyTurn;
        GameController.GC.RollButton.GetComponent<Button>().interactable = GameController.GC.isMyTurn;
        if (!PV.IsMine)
        {
            GameController.GC.OtherScore.text = otherScore;
           
            if (GameController.GC.isMyTurn == true)
            {
                winLoseFunction();
             }
        }
        else
        {
            winLoseFunction();
        }
     }

    void winLoseFunction()
    {
        if (GameController.GC.roundCount < 6)
        {
            GameController.GC.roundCount += 1;

            GameController.GC.roundNo.text = GameController.GC.roundCount.ToString();
            //GameController.GC.roundNo.text = GameController.GC.roundCount.ToString();

            ResetDice();
        }
        else
        {
            if (int.Parse(GameController.GC.myScore.text) > int.Parse(GameController.GC.OtherScore.text))
            {
                GameController.GC.winLoseText.text = "You Win";
            }
            else if (int.Parse(GameController.GC.myScore.text) < int.Parse(GameController.GC.OtherScore.text))
            {
                GameController.GC.winLoseText.text = "YOu Losess";
            }
            else
            {
                GameController.GC.winLoseText.text = "Tie";
            }
            GameController.GC.RollButton.GetComponent<Button>().interactable = false;
            GameController.GC.winLosePanel.SetActive(true);
        }

    }

}
