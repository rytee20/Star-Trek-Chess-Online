using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pin : MonoBehaviour
{
    public bool isThereAttackBoard;

    //private Camera currentCamera;
    //private Vector3Int currentHoverPin = -Vector3Int.one;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if (!currentCamera)
        //{
        //    currentCamera = Camera.main;
        //    return;
        //}

        //RaycastHit info;
        //Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
        //if (Physics.Raycast(ray, out info, 500, LayerMask.GetMask("Tile", "Hover", "Highlight")))
        //{
        //    // Get the indexes of the tile i've hit
        //    Vector3Int hitPositionPin = LookUpPinIndex(info.transform.gameObject);

        //    // if we're covering a tile after not hovering any tiles

        //    if (currentHoverPin == -Vector3Int.one)
        //    {
        //        currentHoverPin = hitPositionPin;
        //        pins[hitPositionPin.x, hitPositionPin.y, hitPositionPin.z].GetComponent<MeshRenderer>().material = hoverMaterial;
        //    }

        //    // if we were already covering a tile, change the previous one

        //    if (currentHoverPin != -Vector3Int.one && currentHoverPin != hitPositionPin)
        //    {
        //        pins[currentHoverPin.x, currentHoverPin.y, currentHoverPin.z].GetComponent<MeshRenderer>().material = tileMaterial;
        //        currentHoverPin = hitPositionPin;
        //        pins[hitPositionPin.x, hitPositionPin.y, hitPositionPin.z].GetComponent<MeshRenderer>().material = hoverMaterial;
        //    }
        //}
        //else
        //{

        //    if (currentHoverPin != -Vector3Int.one)
        //    {
        //        //Debug.Log(currentHover.x.ToString()+'\t'+currentHover.y.ToString()+ '\t' + currentHover.z.ToString());
        //        pins[currentHoverPin.x, currentHoverPin.y, currentHoverPin.z].GetComponent<MeshRenderer>().material = tileMaterial;

        //        currentHoverPin = -Vector3Int.one;
        //    }
        //}
    }
}
