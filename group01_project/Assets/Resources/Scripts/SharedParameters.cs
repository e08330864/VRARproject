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

    public void AssignClientAuthority(NetworkConnection conn)
    {
        if (this.GetComponent<NetworkIdentity>().clientAuthorityOwner == null)
        {
            //this.netID.AssignClientAuthority(conn);
            //Debug.Log("Has Authority " + this.GetComponent<NetworkIdentity>().hasAuthority);
            //RpcGotAuthority();
        }
    }

    // should only be called on server (by an Actor)
    // remove the authority over this game object from a client with NetworkConnection conn
    public void RemoveClientAuthority(NetworkConnection conn)
    {
        if (this.GetComponent<NetworkIdentity>().clientAuthorityOwner == conn)
        {
            //this.netID.RemoveClientAuthority(conn);
            //RpcLostAuthority();
        }
    }
}
