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
        // Update current pose in queue
        poses[0] = transform.position; 

        // Proccess user interface
        EditPath();

        // Update all pathes
        List<NavMeshPath> path_list = GetPathes();

        // Show gloabal path
        RenderPath(path_list);

        // Check local goal state
        if (path_list.Count > 0){
            agent.SetPath(path_list[0]);
            if (agent.path.corners.Length < 2){
                poses.RemoveAt(1);
            }
        }
        
    }

    // Implementation of user interface with path planner
    private void EditPath(){
        if (Input.GetMouseButtonDown(0)){
            Ray CameraToMouse = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            
            if (Physics.Raycast(CameraToMouse, 
                out hitInfo,  Mathf.Infinity, canBeClicked)) {
                poses.Add(hitInfo.point);
            }
        } else if (Input.GetMouseButtonDown(1)) {
            Ray CameraToMouse = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            
            if (Physics.Raycast(CameraToMouse, 
                out hitInfo,  Mathf.Infinity, canBeClicked)) {
                
            }
        }
    }

    // Get list of pathes to execute
    private List<NavMeshPath> GetPathes(){
        List<NavMeshPath> path_list = new List<NavMeshPath>();
        if (poses.Count > 1){
            for (int i = 0; i < poses.Count - 1; ++i){
                path_list.Add(new NavMeshPath());
                NavMesh.CalculatePath(poses[i], poses[i+1], 
                    canBeClicked, path_list[i]);
            }
        }
        return path_list;
    }

    // Draw line through all pathes
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
