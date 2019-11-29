using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SharedParameters : NetworkBehaviour
{
    // Start is called before the first frame update

    [SyncVar]
    bool allowGameSpaceExtension = false;

    [Command]
    public void CmdSetGameSpaceExtension(bool extensionAllowed)
    {
        if (!isServer)
        {
            Debug.Log("ERROR: Allow Gamespace called on client");
            return;
        }
           

        allowGameSpaceExtension = extensionAllowed;
    }

    public bool GameSpaceExtensionPossible()
    {
        return allowGameSpaceExtension;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            Debug.Log("Allow Gamespace extension from leap side");
            allowGameSpaceExtension = true;
        }
        else
        {
            Debug.Log("Deny Gamespace extension from leap side");
            allowGameSpaceExtension = false;
        }
    }
}
