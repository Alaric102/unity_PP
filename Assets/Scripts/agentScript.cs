using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.InputSystem;

public class agentScript : MonoBehaviour
{
    public LayerMask canBeClicked;
    public GameObject editRegion;
    public GameObject nearestPoint;
    private List<Vector3> poses = new List<Vector3>();
    private NavMeshAgent agent;
    private LineRenderer pathRender;
    private Vector3 next_goal;
    private Vector3 edit_goal;
    
    public InputActionReference triggerAction;
    public InputActionReference primarryAction;
    public GameObject controller;
    public GameObject marker;

    // Start is called before the first frame update
    void Start()
    {
        editRegion.SetActive(false);
        nearestPoint.SetActive(false);

        agent = GetComponent<NavMeshAgent>();
        pathRender = GetComponent<LineRenderer>();

        pathRender.positionCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // Check local goal state
        if (poses.Count > 0){
            NavMeshPath path = new NavMeshPath();
            NavMesh.CalculatePath(transform.position, poses[0], canBeClicked, path);
            agent.SetPath(path);
            if (agent.path.corners.Length < 2){
                Debug.Log("Reached goal.");
                poses.RemoveAt(0);
            }
        }

        // Proccess new pose
        if (AddPose()){
            foreach (var pose in ProccessNextGoal().corners){
                poses.Add(pose);
            }
        }

        if (EditPose()){
            int closes_pose_id = FindPoseID();
            if ( closes_pose_id > -1){
                nearestPoint.transform.position = poses[closes_pose_id];
                nearestPoint.SetActive(true);
                if (movePose(closes_pose_id)){
                    Debug.Log("edit");
                    edit_goal.y = poses[closes_pose_id].y;
                    poses[closes_pose_id] = edit_goal;
                }
            }
        }
        
        // Show gloabal path
        RenderPath();
    }

    private bool lastTrigger = false;

    private bool GetRightControllerTrigger() {
        return (triggerAction.action.ReadValue<float>() == 1.0);
    }

    private bool GetRightControllerTriggerDown(){
        bool res = false;
        if (GetRightControllerTrigger() && (!lastTrigger)) {
            res = true;
        }

        lastTrigger = GetRightControllerTrigger();
        return res;
    }
    
    private bool lastSondary = false;

    private bool GetRightControllerSondary()
    {
        Debug.Log("Prime: " + primarryAction.action.ReadValue<float>());
        return (primarryAction.action.ReadValue<float>() == 1.0);
    }

    private bool GetRightControllerSecondaryDown()
    {
        bool res = false;
        if (GetRightControllerSondary() && (!lastSondary))
        {
            res = true;
        }

        lastSondary = GetRightControllerSondary();
        return res;
    }
    private bool GetRightControllerSecondary()
    {
        return (primarryAction.action.ReadValue<float>() == 1.0);
    }
    // Add new point and return true, else false
    private bool AddPose(){
        if (GetRightControllerTriggerDown() && !(GetRightControllerSecondary())){
            Ray CameraToMouse = new Ray(controller.transform.position, controller.transform.forward);
            RaycastHit hitInfo;

            Debug.DrawRay(controller.transform.position, controller.transform.forward, Color.green);

            if (Physics.Raycast(CameraToMouse, 
                out hitInfo,  Mathf.Infinity, canBeClicked)) {
                next_goal = hitInfo.point;
                return true;
            }
        } 
        return false;
    }
    private bool EditPose(){
        editRegion.SetActive(false);
        nearestPoint.SetActive(false);

        if (GetRightControllerSecondary())
        {
            Ray CameraToMouse = new Ray(controller.transform.position, controller.transform.forward);
            RaycastHit hitInfo;
            
            if (Physics.Raycast(CameraToMouse, 
                out hitInfo,  Mathf.Infinity, canBeClicked)) {
                editRegion.transform.position = hitInfo.point;
                editRegion.SetActive(true);
                return true;
            }
        } 
        return false;
    }
    // Get list of pathes to execute
    private NavMeshPath ProccessNextGoal(){
        NavMeshPath new_path = new NavMeshPath();
        if (poses.Count > 0){
            NavMesh.CalculatePath(poses[poses.Count - 1], next_goal, canBeClicked, new_path);
        } else {
            NavMesh.CalculatePath(transform.position, next_goal, canBeClicked, new_path);
        }
        return new_path;
    }


    private int FindPoseID(){
        if (poses.Count < 1) return -1;
        float min_distance = float.MaxValue;
        int pose_id = -1;
        for (int i = 0; i < poses.Count; ++i){
            float distance = Vector3.Distance(poses[i], editRegion.transform.position);
            if ((distance < editRegion.transform.localScale.x) && (distance < min_distance)){
                min_distance = distance;
                pose_id = i;
            }
        }
        return pose_id;
    }

    private bool movePose(int pose_id){
        if (GetRightControllerTrigger())
        {
            //Ray CameraToMouse = Camera.main.ScreenPointToRay(Input.mousePosition);
            Ray CameraToMouse = new Ray(controller.transform.position, controller.transform.forward);
            RaycastHit hitInfo;
            
            if (Physics.Raycast(CameraToMouse, 
                out hitInfo,  Mathf.Infinity, canBeClicked)) {
                edit_goal = hitInfo.point;
                Debug.Log("goal exists");
                return true;
            } else {
                Debug.Log("Goal lost");
            }
        }
        return false;
    }
    
    // Draw line through all pathes
    private void RenderPath(){
        pathRender.positionCount = poses.Count + 1;
        pathRender.SetPosition(0, transform.position);
        for (int i = 0; i < poses.Count; ++i){
            pathRender.SetPosition(i + 1, poses[i]);
        }
    }
}
