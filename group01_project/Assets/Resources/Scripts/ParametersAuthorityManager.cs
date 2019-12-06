using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

// TODO: this script should manage authority for a shared object
public class ParametersAuthorityManager : NetworkBehaviour
{ 
    NetworkIdentity netID; // NetworkIdentity component attached to this game object
    Actor localActor;
    SharedParameters sharedParameters;

    bool gameSpaceExtensionPossible = false;
    bool finisedUpdatingParams = false;
    bool updateParams = false;
   

    // Use this for initialization
    void Start () {
        if ((netID = GetComponent<NetworkIdentity>()) == null)
        {
            Debug.LogError("netID is NULL in AuthorityManager");
        }
        
        sharedParameters = GetComponent<SharedParameters>();
    }

    private void Update()
    {
        //Debug.Log("HasAuthority: " + netID.hasAuthority);
        UpdateSharedParams();
    }

    private void UpdateSharedParams()
    {
        if (isServer)
        {
            return;
        }

        if(updateParams && !netID.hasAuthority)
        {
            localActor.RequestObjectAuthority(netID);
            return;
        }

        if (netID.hasAuthority)
        {
            if (!finisedUpdatingParams)
            {
                sharedParameters.CmdSetGameSpaceExtension(gameSpaceExtensionPossible);
                finisedUpdatingParams = true;
                return;
            }
            if (finisedUpdatingParams)
            {
                localActor.ReturnObjectAuthority(netID);
                finisedUpdatingParams = false;
                updateParams = false;
                return;
            }
        }
    }

    public void SetGameSpaceExtensionPossible(bool my_gameSpaceExtensionPossible)
    {
        gameSpaceExtensionPossible = my_gameSpaceExtensionPossible;
        updateParams = true;
    }


    // should only be called on server (by an Actor)
    // assign the authority over this game object to a client with NetworkConnection conn
    public void AssignClientAuthority(NetworkConnection conn)
    {
        if (this.GetComponent<NetworkIdentity>().clientAuthorityOwner == null)
        {
            this.netID.AssignClientAuthority(conn);
            Debug.Log("ParameterAuthorityManager Assign : Has Authority " + netID.hasAuthority);
        }
    }

    // should only be called on server (by an Actor)
    // remove the authority over this game object from a client with NetworkConnection conn
    public void RemoveClientAuthority(NetworkConnection conn)
    {
        if (this.GetComponent<NetworkIdentity>().clientAuthorityOwner == conn)
        {
            this.netID.RemoveClientAuthority(conn);
            Debug.Log("ParameterAuthorityManager - Remove Client Authority: Has Authority " + netID.hasAuthority);
        }
    }

    [ClientRpc]
    void RpcGotAuthority(bool gameSpaceExtensionPossible)
    {
        sharedParameters.CmdSetGameSpaceExtension(gameSpaceExtensionPossible);
    }

    [ClientRpc]
    void RpcLostAuthority()
    {
        
    }

    public void AssignActor(Actor actor)
    {
        localActor = actor;
    }
}
