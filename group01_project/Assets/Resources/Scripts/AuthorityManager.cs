using UnityEngine;
using UnityEngine.Networking;

// TODO: this script should manage authority for a shared object
public class AuthorityManager : NetworkBehaviour {

    
    NetworkIdentity netID; // NetworkIdentity component attached to this game object
    NetworkConnection waitingNetID;

    // these variables should be set up on a client
    //**************************************************************************************************
    Actor localActor; // Actor that is steering this player 


    private bool grabbed = false; // if this is true client authority for the object should be requested
    private bool isGrapping = false;
    public bool grabbedByPlayer // private "grabbed" field can be accessed from other scripts through grabbedByPlayer
    {
        get { return grabbed; }
        set { grabbed = value; }
    }

    OnGrabbedBehaviour onb; // component defining the behaviour of this GO when it is grabbed by a player
                            // this component can implement different functionality for different GO´s


    //***************************************************************************************************

    // these variables should be set up on the server

    // TODO: implement a mechanism for storing consequent authority requests from different clients
    // e.g. manage a situation where a client requests authority over an object that is currently being manipulated by another client

    //*****************************************************************************************************

    // TODO: avoid sending two or more consecutive RemoveClientAuthority or AssignClientAUthority commands for the same client and shared object
    // a mechanism preventing such situations can be implemented either on the client or on the server

    // Use this for initialization
    void Start () {
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
        if (grabbedByPlayer)
        {
            if (!isGrapping)
            {
                Debug.Log("grabbed && !isGrapped");
                grabbedByPlayer = true;
                localActor.RequestObjectAuthority(netID);
                isGrapping = true;
            }
        }
        else
        {
            if (isGrapping)
            {
                Debug.Log("!grabbed && isGrapping");
                grabbedByPlayer = false;
                localActor.ReturnObjectAuthority(netID);
                isGrapping = false;
            }
        }
    }

    // assign localActor here
    public void AssignActor(Actor actor)
    {
        localActor = actor;
    }

    // should only be called on server (by an Actor)
    // assign the authority over this game object to a client with NetworkConnection conn
    public void AssignClientAuthority(NetworkConnection conn)
    {
        Debug.Log("Has Authority " + this.GetComponent<NetworkIdentity>().hasAuthority);
        if (this.GetComponent<NetworkIdentity>().clientAuthorityOwner == null)
        {
            this.netID.AssignClientAuthority(conn);
            RpcGotAuthority();
        } else {
            waitingNetID = conn;
        }
    }

    // should only be called on server (by an Actor)
    // remove the authority over this game object from a client with NetworkConnection conn
    public void RemoveClientAuthority(NetworkConnection conn)
    {
        if (this.GetComponent<NetworkIdentity>().clientAuthorityOwner == conn)
        {
            this.netID.RemoveClientAuthority(conn);
            RpcLostAuthority();
        }
        else if (waitingNetID == conn)
        {
            waitingNetID = null;
        } 
        if( waitingNetID != null) 
        {
            this.netID.AssignClientAuthority(waitingNetID);
            RpcGotAuthority();
            waitingNetID = null;
        }
    }

    [ClientRpc]
    void RpcGotAuthority()
    {
        if (grabbed)
        {
            onb.OnGrabbed(localActor);
        }
    }

    [ClientRpc]
    void RpcLostAuthority()
    {
        onb.OnReleased();
    }
}
