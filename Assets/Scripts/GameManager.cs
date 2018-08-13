using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public ApartmentObject[] gifts;
    int giftIndex;
    public Transform dropOffPoint;
    public GameObject giftPrefab;

	// Use this for initialization
	void Start () {
        StartCoroutine("DropOffGift");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator DropOffGift()
    {
        GameObject gift = Instantiate(giftPrefab, dropOffPoint.position, Quaternion.identity);
        //gift.transform.localScale = new Vector3(gifts[giftIndex].width, 1, gifts[giftIndex].length);
        var giftProperties = gift.GetComponent<PlaceableObject>();
        giftProperties.Properties = gifts[giftIndex];
        giftIndex++;
        if (giftIndex < gifts.Length)
        {
            yield return new WaitForSeconds(3);
            StartCoroutine("DropOffGift");
        }
    }
}

[System.Serializable]
public class ApartmentObject
{
    public string Name;
    //public ApartmentObjectType type;
    public PlacableOn[] placeables;
    public int width;
    public int length;
    public Vector2 Size { get { return new Vector2(width, length); } }
    public GameObject prefab;
    public Transform PrefabTransform { get { return prefab.transform; } }
    public Transform PrefabChildTransform { get { return prefab.transform.GetChild(0); } }
    public Vector3 ChildScaleOffset { get
        {
            Vector3 off = Vector3.one;

            //switch (placeables[0])
            //{
            //    case PlacableOn.Floor:
            //        break;
            //    case PlacableOn.Furniture:
                    
            //        break;
            //    case PlacableOn.Wall:
            //        off = new Vector3(0.001f, 1, 1);

            //        break;
            //    case PlacableOn.Shelf:
                    

            //        break;
            //    default:
            //        break;
            //}
            off = prefab.transform.GetChild(0).localScale;
            return off;
        } }
    public Vector3 PositionOffset { get
        {
            Vector3 off = Vector3.zero;

            switch (placeables[0])
            {
                case PlacableOn.Floor:
                    break;
                case PlacableOn.Furniture:
                    off = new Vector3(0, 1, 0);
                    break;
                case PlacableOn.Wall:
                    off = new Vector3(0, 2, 0);

                    break;
                case PlacableOn.Shelf:
                    off = new Vector3(0, 2, 0);

                    break;
                default:
                    break;
            }
            return off;

        }
    }
    public Vector3 ChildOffset { get {
            Vector3 off = Vector3.zero;
            switch (placeables[0])
            {
                case PlacableOn.Floor:
                    off = new Vector3(0.5f, 0, 0.5f);
                    break;
                case PlacableOn.Furniture:
                    off = new Vector3(0.5f, 0, 0.5f);
                    break;
                case PlacableOn.Wall:
                    off = new Vector3(0.5f, 0, 0.5f);
                    break;
                case PlacableOn.Shelf:
                    off = new Vector3(0.5f, 0.6f, 0.5f);
                    break;
                default:
                    break;
            }
            return off;
        } }
}

public enum PlacableOn { Floor, Furniture, Wall, Shelf }
