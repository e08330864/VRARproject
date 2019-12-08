using UnityEngine;

public class StationController : MonoBehaviour
{
    Transform station_head = null;
    

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

   
}
