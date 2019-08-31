using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerSettings : MonoBehaviour
{
    public static MultiplayerSettings multiPlayerSetting;

    public bool delayStart;
    public int maxPlayer;

    public int menuScene;
    public int multiPlaryerScene;

    private void Awake()
    {
        if (MultiplayerSettings.multiPlayerSetting == null)
        {
            MultiplayerSettings.multiPlayerSetting = this;
        }
        else
        {
            if (MultiplayerSettings.multiPlayerSetting != this)
            {
                Destroy(this.gameObject);
            }
        }
        DontDestroyOnLoad(this.gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
