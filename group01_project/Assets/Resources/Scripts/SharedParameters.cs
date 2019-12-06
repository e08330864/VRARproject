using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SharedParameters : NetworkBehaviour
{
    // Start is called before the first frame update

    [SyncVar]
    bool allowGameSpaceExtension = false;

    [SyncVar] 
    Vector3 updatedLeapPosition = new Vector3(0.0f, 0.0f, 0.0f);

    [Command]
    public void CmdSetGameSpaceExtension(bool extensionAllowed)
    {
        if (!isServer)
        {
            Debug.Log("ERROR: Allow Gamespace called on client");
            return;
        }
        Debug.Log("Shared Parameter on Server - GameSpaceExtensionAllowed: " + extensionAllowed);
        allowGameSpaceExtension = extensionAllowed;
    }

    [Command]
    public void CmdSetNewPosition(Vector3 position)
    {
        if (!isServer)
        {
            Debug.Log("ERROR: Allow Gamespace called on client");
            return;
        }
        Debug.Log("Shared Parameter on Server - new shift");
        updatedLeapPosition = position;
    }

    public bool GameSpaceExtensionPossible()
    {
        return allowGameSpaceExtension;
    }

    public Vector3 GetNewPosition()
    {
        return updatedLeapPosition;
    }

    private void Update()
    {
        //Debug.Log("GameSpaceExtensionPossible: " + allowGameSpaceExtension);
    }
}
