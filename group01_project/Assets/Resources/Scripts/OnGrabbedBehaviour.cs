using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

// TODO: define the behaviour of a shared object when it is manipulated by a client

public class OnGrabbedBehaviour : NetworkBehaviour
{
    [SerializeField]
    private float throwingSpeedFactor = 300f;

    private bool isGrabbed;
    private bool releasing;
    private Vector3 oldPos;
    private Vector3 newPos;
    private List<Vector3> speedList = new List<Vector3>();

    private Rigidbody rigidbody = null;
    private NetworkIdentity netId = null;
    private AuthorityManager authorityManager = null;
    private GameObject handLeft = null;
    private GameObject handRight = null;

    // Use this for initialization
    void Start ()
    {
        if ((authorityManager = GetComponent<AuthorityManager>()) == null) { Debug.LogError("authorityManager is NULL in OnGrabbedBehaviour"); }
        if ((rigidbody = GetComponent<Rigidbody>()) == null) { Debug.LogError("rigidbody is NULL in OnGrabbedBehaviour"); }
        if ((netId = GetComponent<NetworkIdentity>()) == null) { Debug.LogError("netId is NULL in OnGrabbedBehaviour"); }
    }

    // Update is called once per frame
    void Update()
    {
        GetHands();
        if (isGrabbed && authorityManager.GetLeftGrabbed() && handLeft != null && netId.hasAuthority)
        {
            // object is gabbed and moved by left hand
            oldPos = this.transform.position;
            this.transform.position = handLeft.transform.position;
            newPos = this.transform.position;
            //Vector3 speed = (newPos - oldPos) / Time.deltaTime;
            //AddSpeedVector(speed);
            //Debug.Log("speed=" + speed * throwingSpeedFactor * rigidbody.mass);
        }
        else if (isGrabbed && !authorityManager.GetLeftGrabbed() && handRight != null && netId.hasAuthority)
        {
            // object is gabbed and moved by right hand
            oldPos = this.transform.position;
            this.transform.position = handRight.transform.position;
            newPos = this.transform.position;
            //Vector3 speed = (newPos - oldPos) / Time.deltaTime;
            //AddSpeedVector(speed);
            //Debug.Log("speed=" + speed * throwingSpeedFactor * rigidbody.mass);
        }
        else if (releasing)
        {
            // object is released
            Vector3 speed = (newPos - oldPos) / Time.deltaTime;
            //AddSpeedVector(speed);
            //speed = GetSpeedAverage();
            Debug.Log("throwing speed=" + speed * throwingSpeedFactor * rigidbody.mass);
            //if (netId.isServer)
            //{
            //    Debug.Log("is server");
            //    RpcAddClientForce(speed, throwingSpeedFactor);
            //}
            //else if (netId.isClient)
            //{
            //    Debug.Log("is client");
            //    CmdAddForce(speed, throwingSpeedFactor);
            //}
            CmdAddForce(speed, throwingSpeedFactor);
            //rigidbody.AddForce(speed * throwingSpeedFactor * rigidbody.mass);
            releasing = false;
        }
    }

    private void AddSpeedVector(Vector3 speed)
    {
        if (speedList.Count >= 5)
        {
            speedList.RemoveAt(0);
            speedList.Add(speed);
        }
    }

    private Vector3 GetSpeedAverage()
    {
        Vector3 average = Vector3.zero;
        foreach (Vector3 v in speedList)
        {
            average += v;
        }
        if (speedList.Count > 0)
        {
            average = average / speedList.Count;
        }
        return average;
    }

    // called first time when the GO gets grabbed by a player
    public void OnGrabbed()
    {
        Debug.Log("OnGrabbedBehaviour: OnGrabbed...isKinematic=true, useGravity=false");
        isGrabbed = true;
        rigidbody.isKinematic = true;
        rigidbody.useGravity = false;
        releasing = false;
    }

    // called when the GO gets released by a player
    public void OnReleased()
    {
        Debug.Log("OnGrabbedBehaviour: OnReleased...isKinematic=false, useGravity=true");
        isGrabbed = false;
        rigidbody.isKinematic = false;
        rigidbody.useGravity = true;
        releasing = true;
    }

    [Command]
    public void CmdAddForce(Vector3 forcevector, float throwingSpeedFactor)
    {
        Debug.Log("command add force");
        rigidbody.AddForce(forcevector * throwingSpeedFactor * rigidbody.mass);
    }

    //[ClientRpc]
    //public void RpcAddClientForce(Vector3 forcevector, float throwingSpeedFactor)
    //{
    //    Debug.Log("clientrpc add force");
    //    rigidbody.AddForce(forcevector * throwingSpeedFactor * rigidbody.mass);
    //}

    private void GetHands()
    {
        if (handLeft == null || handRight == null)
        {
            Actor actor = authorityManager.GetActor();
            if (actor != null)
            {
                GameObject go = actor.gameObject;
                if (go != null)
                {
                    Transform trParent = go.transform.parent;
                    if (trParent != null)
                    {
                        if (trParent.tag == "vive")
                        {
                            if (handLeft == null)
                            {
                                handLeft = authorityManager.GetActor().transform.Find("ViveHands/Left").gameObject;
                            }
                            if (handRight == null)
                            {
                                handRight = authorityManager.GetActor().transform.Find("ViveHands/Right").gameObject;
                            }
                        }
                        else if (trParent.tag == "leap")
                        {
                            if (handLeft == null)
                            {
                                handLeft = GameObject.FindGameObjectWithTag("LeftHandInteraction");
                            }
                            if (handRight == null)
                            {
                                handRight = GameObject.FindGameObjectWithTag("RightHandInteraction");
                            }
                        }
                    }
                }
            }
        }
    }
}
