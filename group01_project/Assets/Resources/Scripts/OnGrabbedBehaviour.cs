using UnityEngine;
using UnityEngine.Networking;

// TODO: define the behaviour of a shared object when it is manipulated by a client

public class OnGrabbedBehaviour : MonoBehaviour
{
    bool held;
    //Actor currentActor;

    Rigidbody rigidbody = null;
    NetworkIdentity netId = null;
    AuthorityManager authorityManager = null;
    GameObject handLeft = null;
    GameObject handRight = null;

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
        if (handLeft != null && held && authorityManager.GetLeftGrabbed() && netId.hasAuthority)
        {
            
            this.transform.position = handLeft.transform.position;
            
        }
        if (handRight != null && held && !authorityManager.GetLeftGrabbed() && netId.hasAuthority)
        {
            this.transform.position = handRight.transform.position;
        }
    }

    // called first time when the GO gets grabbed by a player
    public void OnGrabbed()
    {
        Debug.Log("OnGrabbedBehaviour: OnGrabbed...isKinematic=true, useGravity=false");
        held = true;
        rigidbody.isKinematic = true;
        rigidbody.useGravity = false;

    }

    // called when the GO gets released by a player
    public void OnReleased()
    {
        Debug.Log("OnGrabbedBehaviour: OnReleased...isKinematic=false, useGravity=true");
        held = false;
        rigidbody.isKinematic = false;
        rigidbody.useGravity = true;
    }

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
