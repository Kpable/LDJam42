using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    public float speed = 10f;
    public GameObject carying;
    public Transform target;
    List<Collider> nearbyObjects;
    public GameObject validator;
    bool canPlace = true;

    Vector3 debugCube, debugSize;
    
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
        //RaycastHit hit;
        Vector3 mousePos = Input.mousePosition;

        ray = Camera.main.ScreenPointToRay(mousePos);
        //if (Physics.Raycast(ray, out hit))
        //{
        //    if (hit.collider.name == "Floor")
        //    {
        //        //Debug.Log("Mouse Hit:" + hit.point);
        //        transform.LookAt(new Vector3(hit.point.x, transform.position.y, hit.point.z));
        //    }
        //}

        RaycastHit[] camRayHits = Physics.RaycastAll(ray);
        foreach (var col in camRayHits)
        {
            if (col.collider.name == "Floor")
            {
                transform.LookAt(new Vector3(col.point.x, transform.position.y, col.point.z));
            }
        }

        if (carying)
        {
            Vector3 validatorPos = validator.transform.position;
            validatorPos.x = Mathf.RoundToInt(target.position.x + transform.forward.x);
            validatorPos.z = Mathf.RoundToInt(target.position.z + transform.forward.z);
            validator.transform.position = validatorPos;
            PlacableOn[] canBePlacedOn = carying.GetComponent<PlaceableObject>().Properties.placeables;

            if (Array.IndexOf(canBePlacedOn, PlacableOn.Wall) != -1)
                RotateValidator();
            Validate();
            
        }
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
                if(canPlace)
                    ToggleCarry(carying);
            }
;        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(debugCube, debugSize);
    }

    private void Validate()
    {
        Vector3 scale = validator.transform.localScale;
        Vector3 pos = validator.transform.position;

        //Debug.Log("pos: " + pos + " scale: " + scale);
        var props = carying.GetComponent<PlaceableObject>().Properties;

        Color validationColor = new Color(Color.green.r, Color.green.g, Color.green.b, 0.4f);

        bool valid = true;

        if (IsCollidingWithAnything(scale, pos) || !IsOverPlaceableOnSurface(scale, pos))
        {
            validationColor = new Color(Color.red.r, Color.red.g, Color.red.b, 0.4f);
            valid = false;
        }
       
        validator.transform.GetChild(0).GetComponent<Renderer>().material.color = validationColor;
        canPlace = valid;
    }

    private bool IsOverPlaceableOnSurface(Vector3 scale, Vector3 pos)
    {
        bool ret = false;


        PlacableOn[] canBePlacedOn = carying.GetComponent<PlaceableObject>().Properties.placeables;
        foreach (var placedOnAble in canBePlacedOn)
        {

            Vector3[] pointsToCheck = new Vector3[Mathf.RoundToInt(scale.x * scale.z)];
            //Debug.Log("Points to check: " + pointsToCheck.Length);
            int index = 0;
            for (int x = 0; x < scale.x; x++)
            {
                for (int z = 0; z < scale.z; z++)
                {
                    if(placedOnAble == PlacableOn.Wall)
                        pointsToCheck[index] = new Vector3(pos.x + x + 0.5f, pos.y, pos.z + z + 0.5f);
                    else if(placedOnAble == PlacableOn.Shelf)
                        pointsToCheck[index] = new Vector3(pos.x + x + 0.5f, pos.y - 0.25f, pos.z + z + 0.5f);
                    else
                        pointsToCheck[index] = new Vector3(pos.x + x + 0.5f, pos.y - 1, pos.z + z + 0.5f);
                    Debug.Log("pointToCheck: " + pointsToCheck[index]);

                    pointsToCheck[index] = pos + (Quaternion.AngleAxis(validator.transform.eulerAngles.y, Vector3.up) * (pointsToCheck[index] - validator.transform.position));
                    Debug.Log("pointToCheck rotated by" + validator.transform.eulerAngles.y + ": " + pointsToCheck[index]);

                    index++;
                }
            }

            int validPoints = 0;
            for (int i = 0; i < pointsToCheck.Length; i++)
            {
                Collider[] hits = Physics.OverlapBox(pointsToCheck[i], Vector3.one * 0.45f);
                //Debug.Log("found " + hits.Length + " hits ");

                //for (int u = 0; u < hits.Length; u++)
                //{
                //    Debug.Log("hit " + hits[u].name);
                //}
                debugCube = pointsToCheck[i];
                debugSize = Vector3.one * 0.45f;
                if (hits.Length == 0) break;
                for (int j = 0; j < hits.Length; j++)
                {
                    var surface = hits[j].GetComponent<Surface>();
                    if (surface != null)
                    {

                        if (surface.type == placedOnAble)
                        {
                            validPoints++;
                            break;
                        }
                    }
                }
            }
            ret = validPoints == pointsToCheck.Length;
            if (ret) break;
        }

        Debug.Log("IsOverPlaceableOnSurface: " + ((ret) ? "true" : "false"));
        return ret; 
    }

    private bool IsCollidingWithAnything(Vector3 scale, Vector3 pos)
    {
        bool ret = false;

        PlacableOn[] canBePlacedOn = carying.GetComponent<PlaceableObject>().Properties.placeables;
        foreach (var placedOnAble in canBePlacedOn)
        {
            Collider[] hits = Physics.OverlapBox(new Vector3(pos.x + scale.x / 2, pos.y, pos.z + scale.z / 2), new Vector3(scale.x / 2 - .01f, scale.y / 2 - .01f, scale.z / 2 - .01f));
            for (int j = 0; j < hits.Length; j++)
            {
                if (hits[j].CompareTag("Movable") && placedOnAble != PlacableOn.Wall && placedOnAble != PlacableOn.Shelf)
                {
                    //Debug.Log("found " + j + " " + hits[j].name);
                    ret = true;
                    break;
                }
            }
            //debugCube = new Vector3(pos.x + scale.x / 2, pos.y, pos.z + scale.z / 2);
            //debugSize = new Vector3(scale.x, scale.y, scale.z);
            Debug.Log("IsCollidingWithAnything: " + ((ret) ? "true" : "false"));
        }
        return ret;
    }

    private void RotateValidator()
    {
        validator.transform.LookAt(transform);

        Vector3 rot = validator.transform.eulerAngles;

        rot.x = 0;
        rot.y = Mathf.Round(rot.y / 90) * 90;
        rot.z = 0;
        validator.transform.eulerAngles = rot;

    }

    public void ToggleCarry(GameObject movable)
    {
        if (carying)
        {
            validator.SetActive(false);

            var props = carying.GetComponent<PlaceableObject>().Properties;
            GameObject apartmentObject = Instantiate(props.prefab, validator.transform.position, validator.transform.rotation);
            if(Array.IndexOf(props.placeables, PlacableOn.Wall) == -1)
                apartmentObject.transform.GetChild(0).localPosition = props.ChildOffset;
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
            validator.transform.position = props.PrefabTransform.position;
            validator.transform.position += props.PositionOffset;

            //validator.transform.GetChild(0).localPosition = props.PrefabChildTransform.localPosition;
            validator.transform.GetChild(0).localPosition = props.ChildOffset;
            //validator.transform.GetChild(0).localScale = props.PrefabChildTransform.localScale;
            //validator.transform.GetChild(0).rotation = props.PrefabChildTransform.rotation;

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


