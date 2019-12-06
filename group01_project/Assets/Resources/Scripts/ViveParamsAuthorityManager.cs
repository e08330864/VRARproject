using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

// TODO: this script should manage authority for a shared object
public class ViveParamsAuthorityManager : NetworkBehaviour
{ 
    NetworkIdentity netID; // NetworkIdentity component attached to this game object
    Actor localActor;
    SharedParameters sharedParameters;
    Vector3 position = new Vector3(0.0f, 0.0f, 0.0f);
    bool finishedUpdatingParams = false;
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
            Debug.Log("ViveShared Params - REQUESTING AUTHORITY");
            localActor.RequestObjectAuthority(netID);
            return;
        }
        
        if (netID.hasAuthority)
        {
            if (updateParams && !finishedUpdatingParams)
            {
                sharedParameters.CmdSetNewPosition(position);
                finishedUpdatingParams = true;
                return;
            }
            if (finishedUpdatingParams)
            {
                Debug.Log("ViveShared Params - REMOVING AUTHORITY");
                localActor.ReturnObjectAuthority(netID);
                finishedUpdatingParams = false;
                updateParams = false;
                return;
            }
        }
    }

    public void SetPosition(Vector3 my_position)
    {
        position = my_position;
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

    public void AssignActor(Actor actor)
    {
        localActor = actor;
    }
}
