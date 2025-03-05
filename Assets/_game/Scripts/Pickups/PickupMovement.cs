using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupMovement : MonoBehaviour
{
    //[SerializeField] float rotationSpeedRight = 150f;
    //[SerializeField] float rotationSpeedDown = 100f;
    [SerializeField] private float rotationSpeed = 90;
    [SerializeField] private float bounceAmplitude = 0.05f;
    [SerializeField] private float bounceSpeed = 8;
    private float startingHeight;
    private float timeOffset;
    [SerializeField] bool uiItem = false;
    [SerializeField] bool floating;
    void Start()
    {
        startingHeight = transform.localPosition.y;
        timeOffset = Random.value * Mathf.PI * 2;
    }

    void Update()
    {
        //transform.Rotate(Vector3.right * rotationSpeedRight * Time.deltaTime, Space.Self);
        //transform.Rotate(Vector3.down * rotationSpeedDown * Time.deltaTime, Space.Self);
        if (!floating)
        {
            if (!CheckGround())
            {
                transform.Translate(Vector3.down * 5 * Time.deltaTime);
            }
        }
        Vector3 rotation = transform.localRotation.eulerAngles;
        if (!uiItem)
        {
            rotation.y += rotationSpeed * Time.deltaTime;
        }
        else
        {
            rotation.y += rotationSpeed * Time.unscaledDeltaTime;
        }
       transform.localRotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
       float finalHeight = startingHeight + Mathf.Sin(Time.time * bounceSpeed + timeOffset) * bounceAmplitude;
       var position = transform.localPosition;
       position.y = finalHeight;
       transform.localPosition = position;
    }

    bool CheckGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1))
        {
            if (!hit.collider.isTrigger)
            {
                floating = true;
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }
}
