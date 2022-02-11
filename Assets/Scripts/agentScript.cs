using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class agentScript : MonoBehaviour
{
    public LayerMask canBeClicked;
    
    public int corners;
    
    private List<Vector3> target_list = new List<Vector3>();
    NavMeshAgent agent;
    LineRenderer pathRender;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        pathRender = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        addTarget();
        getPath(agent);
        calcPathes();
    }

    private void calcPathes(){
        for (int i = 1; i < target_list.Count; ++i){
            
        }
    }
    private void addTarget(){
        if (Input.GetMouseButtonDown(0)){
            Ray CameraToMouse = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            
            if (Physics.Raycast(CameraToMouse, 
                out hitInfo,  Mathf.Infinity, canBeClicked)) {
                target_list.Add(hitInfo.point);
            }
        }
    }
    private void getPath(NavMeshAgent agent){
        if (agent.path.corners.Length > 1){
            pathRender.positionCount = agent.path.corners.Length;
            for (int i = 0; i < agent.path.corners.Length; ++i){
                pathRender.SetPosition(i, agent.path.corners[i]);
            }
        } else if (target_list.Count > 0){
            agent.SetDestination(target_list[0]);
            target_list.RemoveAt(0);
        }
    }
}
