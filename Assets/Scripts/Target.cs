using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour {
    public Transform player;
    public Player playerScript;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerScript = player.GetComponent<Player>();
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        Vector3 pos = player.GetChild(0).position;
        pos.x = Mathf.RoundToInt(pos.x);
        pos.z = Mathf.RoundToInt(pos.z);
        transform.position = pos;

        if (Input.GetMouseButtonDown(0))
        {
            Collider[] hits = Physics.OverlapBox(pos, new Vector3(0.5f, 0.5f, 0.5f));

            if (hits.Length > 0)
            {
                for (int i = 0; i < hits.Length; i++)
                {
                    Debug.Log(name + " triggered by " + hits[i].name);
                    if (hits[i].CompareTag("Movable"))
                    {
                        playerScript.ToggleCarry(hits[i]);
                        
                    }
                }

            }
        }
    }
}
