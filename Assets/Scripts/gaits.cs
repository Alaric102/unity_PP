using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gaits : MonoBehaviour
{
    internal enum gait_type
    {
        trotGait,
        waveGait
    }

    [SerializeField] private gait_type drive;


    [Header("control Variables")]
    public bool walk = false; //1
    
    public bool forward = false;  //2
    public bool backward = false;  //3
    public bool left = false;  //4
    public bool right = false;

    public bool pitch_up = false;  //5
    public bool pitch_down = false;  //6
    public bool roll_up = false;  //7
    public bool roll_down = false;  //8
    public bool yaw_up = false;  //9
    public bool yaw_down = false;  //10

    public bool rotate_left = false;  //11
    public bool rotate_right = false;  //12

    public static bool _walk = false; //1
    public static bool _forward = false;  //2
    public static bool _backward = false;  //3
    public static bool _left = false;  //4
    public static bool _right = false;

    public static bool _pitch_up = false;  //5
    public static bool _pitch_down = false;  //6
    public static bool _roll_up = false;  //7
    public static bool _roll_down = false;  //8
    public static bool _yaw_up = false;  //9
    public static bool _yaw_down = false;  //10

    public static bool _rotate_left = false;  //11
    public static bool _rotate_right = false;  //12

    public  float[] eular_ang = { 0.0f, 0.0f, 0.0f };

    public float stepLen_x = 150;
    public float stepLen_y = 100;

    public int walk_speed = 0;
    public float[] stepHiegth = { 70, 70, 70, 70 };
    public float Body_heigth = 230;


    // initial coordinate
    public float[] init_coordFR = { 0, 0, 9 };
    public float[] init_coordFL = { 0, 0, 9 };
    public float[] init_coordRR = { 0, 0, 9 };
    public float[] init_coordRL = { 0, 0, 9 };

    // step length in x and y direction
    public float[] stepLength_FR = { 15, 0 };
    public float[] stepLength_FL = { 15, 0 };
    public float[] stepLength_RR = { 15, 0 };
    public float[] stepLength_RL = { 15, 0 };

    [Header("other Variables")]
    public static  float[] FR_coordinate = { 0, 0, 0 };
    public static float[] FL_coordinate = { 0, 0, 0 };
    public static float[] RR_coordinate = { 0, 0, 0 };
    public static float[] RL_coordinate = { 0, 0, 0 };

    public float[] startPoint_FR = { 0, 0 };
    public float[] endPoint_FR = { 0, 0 };
    public float[] startPoint_FL = { 0, 0 };
    public float[] endPoint_FL = { 0, 0 };
    public float[] startPoint_RR = { 0, 0 };
    public float[] endPoint_RR = { 0, 0 };
    public float[] startPoint_RL = { 0, 0 };
    public float[] endPoint_RL = { 0, 0 };

    [Header("debug Variables")]
    public float test_i1 = 0;
    public float test_i2 = 0;
    public float test_map = 0;
    public int found = 0;
    //

   

    public static float[] delta_Z = { 0, 0, 0, 0 }; // variables for changes in each leg height {FR, FL, RR, RL}

    public float nextTimeCall;
    public float prevTime;
    public float dT = 0.2f;
    
    private void Start()
    {
        //Body_heigth = InverseKinamtics.Z_min;
        

    }
    
   // Update is called once per frame
   void Update()
    {
        // ========== Change Pitch =================
        if (pitch_up)
        {
            pitch_down = false;
            if (eular_ang[0] >= -10)
            {
                eular_ang[0] -= 0.2f;
            }
        }
        else if (pitch_down)
        {
            pitch_up = false;
            if (eular_ang[0] <= 20)
            {
                eular_ang[0] += 0.2f;
            }

        }

        // ========== Change Roll =================
        if (roll_up)
        {
            roll_down = false;
            if (eular_ang[1] <= 30)
            {
                eular_ang[1] += 0.2f;
            }
        }
        else if (roll_down)
        {
            roll_up = false;
            if (eular_ang[1] >= -30)
            {
                eular_ang[1] -= 0.2f;
            }

        }

        // ========== Change Yaw =================
        if (yaw_up)
        {
    
            yaw_down = false;
            if (eular_ang[2] <= 25)
            {
                eular_ang[2] += 0.2f;
            }
        }
        else if (yaw_down)
        {
    
            yaw_up = false;
            if (eular_ang[2] >= -25)
            {
                eular_ang[2] -= 0.2f;
            }

        }
      



        InverseKinamtics.eular_ang[0] = eular_ang[0];
        InverseKinamtics.eular_ang[1] = eular_ang[1];
        InverseKinamtics.eular_ang[2] = eular_ang[2];


        if (walk)
        {
            _walk = true;
            
            // ========= Forward & backward ==========
            if (forward && !_forward)
            {
                _forward = true;
                _backward = false;
                backward = false;
                stepLength_FR[0] = stepLength_FL[0] = stepLength_RR[0] = stepLength_RL[0] = stepLen_x;
            }
            else if (backward && !_backward)
            {
                _forward = false;
                _backward = true;
                forward = false;
                stepLength_FR[0] = stepLength_FL[0] = stepLength_RR[0] = stepLength_RL[0] = -stepLen_x;

            }
            else if (!forward && !backward)
            {
                _forward = false;
                _backward = false;
                stepLength_FR[0] = stepLength_FL[0] = stepLength_RR[0] = stepLength_RL[0] = 0;
            }

            // =========== Right & Left  ===========
            if (right && !_right)
            {
                _right = true;
                _left = false;
                left = false;
                stepLength_FR[1] = stepLength_FL[1] = stepLength_RR[1] = stepLength_RL[1] = stepLen_y;
            }
            else if (left && !_left)
            {
                _right = false;
                _left = true;
                right = false;
                stepLength_FR[1] = stepLength_FL[1] = stepLength_RR[1] = stepLength_RL[1] = -stepLen_y;

            }
            else if (!right && !left)
            {
                _right = false;
                _left = false;
                stepLength_FR[1] = stepLength_FL[1] = stepLength_RR[1] = stepLength_RL[1] = 0;

            }

            // Run gait algorithm
            Test1(walk);
        }
        else
        {
            stepLength_FR[0] = stepLength_FL[0] = stepLength_RR[0] = stepLength_RL[0] = 0;
            stepLength_FR[1] = stepLength_FL[1] = stepLength_RR[1] = stepLength_RL[1] = 0;
            InverseKinamtics.FR_coordinate[2] = Body_heigth;
            InverseKinamtics.FL_coordinate[2] = Body_heigth;
            InverseKinamtics.RR_coordinate[2] = Body_heigth;
            InverseKinamtics.RL_coordinate[2] = Body_heigth;

        }
        
    }


  //============================================================
    float i = 0;
    public bool stance = false;

    // ------------- JUST FOR TESTING TROT GAIT ALGORITHM-----------------
    public void Test1(bool on)
    {
       
        if (dT < 0.01) dT = 0.01f; //limiting dT
        
        ///*
        // -------- SWING  ------------------
        if (i < walk_speed && !stance ) // when stance = 1
        {
            test_map = map(0, walk_speed, -10, 10, i) ;
            trot_stance_FL(i, on);
            trot_stance_RR(i, on);
            trot_swing_FR(i, on);
            trot_swing_RL(i, on);

            InverseKinamtics.FR_coordinate = FR_coordinate;
            InverseKinamtics.RL_coordinate = RL_coordinate;
            InverseKinamtics.FL_coordinate = FL_coordinate;
            InverseKinamtics.RR_coordinate = RR_coordinate;
            i += 1;
            test_i1 = i;
            prevTime = Time.time;
            
        }
        else if(!stance)
        {
            i = 0;
            stance = !stance; // DELETE COMMENT WHEN UNCOMMENT FUNTION FOR STANCE
        }
        //*/
        
        
        // ------------- STANCE ------------------
         ///*
        if (i < walk_speed && stance)
        {
            test_map = map(0, walk_speed, -10, 10, i);
            trot_stance_FR(i, on);
            trot_stance_RL(i, on);
            trot_swing_FL(i, on);
            trot_swing_RR(i, on);
            InverseKinamtics.FR_coordinate = FR_coordinate;
            InverseKinamtics.RL_coordinate = RL_coordinate;
            InverseKinamtics.FL_coordinate = FL_coordinate;
            InverseKinamtics.RR_coordinate = RR_coordinate;

            i += 1;
            test_i2 = i;
            prevTime = Time.time;
        }
        else if (stance)
        {
            i = 0;
            stance = !stance;
        }
        //*/ 
    }

   

    //====================  TROT GAIT ==============================

    void trot_gait(bool start)
    {
        // WRITE TROTE ALGORITHM
    }
    float[] threshold = { 3.1f, 3.1f, 3.5f };
    public float gaitRun = 0;



    void trot_swing_FR(float _i, bool start)
    {
        
        if (_i == 0)
        {   
            if (FR_coordinate[0] != -stepLength_FR[0] / 2 ) startPoint_FR[0] = FR_coordinate[0];
            else startPoint_FR[0] = -stepLength_FR[0] / 2;
            if (FR_coordinate[1] != stepLength_FR[1] / 2  ) startPoint_FR[1] = FR_coordinate[1];
            else startPoint_FR[1] = stepLength_FR[1] / 2;
            
            
        }
        else
        {
            startPoint_FR[0] = -stepLength_FR[0] / 2;
            startPoint_FR[1] = stepLength_FR[1] / 2;
            endPoint_FR[0] = stepLength_FR[0] / 2;
            endPoint_FR[1] = -stepLength_FR[1] / 2;

        }
        if (!start)
        {
            gaitRun = 0;
            endPoint_FR[0] = init_coordFR[0];
            endPoint_FR[1] = init_coordFR[1];
        }    
        gaitRun = 1;
        if (i >= 1)
        {
            FR_coordinate[0] = init_coordFR[0] + map(0.0f, walk_speed, startPoint_FR[0], endPoint_FR[0], _i); //map(i, 0, walk.spd, -walk.Lx/2, walk.Lx/2);
            FR_coordinate[1] = init_coordFR[1] - map(0.0f, walk_speed, startPoint_FR[1], endPoint_FR[1], _i);
            FR_coordinate[2] = Body_heigth - delta_Z[0] - stepHiegth[0] * Mathf.Sin(Mathf.Deg2Rad * _i * 180 / walk_speed); //    map(i, 0, walk.spd, 0, 180));
        }

        //print(FR_coordinate[0]);

        if (_i >= walk_speed )
        {
            FR_coordinate[0] = endPoint_FR[0];
            FR_coordinate[1] = endPoint_FR[1];

            print("Swing_startP =");
            print(startPoint_FR[0]);
            print(startPoint_FR[1]);
            print("Swing_endP =");
            print(endPoint_FR[0]);
            print(endPoint_FR[1]);
        }
    }

    void trot_stance_FR(float _i, bool start)
    {
        if (_i ==0 )
        {
            if (FR_coordinate[0] != stepLength_FR[0] / 2 ) startPoint_FR[0] = FR_coordinate[0];
            else startPoint_FR[0] = stepLength_FR[0] / 2;
            if (FR_coordinate[1] != -stepLength_FR[1] / 2 ) startPoint_FR[1] = FR_coordinate[1];
            else startPoint_FR[1] = -stepLength_FR[1] / 2;     
        }
        else
        {
            startPoint_FR[0] = stepLength_FR[0] / 2;
            startPoint_FR[1] = -stepLength_FR[1] / 2;
            endPoint_FR[0] = -stepLength_FR[0] / 2;
            endPoint_FR[1] = stepLength_FR[1] / 2;
        }
        if (!start)
        {
            endPoint_FR[0] = init_coordFR[0];
            endPoint_FR[1] = init_coordFR[1];
        } 
        if (i >= 1)
        {
            FR_coordinate[0] = init_coordFR[0] + map(0, walk_speed, startPoint_FR[0], endPoint_FR[0], _i); //map(i, 0, walk.spd, -walk.Lx/2, walk.Lx/2);
            FR_coordinate[1] = init_coordFR[1] - map(0, walk_speed, startPoint_FR[1], endPoint_FR[1], _i);
            FR_coordinate[2] = Body_heigth - delta_Z[0]; // 
        }
        
        if (_i >= walk_speed  )
        {
            FR_coordinate[0] = endPoint_FR[0];
            FR_coordinate[1] = endPoint_FR[1];
            print("Stance_startP =");
            print(startPoint_FR[0]);
            print(startPoint_FR[1]);
            print("Stance_endP =");
            print(endPoint_FR[0]);
            print(endPoint_FR[1]);
        }
    }

    void trot_swing_FL(float _i, bool start)
    {
        if (_i == 0)
        {
            if (FL_coordinate[0] != -stepLength_FL[0] / 2) startPoint_FL[0] = FL_coordinate[0];
            else startPoint_FL[0] = -stepLength_FL[0] / 2;
            if (FL_coordinate[1] != stepLength_FL[1] / 2) startPoint_FL[1] = FL_coordinate[1];
            else startPoint_FL[1] = stepLength_FL[1] / 2;

        }
        else {
            startPoint_FL[0] = -stepLength_FL[0] / 2;
            startPoint_FL[1] = stepLength_FL[1] / 2;
            endPoint_FL[0] = stepLength_FL[0] / 2;
            endPoint_FL[1] = -stepLength_FL[1] / 2;
        }
        if (!start)
        {
            endPoint_FL[0] = init_coordFL[0];
            endPoint_FL[1] = init_coordFL[1];
        }
        

        if (i >= 1)
        {
            FL_coordinate[0] = init_coordFL[0] + map(0, walk_speed, startPoint_FL[0], endPoint_FL[0], _i); //map(i, 0, walk.spd, -walk.Lx/2, walk.Lx/2);
            FL_coordinate[1] = init_coordFL[1] + map(0, walk_speed, startPoint_FL[1], endPoint_FL[1], _i);
            FL_coordinate[2] = Body_heigth - delta_Z[1] - stepHiegth[1] * Mathf.Sin(Mathf.Deg2Rad * _i * 180 / walk_speed);
        }
        
        if (_i >= walk_speed)
        {
            FL_coordinate[0] = endPoint_FL[0];
            FL_coordinate[1] = endPoint_FL[1];
        }
    }

    void trot_stance_FL(float _i, bool start)
    {
        if (_i == 0)
        {
            if (FL_coordinate[0] != stepLength_FL[0] / 2) startPoint_FL[0] = FL_coordinate[0];
            else startPoint_FL[0] = stepLength_FL[0] / 2;
            if (FL_coordinate[1] != -stepLength_FL[1] / 2) startPoint_FL[1] = FL_coordinate[1];
            else startPoint_FL[1] = -stepLength_FL[1] / 2;
        } else
        {
            startPoint_FL[0] = stepLength_FL[0] / 2;
            startPoint_FL[1] = -stepLength_FL[1] / 2;
            endPoint_FL[0] = -stepLength_FL[0] / 2;
            endPoint_FL[1] = stepLength_FL[1] / 2;
        }

        if (!start)
        {
            endPoint_FL[0] = init_coordFL[0];
            endPoint_FL[1] = init_coordFL[1];
        }
       
        if (i >= 1)
        {
            FL_coordinate[0] = init_coordFL[0] + map(0, walk_speed, startPoint_FL[0], endPoint_FL[0], _i); //map(i, 0, walk.spd, -walk.Lx/2, walk.Lx/2);
            FL_coordinate[1] = init_coordFL[1] + map(0, walk_speed, startPoint_FL[1], endPoint_FL[1], _i);
            FL_coordinate[2] = Body_heigth - delta_Z[1]; // 
        }
        
        if (_i >= walk_speed)
        {
            FL_coordinate[0] = endPoint_FL[0];
            FL_coordinate[1] = endPoint_FL[1];
        }
    }

    void trot_swing_RR(float _i, bool start)
    {
        if (_i == 0)
        {
            if (RR_coordinate[0] != -stepLength_RR[0] / 2) startPoint_RR[0] = RR_coordinate[0];
            else startPoint_RR[0] = -stepLength_RR[0] / 2;
            if (RR_coordinate[1] != stepLength_RR[1] / 2) startPoint_RR[1] = RR_coordinate[1];
            else startPoint_RR[1] = stepLength_RR[1] / 2;
        }else
        {
            startPoint_RR[0] = -stepLength_RR[0] / 2;
            startPoint_RR[1] = stepLength_RR[1] / 2;
            endPoint_RR[0] = stepLength_RR[0] / 2;
            endPoint_RR[1] = -stepLength_RR[1] / 2;
        }

        if (!start)
        {
            endPoint_RR[0] = init_coordRR[0];
            endPoint_RR[1] = init_coordRR[1];
        }


        if (i >= 1)
        {
            RR_coordinate[0] = init_coordRR[0] + map(0, walk_speed, startPoint_RR[0], endPoint_RR[0], _i); //map(i, 0, walk.spd, -walk.Lx/2, walk.Lx/2);
            RR_coordinate[1] = init_coordRR[1] - map(0, walk_speed, startPoint_RR[1], endPoint_RR[1], _i);
            RR_coordinate[2] = Body_heigth - delta_Z[2] - stepHiegth[2] * Mathf.Sin(Mathf.Deg2Rad * _i * 180 / walk_speed);
        }

        if (_i >= walk_speed)
        {
            
            RR_coordinate[0] = endPoint_RR[0];
            RR_coordinate[1] = endPoint_RR[1];
        }
    }

    void trot_stance_RR(float _i, bool start)
    {
        if (_i == 0)
        {
            if (RR_coordinate[0] != stepLength_RR[0] / 2) startPoint_RR[0] = RR_coordinate[0];
            else startPoint_RR[0] = stepLength_RR[0] / 2;
            if (RR_coordinate[1] != -stepLength_RR[1] / 2) startPoint_RR[1] = RR_coordinate[1];
            else startPoint_RR[1] = -stepLength_RR[1] / 2;
        } else
        {
            startPoint_RR[0] = stepLength_RR[0] / 2;
            startPoint_RR[1] = -stepLength_RR[1] / 2;
            endPoint_RR[0] = -stepLength_RR[0] / 2;
            endPoint_RR[1] = stepLength_RR[1] / 2;
        }
        

        if (!start)
        {
            endPoint_RR[0] = init_coordRR[0];
            endPoint_RR[1] = init_coordRR[1];
        }
       

        if (i >= 1)
        {
            RR_coordinate[0] = init_coordRR[0] + map(0, walk_speed, startPoint_RR[0], endPoint_RR[0], _i); //map(i, 0, walk.spd, -walk.Lx/2, walk.Lx/2);
            RR_coordinate[1] = init_coordRR[1] - map(0, walk_speed, startPoint_RR[1], endPoint_RR[1], _i);
            RR_coordinate[2] = Body_heigth - delta_Z[2]; // 
        }
        if (_i >= walk_speed )
        {
            RR_coordinate[0] = endPoint_RR[0];
            RR_coordinate[1] = endPoint_RR[1];
        }
    }

    void trot_swing_RL(float _i, bool start)
    {
        if (_i == 0)
        {
            if (RL_coordinate[0] != -stepLength_RL[0] / 2) startPoint_RL[0] = RL_coordinate[0];
            else startPoint_RL[0] = -stepLength_RL[0] / 2;
            if (RL_coordinate[1] != stepLength_RL[1] / 2) startPoint_RL[1] = RL_coordinate[1];
            else startPoint_RL[1] = stepLength_RL[1] / 2;
        }
        else
        {
            startPoint_RL[0] = -stepLength_RL[0] / 2;
            startPoint_RL[1] = stepLength_RL[1] / 2;
            endPoint_RL[0] =  stepLength_RL[0] / 2;
            endPoint_RL[1] = -stepLength_RL[1] / 2;
        }

        if (!start)
        {
            endPoint_RL[0] = init_coordRL[0];
            endPoint_RL[1] = init_coordRL[1];
        }


        if (i >=1)
        {
            RL_coordinate[0] = init_coordRL[0] + map(0, walk_speed, startPoint_RL[0], endPoint_RL[0], _i); //map(i, 0, walk.spd, -walk.Lx/2, walk.Lx/2);
            RL_coordinate[1] = init_coordRL[1] + map(0, walk_speed, startPoint_RL[1], endPoint_RL[1], _i);
            RL_coordinate[2] = Body_heigth - delta_Z[3] - stepHiegth[3] * Mathf.Sin(Mathf.Deg2Rad * _i * 180 / walk_speed);
        }
        

        if (_i >= walk_speed)
        {
            RR_coordinate[0] = endPoint_RR[0];
            RR_coordinate[1] = endPoint_RR[1];
        }
    }

    void trot_stance_RL(float _i, bool start)
    {
        if (_i == 0)
        {
            if (RL_coordinate[0] != stepLength_RL[0] / 2) startPoint_RL[0] = RL_coordinate[0];
            else startPoint_RL[0] = stepLength_RL[0] / 2;
            if (RL_coordinate[1] != -stepLength_RL[1] / 2) startPoint_RL[1] = RL_coordinate[1];
            else startPoint_RL[1] = -stepLength_RL[1] / 2;
        }
        else
        {
            startPoint_RL[0] = stepLength_RL[0] / 2;
            startPoint_RL[1] = -stepLength_RL[1] / 2;
            endPoint_RL[0] = -stepLength_RL[0] / 2;
            endPoint_RL[1] = stepLength_RL[1] / 2;
        }
        if (!start)
        {
            endPoint_RL[0] =init_coordRL[0];
            endPoint_RL[1] = init_coordRL[1];
        }

        if (i >= 1)
        {
            RL_coordinate[0] = init_coordRL[0] + map(0, walk_speed, startPoint_RL[0], endPoint_RL[0], _i); //map(i, 0, walk.spd, -walk.Lx/2, walk.Lx/2);
            RL_coordinate[1] = init_coordRL[1] + map(0, walk_speed, startPoint_RL[1], endPoint_RL[1], _i);
            RL_coordinate[2] = Body_heigth - delta_Z[3];

        }

        if (_i >= walk_speed )
        {
            RR_coordinate[0] = endPoint_RR[0];
            RR_coordinate[1] = endPoint_RR[1];
        }
    }

    //-------------------------------------------
    float map(float st1, float fn1, float st2, float fn2, float value)
    {
        return (1.0f * (value - st1)) / ((fn1 - st1) * 1.0f) * (fn2 - st2) + st2;
    }


}
