using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpItem : MonoBehaviour
{
    public string NameOfObject;
    public VideoManager videoManager;
    // Start is called before the first frame update
    void Start()
    {
        if (videoManager == null)
            videoManager = GameObject.FindGameObjectWithTag("VideoManager").GetComponent<VideoManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ItemPickedUp()
    {
        videoManager.PickedUp(NameOfObject);
    }

}
