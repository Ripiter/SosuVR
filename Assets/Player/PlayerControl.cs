using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
public class PlayerControl : MonoBehaviour
{
    // Public attributes
    public XRNode inputSource;
    public float characterSpeed;
    public float additionalHeight = 0.2f;
    public VideoPlayer videoPlayer;
    public Transform RoomParent;
    //public GameObject mainMenuPrefab;
    private InputDevice device;

    private Vector2 inputAxis;

    private CharacterController character;
    private Vector3 direction;
    private XRRig rig;


    float gravity = -9.81f;
    float fallingSpeed = 0;
    public LayerMask layerMask;

    public float aditionalHeight = 0.2f;

    void Start()
    {
        character = GetComponent<CharacterController>();
        rig = GetComponent<XRRig>();
        device = InputDevices.GetDeviceAtXRNode(inputSource);
        //mainMenu = Instantiate(mainMenuPrefab);
    }

   


    void Update()
    {
        device.TryGetFeatureValue(CommonUsages.primary2DAxis, out inputAxis);
    }



    private void CapsuleFollowHeadset()
    {
        character.height = rig.cameraInRigSpaceHeight + additionalHeight;
        Vector3 capsuleCenter = transform.InverseTransformPoint(rig.cameraGameObject.transform.position);
        character.center = new Vector3(capsuleCenter.x, character.height / 2 + character.skinWidth, capsuleCenter.z);
    }

    private void FixedUpdate()
    {
        CapsuleFollowHeadset();

        Quaternion headYaw = Quaternion.Euler(0, rig.cameraGameObject.transform.eulerAngles.y, 0);
        direction = headYaw * new Vector3(inputAxis.x, 0, inputAxis.y);

        character.Move(direction * Time.deltaTime * characterSpeed);
        
        if (CheckIfGrounded())
            fallingSpeed = 0;
        else
            fallingSpeed += gravity * Time.fixedDeltaTime;
        
        character.Move(Vector3.up * fallingSpeed * Time.deltaTime);
    }

    bool CheckIfGrounded()
    {
        Vector3 rayStart = transform.TransformPoint(character.center);
        float rayLenght = character.center.y + 0.01f;

        bool hasHit = Physics.SphereCast(rayStart, character.radius, Vector3.down, out RaycastHit hitInfo, rayLenght, layerMask);

        return hasHit;
    }
}
