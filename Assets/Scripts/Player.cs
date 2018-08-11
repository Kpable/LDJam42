using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    public float speed = 10f; 
	// Use this for initialization
	void Start () {
		
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
                transform.LookAt(hit.point);
            }
        }

    }
}
