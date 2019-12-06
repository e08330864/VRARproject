using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

// TODO: this script should manage authority for a shared object
public class AuthorityManager : NetworkBehaviour {
 
    NetworkIdentity netID; // NetworkIdentity component attached to this game object
    
    // these variables should be set up on a client
    //**************************************************************************************************
    Actor localActor; // Actor that is steering this player 

    [SyncVar]
    private bool isHeld; // true, if object is currently held by any player
    private bool isHeldByLocalPlayer = false;  // true, if object is currently held by local player

    private bool _playerGrabs = false; // true, if local player is currently grabbing the object
    public bool playerGrabs     // true, if local player is currently grabbing object
    {
        get { return _playerGrabs; }
        set { _playerGrabs = value; }
    } 
    private bool leftGrabbedNew = false; // true, if grabbing action of local player is done by left hand   
    private bool leftGrabbed = false; // true, if object is grabbed by local player with left hand

    OnGrabbedBehaviour onb; // component defining the behaviour of this GO when it is grabbed by a player
                            // this component can implement different functionality for different GO´s


    //***************************************************************************************************

    // these variables should be set up on the server

    // TODO: implement a mechanism for storing consequent authority requests from different clients
    // e.g. manage a situation where a client requests authority over an object that is currently being manipulated by another client

    //*****************************************************************************************************

    // TODO: avoid sending two or more consecutive RemoveClientAuthority or AssignClientAUthority commands for the same client and shared object
    // a mechanism preventing such situations can be implemented either on the client or on the server


    public void SetLeftGrabbedNew(bool leftGrabbedNew)
    {
        this.leftGrabbedNew = leftGrabbedNew;
    }

    public bool GetLeftGrabbed()
    {
        return leftGrabbed;
    }

    // Use this for initialization
    void Start () {
        if ((localActor = GameObject.Find("Player").GetComponent<Actor>()) == null)
        {
            Debug.LogError("localActor is NULL in AuthorityManager");
        }
        if ((netID = GetComponent<NetworkIdentity>()) == null)
        {
            Debug.LogError("netID is NULL in AuthorityManager");
        }
        if ((onb = GetComponent<OnGrabbedBehaviour>()) == null)
        {
            Debug.LogError("onb is NULL in AuthorityManager");
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (localActor != null) { 
            if (playerGrabs)    // local player is currently grabbing the object
            {
                if (!isHeld)    // the object is currently held by a player
                {
                    Debug.Log("AuthorityManager: calling RequestObjectAuthority --> isGrabbed = true");
                    //Debug.Log("AuthorityManager: localActor=" + localActor.gameObject.transform.parent.name);
                    leftGrabbed = leftGrabbedNew;
                    localActor.RequestObjectAuthority(netID);
                }
            }
            else    // the local player is currently not grabbing the object
            {
                if (isHeld)  // the object is currently held, but not grabbed by local player
                {
                    Debug.Log("AuthorityManager: calling ReturnObjectAuthority --> isGrapping = false");
                    localActor.ReturnObjectAuthority(netID);
                }
            }
        }
    }

    //// assign localActor here
    //public void AssignActor(Actor actor)
    //{
    //    localActor = actor;
    //}

    public Actor GetActor()
    {
        return localActor;
    }

    // should only be called on server (by an Actor)
    // assign the authority over this game object to a client with NetworkConnection conn
    public void AssignClientAuthority(NetworkConnection conn)
    {
        if (this.GetComponent<NetworkIdentity>().clientAuthorityOwner == null)
        {
            if (this.netID.AssignClientAuthority(conn))
            {
                Debug.Log("AuthorityManager: AssignClientAuthority...authority=" + this.GetComponent<NetworkIdentity>().hasAuthority);
                isHeld = true;
                RpcGotAuthority();
            }
        }
    }

    // should only be called on server (by an Actor)
    // remove the authority over this game object from a client with NetworkConnection conn
    public void RemoveClientAuthority(NetworkConnection conn)
    {
        if (this.GetComponent<NetworkIdentity>().clientAuthorityOwner == conn)
        {
            if (this.netID.RemoveClientAuthority(conn))
            {
                Debug.Log("AuthorityManager: RemoveClientAuthority...authority=" + this.GetComponent<NetworkIdentity>().hasAuthority);
                isHeld = false;
                RpcLostAuthority();
            }
        }
    }

    [ClientRpc]
    void RpcGotAuthority()
    {
        if (playerGrabs)
        {
            Debug.Log("AuthorityManager: calling onb.OnGrabbed");
            onb.OnGrabbed();
        }
    }

    [ClientRpc]
    void RpcLostAuthority()
    {
        Debug.Log("AuthorityManager: calling onb.OnReleased");
        onb.OnReleased();
    }
}
