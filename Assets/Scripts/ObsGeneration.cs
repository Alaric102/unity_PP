using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ObsGeneration : MonoBehaviour
{    

    private GameObject MyCube;
    private GameObject MySphere;
    
    void Start()
    {
        MyCube = GameObject.Find("Coube");
        MyCube.SetActive(false);
        MySphere = GameObject.Find("Sphere");
        MySphere.SetActive(false);

    }

    void Update()
    {
        if (Input.GetMouseButtonDown(2))
        {
            Vector3 mouse = Input.mousePosition;
            Ray castPoint = Camera.main.ScreenPointToRay(mouse);
            RaycastHit hit;
            if (Physics.Raycast(castPoint, out hit, Mathf.Infinity))
            {
                int param = Random.Range(0, 2);

                if (param == 0)
                {
                    GameObject newCoube = Instantiate(MyCube, new Vector3(hit.point.x, 1/2, hit.point.z), Quaternion.identity);
                    newCoube.SetActive(true);

                }
                else if(param == 1)
                {
                    GameObject newSphere = Instantiate(MySphere, new Vector3(hit.point.x, 1/2, hit.point.z), Quaternion.identity);
                    newSphere.SetActive(true);
                }

            }   
        }
    }
}
