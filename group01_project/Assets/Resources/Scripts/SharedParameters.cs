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
    Vector3 extensionShift = new Vector3(0.0f, 0.0f, 0.0f);

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
    public void CmdSetExtensionShift(Vector3 shift)
    {
        if (!isServer)
        {
            Debug.Log("ERROR: Allow Gamespace called on client");
            return;
        }
        Debug.Log("Shared Parameter on Server - new shift");
        extensionShift = shift;
    }

    public bool GameSpaceExtensionPossible()
    {
        return allowGameSpaceExtension;
    }

    public Vector3 GetExtensionShift()
    {
        return extensionShift;
    }

    private void Update()
    {
        //Debug.Log("GameSpaceExtensionPossible: " + allowGameSpaceExtension);
    }
}
