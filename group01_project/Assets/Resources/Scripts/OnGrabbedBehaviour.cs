using UnityEngine;
using UnityEngine.Networking;

// TODO: define the behaviour of a shared object when it is manipulated by a client

public class OnGrabbedBehaviour : MonoBehaviour
{
    private bool isGrabbed;
    private bool releasing;
    private Vector3 oldPos;
    private Vector3 newPos;

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
            oldPos = this.transform.position;
            this.transform.position = handLeft.transform.position;
            newPos = this.transform.position;
            Vector3 speed = (newPos - oldPos) / Time.deltaTime;
            Debug.Log("speed=" + speed * 1000f * rigidbody.mass);
        }
        else if (isGrabbed && !authorityManager.GetLeftGrabbed() && handRight != null && netId.hasAuthority)
        {
            oldPos = this.transform.position;
            this.transform.position = handRight.transform.position;
            newPos = this.transform.position;
            Vector3 speed = (newPos - oldPos) / Time.deltaTime;
            Debug.Log("speed=" + speed * 1000f * rigidbody.mass);
        }
        else if (releasing)
        {
            Vector3 speed = (newPos - oldPos) / Time.deltaTime;
            Debug.Log("throwing speed=" + speed * 1000f * rigidbody.mass);
            rigidbody.AddForce(speed * 1000f * rigidbody.mass);
            releasing = false;
        }
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
