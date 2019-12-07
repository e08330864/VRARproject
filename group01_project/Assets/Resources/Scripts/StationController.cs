using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class StationController : MonoBehaviour
{
    Transform station_head = null;

    public GameObject lightningPrefab = null;
    private GameObject lightning = null;

    // Start is called before the first frame update
    void Start()
    {
        foreach(Transform child in transform)
        {
            if(child.tag == "StationHead")
            {
                station_head = child;
            }
        }
        if ((lightning = Instantiate(lightningPrefab, transform.position, Quaternion.identity)) == null)
        {
            Debug.LogError("lightning is NULL in StationController");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(station_head != null)
        {
            Vector3 scale = station_head.localScale;
            float delta = (0.05f * Mathf.Sin(Time.time)) + 1;
            scale.x = delta;
            scale.z = delta;
            station_head.localScale = scale;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("StationController: in collieder");
        if (other.gameObject.tag == "TowerElement")
        {
            Debug.Log("StationController: is TowerElement");
            if (!other.gameObject.GetComponent<NetworkIdentity>().hasAuthority)
            {
                Debug.Log("StationController: no Authority");
                StartCoroutine(ShowLightning());
            }
        }
    }

    IEnumerator ShowLightning()
    {
        lightning.SetActive(true);
        yield return new WaitForSeconds(20);
        lightning.SetActive(false);
    }
}
