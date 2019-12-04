using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

// TODO: this script should manage authority for a shared object
public class ParametersAuthorityManager : NetworkBehaviour {

    
    NetworkIdentity netID; // NetworkIdentity component attached to this game object
    
    // these variables should be set up on a client
    //**************************************************************************************************
    //[SyncVar]
    Actor localActor; // Actor that is steering this player 


    //private bool isGrabbed = false;  // true, if object is currently grabbed by local player
    //private bool m_playerGrabs = false; // true, if local player is grabbing the object
    /*public bool playerGrabs {
        get { return m_playerGrabs; }
        set { m_playerGrabs = value; }
    } */
    //private bool leftGrabbedNew = false; // the   
    //private bool leftGrabbed = false; // if isGrapping=true --> true=left hand grabbed; false=right hand grabbed

    //OnGrabbedBehaviour onb; // component defining the behaviour of this GO when it is grabbed by a player
                            // this component can implement different functionality for different GO´s


    //***************************************************************************************************

    // these variables should be set up on the server

    // TODO: implement a mechanism for storing consequent authority requests from different clients
    // e.g. manage a situation where a client requests authority over an object that is currently being manipulated by another client

    //*****************************************************************************************************

    // TODO: avoid sending two or more consecutive RemoveClientAuthority or AssignClientAUthority commands for the same client and shared object
    // a mechanism preventing such situations can be implemented either on the client or on the server


    /*public void SetLeftGrabbedNew(bool leftGrabbedNew)
    {
        this.leftGrabbedNew = leftGrabbedNew;
    }

    public bool GetLeftGrabbed()
    {
        return leftGrabbed;
    }*/


    // Use this for initialization
    void Start () {
        if ((localActor = GameObject.Find("Player").GetComponent<Actor>()) == null)
        {
            //Debug.LogError("localActor is NULL in AuthorityManager");
        }

        if ((netID = GetComponent<NetworkIdentity>()) == null)
        {
            Debug.LogError("netID is NULL in AuthorityManager");
        }
        /*if ((onb = GetComponent<OnGrabbedBehaviour>()) == null)
        {
            Debug.LogError("onb is NULL in AuthorityManager");
        }*/
    }
	
	// Update is called once per frame
	void Update () {
        /*if (localActor != null) { 
            if (playerGrabs)    // local player is currently grabbing
            {
                if (!isGrabbed) // the object is currently not grabbed
                {
                    Debug.Log("calling RequestObjectAuthority --> isGrabbed = true");
                    Debug.Log("localActor=" + localActor.gameObject.tag);
                    leftGrabbed = leftGrabbedNew;
                    localActor.RequestObjectAuthority(netID);
                    isGrabbed = true;
                }
            }
            else    // the local player is currently not grabbing
            {
                if (isGrabbed)  // the object is currently grabbed
                {
                    Debug.Log("calling ReturnObjectAuthority --> isGrapping = false");
                    localActor.ReturnObjectAuthority(netID);
                    isGrabbed = false;
                }
            }
        }*/
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
            this.netID.AssignClientAuthority(conn);
            //Debug.Log("Has Authority " + this.GetComponent<NetworkIdentity>().hasAuthority);
        }
    }

    // should only be called on server (by an Actor)
    // remove the authority over this game object from a client with NetworkConnection conn
    public void RemoveClientAuthority(NetworkConnection conn)
    {
        if (this.GetComponent<NetworkIdentity>().clientAuthorityOwner == conn)
        {
            this.netID.RemoveClientAuthority(conn);
        }
    }
}
