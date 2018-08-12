using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    public float speed = 10f;
    public GameObject carying;
    public Transform target;
    List<Collider> nearbyObjects;
    public GameObject validator;
    
    
	// Use this for initialization
	void Start () {
        nearbyObjects = new List<Collider>();
        target = transform.GetChild(0);
        validator.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
        float hAxis = Input.GetAxisRaw("Horizontal");
        float vAxis = Input.GetAxisRaw("Vertical");

        Vector3 pos = transform.position;
        pos.x += hAxis * speed * Time.deltaTime;
        pos.z += vAxis * speed * Time.deltaTime;
        transform.position = pos;

        Ray ray;
        RaycastHit hit;
        Vector3 mousePos = Input.mousePosition;

        ray = Camera.main.ScreenPointToRay(mousePos);
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.name == "Floor")
            {
                //Debug.Log("Mouse Hit:" + hit.point);
                transform.LookAt(new Vector3(hit.point.x, transform.position.y, hit.point.z));
            }
        }


        Vector3 validatorPos = validator.transform.position;
        validatorPos.x = Mathf.RoundToInt(target.position.x + transform.forward.x);
        validatorPos.z = Mathf.RoundToInt(target.position.z + transform.forward.z);
        validator.transform.position = validatorPos;

        // pick up/putdown
        if (Input.GetMouseButtonDown(0))
        {
            if (!carying)
            {
                GameObject carry = FindNearestObject();
                if (carry != null) ToggleCarry(carry);
            }
            else
            {
                ToggleCarry(carying);
            }
;        }
    }

    public void ToggleCarry(GameObject movable)
    {
        if (carying)
        {
            validator.SetActive(false);

            var props = carying.GetComponent<PlaceableObject>().Properties;
            GameObject apartmentObject = Instantiate(props.prefab, validator.transform.position, validator.transform.rotation);

            carying.SetActive(false);
            //carying.transform.position = validator.transform.position;
            //carying.transform.rotation = validator.transform.rotation;
            carying.transform.SetParent(null);
            carying.transform.localScale = Vector3.one;



            carying = null;
        }
        else // Not Carrying
        {
            var props = movable.GetComponent<PlaceableObject>().Properties;
            validator.transform.localScale = new Vector3(props.width, 1, props.length);
            validator.transform.position = props.PositionOffset;
            validator.transform.GetChild(0).localPosition = props.ChildOffset;
            validator.SetActive(true);

            carying = movable.gameObject;
            carying.transform.position = target.position;
            carying.transform.rotation = target.rotation;
            carying.transform.localScale = Vector3.one * 0.3f;
            carying.transform.SetParent(transform);
        }
        
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Movable"))
        {
            //Debug.Log(name + " triggered by " + other.name);
            nearbyObjects.Add(other);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Movable"))
        {
            //Debug.Log(name + " triggered by " + other.name);
            nearbyObjects.Remove(other);
        }
    }

    GameObject FindNearestObject()
    {
        float minDistance = float.MaxValue;
        GameObject nearest = null;
        foreach(var movable in nearbyObjects)
        {
            float distance = Vector3.Distance(transform.position, movable.transform.position);
            if(distance < minDistance)
            {
                nearest = movable.gameObject;
                minDistance = distance;
            }
        }
        return nearest;
    }
}


