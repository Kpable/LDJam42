using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceableObject : MonoBehaviour {

    [SerializeField]
    ApartmentObject gift;
    public ApartmentObject Properties { get { return gift; } set { gift = value; } }
	
    // Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
