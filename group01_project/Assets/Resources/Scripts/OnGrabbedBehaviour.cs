using UnityEngine;
using UnityEngine.Networking;

// TODO: define the behaviour of a shared object when it is manipulated by a client

public class OnGrabbedBehaviour : MonoBehaviour
{
    bool grabbed;
    //Actor currentActor;

    Rigidbody rigidbody = null;
    NetworkIdentity netId = null;
    AuthorityManager authorityManager = null;


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
        if (grabbed)
        {
            Debug.Log("OnGrabbedBehaviour: grabbed = true");
            if (authorityManager.GetActor() == null || !netId.hasAuthority) return;
            Debug.Log("OnGrabbedBehaviour: authorityManager.GetActor().gameObject.tag = " + authorityManager.GetActor().gameObject.tag);
            if (authorityManager.GetActor().gameObject.tag == "vive")   // VIVE grabbed
            {
                if (authorityManager.GetLeftGrabbed())  // Left hand
                {
                    GameObject hand = authorityManager.GetActor().transform.Find("ViveHands/Left").gameObject;
                    if (hand)
                    {
                        this.transform.position = hand.transform.position;
                    }
                }
                else    //Right hand
                {
                    GameObject hand = authorityManager.GetActor().transform.Find("ViveHands/Right").gameObject;
                    if (hand)
                    {
                        this.transform.position = hand.transform.position;
                    }
                }
            }
            else if (authorityManager.GetActor().gameObject.tag == "leap")  // Leap grabbed
            {
                Debug.Log("OnGrabbedBehaviour: grabbed = true");
                if (authorityManager.GetLeftGrabbed())  // Left hand
                {
                    GameObject hand = GameObject.FindGameObjectWithTag("LeftHandInteraction");
                    if (hand)
                    {
                        Debug.Log("OnGrabbedBehaviour: Left-Hand grabbed move object to position=" + hand.transform.position);
                        this.transform.position = hand.transform.position;
                    }
                }
                else    //Right hand
                {
                    GameObject hand = GameObject.FindGameObjectWithTag("RightHandInteraction");
                    if (hand)
                    {
                        Debug.Log("OnGrabbedBehaviour: Right-Hand grabbed move object to position=" + hand.transform.position);
                        this.transform.position = hand.transform.position;
                    }
                }
            }
        }
    }

    // called first time when the GO gets grabbed by a player
    public void OnGrabbed()
    {
        Debug.Log("OnGrabbedBehaviour: OnGrabbed...isKinematic=true, useGravity=false");
        grabbed = true;
        rigidbody.isKinematic = true;
        rigidbody.useGravity = false;

    }

    // called when the GO gets released by a player
    public void OnReleased()
    {
        Debug.Log("OnGrabbedBehaviour: OnReleased...isKinematic=false, useGravity=true");
        grabbed = false;
        rigidbody.isKinematic = false;
        rigidbody.useGravity = true;
    }
}
