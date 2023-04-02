using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenController : MonoBehaviour
{
    [SerializeField]
    private Transform controllerBack;
    [SerializeField]
    private Transform controllerPoint;
    [SerializeField]
    private float additionalBack = 30;
    private bool isStickMoving = false;
    private Vector3 anchorPoint;
    public float padDirectionX = 0;
    public float padDirectionY = 0;
    [SerializeField]
    TMPro.TMP_Text healthText;
    [SerializeField]
    TMPro.TMP_Text goldText;
    public UnityEngine.UI.Image colorElement;
    void Start()
    {
        anchorPoint = controllerPoint.position;
    }

    // Update is called once per frame
    void Update()
    {
        padDirectionX = (controllerPoint.position.x - anchorPoint.x) / (controllerBack.GetComponent<RectTransform>().sizeDelta.x / 2 + additionalBack);
        padDirectionY = (controllerPoint.position.y - anchorPoint.y) / (controllerBack.GetComponent<RectTransform>().sizeDelta.y / 2 + additionalBack);
        if(Input.GetMouseButtonDown(0))
        {
            isStickMoving = true;
        }
        if(Input.GetMouseButtonUp(0))
        {
            controllerPoint.position = anchorPoint;
            isStickMoving = false;
        }
 
        if(isStickMoving)
        {
            controllerPoint.position = Vector3.Lerp(controllerPoint.position, Input.mousePosition, Time.deltaTime * 10);
            // Stick Bounds
            // --------------
            // Horizontal Axis
            if((controllerPoint.position.x - anchorPoint.x) > controllerBack.GetComponent<RectTransform>().sizeDelta.x / 2 + additionalBack)
            {
                controllerPoint.position = new Vector3(anchorPoint.x + controllerBack.GetComponent<RectTransform>().sizeDelta.x / 2 + additionalBack, controllerPoint.position.y, controllerPoint.position.z);
            }
            if((controllerPoint.position.x - anchorPoint.x) < -controllerBack.GetComponent<RectTransform>().sizeDelta.x / 2 - additionalBack)
            {
                controllerPoint.position = new Vector3(anchorPoint.x + -controllerBack.GetComponent<RectTransform>().sizeDelta.x / 2 - additionalBack, controllerPoint.position.y, controllerPoint.position.z);
            }
            // Vertical Axis
            if((controllerPoint.position.y - anchorPoint.y) < -controllerBack.GetComponent<RectTransform>().sizeDelta.y / 2 - additionalBack)
            {
                controllerPoint.position = new Vector3(controllerPoint.position.x, anchorPoint.y + -controllerBack.GetComponent<RectTransform>().sizeDelta.y / 2 - additionalBack, controllerPoint.position.z);
            }
            if((controllerPoint.position.y - anchorPoint.y) > controllerBack.GetComponent<RectTransform>().sizeDelta.y / 2 + additionalBack)
            {
                controllerPoint.position = new Vector3(controllerPoint.position.x, anchorPoint.y + controllerBack.GetComponent<RectTransform>().sizeDelta.y / 2 + additionalBack, controllerPoint.position.z);
            }
        }
    }
    public void UpdateHud(int health, int gold)
    {
        goldText.text = "Gold: " + gold;
        healthText.text = "HP: " + health;
    }
}
