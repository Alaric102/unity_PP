using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class agentScript : MonoBehaviour
{
    public LayerMask canBeClicked;
    
    public int corners;
    
    List<Vector3> poses = new List<Vector3>();
    NavMeshAgent agent;
    LineRenderer pathRender;
    LineRenderer secondaryPathRender;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        pathRender = GetComponent<LineRenderer>();
        pathRender.positionCount = 0;
        poses.Add(transform.position);
        
    }

    // Update is called once per frame
    void Update()
    {
        addTarget();
        poses[0] = transform.position;
        List<NavMeshPath> path_list = GetPath();
        RenderPath(path_list);

        if (path_list.Count > 0){
            agent.SetPath(path_list[0]);
            if (agent.path.corners.Length < 2){
                poses.RemoveAt(1);
            }
        }
        
    }

    private void addTarget(){
        if (Input.GetMouseButtonDown(0)){
            Ray CameraToMouse = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            
            if (Physics.Raycast(CameraToMouse, 
                out hitInfo,  Mathf.Infinity, canBeClicked)) {
                poses.Add(hitInfo.point);
            }
        }
    }
    private List<NavMeshPath> GetPath(){
        List<NavMeshPath> path_list = new List<NavMeshPath>();
        
        if (poses.Count > 1){
            for (int i = 0; i < poses.Count - 1; ++i){
                path_list.Add(new NavMeshPath());
                NavMesh.CalculatePath(poses[i], poses[i+1], 
                    NavMesh.AllAreas, path_list[i]);
            }
        }
        return path_list;
    }
    private void RenderPath(List<NavMeshPath> path_list){
        int vertex_num = 0;
        foreach (var path in path_list)
            vertex_num += path.corners.Length;

        if (vertex_num == 0)
            return;

        pathRender.positionCount = vertex_num;
        int counter = 0;
        foreach (var path in path_list){
            foreach (var corner in path.corners){
                pathRender.SetPosition(counter, corner);
                counter++;
            }
        }
        
    }
}
