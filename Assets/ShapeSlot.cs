using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class ShapeSlot : MonoBehaviour
{

    public GameObject myChild;

    public SpriteShapeController spriteShapeController = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetChildShape(GameObject child)
    {
        foreach (Transform item in transform)
        {
            Destroy(item);
        }

        child.transform.parent = transform;
        child.transform.localPosition = Vector3.zero;
        child.transform.localRotation = Quaternion.identity;

        myChild = child;
        spriteShapeController = myChild.GetComponent<SpriteShapeController>();
    }



}
