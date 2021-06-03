using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HandPresence : MonoBehaviour
{
    public XRNode inputSource;
    private InputDevice device;

    public GameObject handModelPrefab;
    public GameObject spawedHandModel;

    private Animator handAnimator;

    // Start is called before the first frame update
    void Start()
    {
        TryInit();
    }

    void TryInit()
    {
        device = InputDevices.GetDeviceAtXRNode(inputSource);
        spawedHandModel = Instantiate(handModelPrefab, transform);
        handAnimator = spawedHandModel.GetComponent<Animator>();
    }

    void UpdateHandAnimator()
    {
        if (device.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue))
        {
            handAnimator.SetFloat("Trigger", triggerValue);
        }
        else
        {
            handAnimator.SetFloat("Trigger", 0);
        }

        if (device.TryGetFeatureValue(CommonUsages.grip, out float grip))
        {
            handAnimator.SetFloat("Grip", grip);
        }
        else
        {
            handAnimator.SetFloat("Grip", 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHandAnimator();
    }
}
