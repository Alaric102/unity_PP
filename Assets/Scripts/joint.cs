using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class joint : MonoBehaviour
{
    // Get joints
    public GameObject[] Joint_FR = new GameObject[3];
    public GameObject[] Joint_FL = new GameObject[3];
    public GameObject[] Joint_RR = new GameObject[3];
    public GameObject[] Joint_RL = new GameObject[3];

    // control variables
    public static float[] FR_angle = { 0, 0, 0 };
    public static float[] FL_angle = { 0, 0, 0 };
    public static float[] RR_angle = { 0, 0, 0 };
    public static float[] RL_angle = { 0, 0, 0 };

    // other variables
    public float angBW23 = 0;
    float delta = 0.1f;
      
    private InputManager IM;

    public static int[] th_min = {-90, -50, 20};
    public static int[] th_max = { 90, 180, 140 };

    private float[] th_FR = { 0, 0, 0 };
    private float[] th_FL = { 0, 0, 0 };
    private float[] th_RR = { 0, 0, 0 };
    private float[] th_RL = { 0, 0, 0 };


    public void Rotate()
    {
        
        //for (int i = 0; i < 4; i++)
        //{
        if(IM.horizontal > 0)
        {
            //target_angle0 += 1; // deg
        }
        else if(IM.horizontal < 0)
        {
            //target_angle0 -= 1; // deg
        }

        

        //}
    }

    void Update()
    {    
        //IM = GetComponent<InputManager>();
        // th1 lower limit
        if (FR_angle[0] < th_min[0]) FR_angle[0] = th_min[0];
        if (FL_angle[0] < th_min[0]) FL_angle[0] = th_min[0];
        if (RR_angle[0] < th_min[0]) RR_angle[0] = th_min[0];
        if (RL_angle[0] < th_min[0]) RL_angle[0] = th_min[0];
        // th1 higher limit
        if (FR_angle[0] > th_max[0]) FR_angle[0] = th_max[0];
        if (FL_angle[0] > th_max[0]) FL_angle[0] = th_max[0];
        if (RR_angle[0] > th_max[0]) RR_angle[0] = th_max[0];
        if (RL_angle[0] > th_max[0]) RL_angle[0] = th_max[0];

        // th2 lower limit
        if (FR_angle[1] < th_min[1]) FR_angle[1] = th_min[1];
        if (FL_angle[1] < th_min[1]) FL_angle[1] = th_min[1];
        if (RR_angle[1] < th_min[1]) RR_angle[1] = th_min[1];
        if (RL_angle[1] < th_min[1]) RL_angle[1] = th_min[1];
        // th2 higher limit
        if (FR_angle[1] > th_max[1]) FR_angle[1] = th_max[1];
        if (FL_angle[1] > th_max[1]) FL_angle[1] = th_max[1];
        if (RR_angle[1] > th_max[1]) RR_angle[1] = th_max[1];
        if (RL_angle[1] > th_max[1]) RL_angle[1] = th_max[1];

        // th3 lower limit
        if (FR_angle[1] + FR_angle[2] < th_min[2]) FR_angle[2] = th_min[2] - FR_angle[1];
        if (FL_angle[1] + FL_angle[2] < th_min[2]) FL_angle[2] = th_min[2] - FL_angle[1];
        if (RR_angle[1] + RR_angle[2] < th_min[2]) RR_angle[2] = th_min[2] - RR_angle[1];
        if (RL_angle[1] + RL_angle[2] < th_min[2]) RL_angle[2] = th_min[2] - RL_angle[1];
        // th3 higher limit
        if (FR_angle[1] + FR_angle[2] > th_max[2]) FR_angle[2] = th_max[2] - FR_angle[1];
        if (FL_angle[1] + FL_angle[2] > th_max[2]) FL_angle[2] = th_max[2] - FL_angle[1];
        if (RR_angle[1] + RR_angle[2] > th_max[2]) RR_angle[2] = th_max[2] - RR_angle[1];
        if (RL_angle[1] + RL_angle[2] > th_max[2]) RL_angle[2] = th_max[2] - RL_angle[1];
        angBW23 = FL_angle[1] + FL_angle[2];

        
        th_FR[0] = FR_angle[0];     th_FR[1] = 38 - FR_angle[1];     th_FR[2] = -97 + FR_angle[1]+ FR_angle[2]; 
        th_FL[0] = -FL_angle[0];     th_FL[1] = 38 - FL_angle[1];     th_FL[2] = -97 + FL_angle[1] + FL_angle[2];
        th_RR[0] = RR_angle[0];     th_RR[1] = 38 - RR_angle[1];     th_RR[2] = -97 + RR_angle[1] + RR_angle[2];
        th_RL[0] = -RL_angle[0];     th_RL[1] = 38 - RL_angle[1];     th_RL[2] = -97 + RL_angle[1] + RL_angle[2];

        
        Joint_FR[0].transform.localRotation = Quaternion.Euler(new Vector3(th_FR[0],       0,          0));
        Joint_FR[1].transform.localRotation = Quaternion.Euler(new Vector3(0,       th_FR[1],          0));
        Joint_FR[2].transform.localRotation = Quaternion.Euler(new Vector3(0,       th_FR[2],          0));

        Joint_FL[0].transform.localRotation = Quaternion.Euler(new Vector3(th_FL[0],       0,          0));
        Joint_FL[1].transform.localRotation = Quaternion.Euler(new Vector3(0,       th_FL[1],          0));
        Joint_FL[2].transform.localRotation = Quaternion.Euler(new Vector3(0,       th_FL[2],          0));

        Joint_RR[0].transform.localRotation = Quaternion.Euler(new Vector3(th_RR[0],        0,         0));
        Joint_RR[1].transform.localRotation = Quaternion.Euler(new Vector3(0,        th_RR[1],         0));
        Joint_RR[2].transform.localRotation = Quaternion.Euler(new Vector3(0,        th_RR[2],         0));

        Joint_RL[0].transform.localRotation = Quaternion.Euler(new Vector3(th_RL[0],       0,          0));
        Joint_RL[1].transform.localRotation = Quaternion.Euler(new Vector3(0,       th_RL[1],          0));
        Joint_RL[2].transform.localRotation = Quaternion.Euler(new Vector3(0,       th_RL[2],          0));


    }
}
