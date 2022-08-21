using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public BlendShape blendShape;

    public List<ShapeSlot> shapeSlots;

    public List<GameObject> allShapes;

    // Start is called before the first frame update
    void Start()
    {
        var selectedShapePerfabs = new List<GameObject>();

        while (selectedShapePerfabs.Count < shapeSlots.Count)
        {
            selectedShapePerfabs.AddUnique(allShapes.RandomElement()); 
        }

        var shapePrefabs = new List<GameObject>();

        selectedShapePerfabs.ForEach(s => shapePrefabs.Add(Instantiate(s.gameObject ,Vector3.zero, Quaternion.identity, null)));

        for (int i = 0; i < shapeSlots.Count; i++)
        {
            shapeSlots[i].SetChildShape(shapePrefabs[i]);
        }

        blendShape.Setup();

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

    }
}
