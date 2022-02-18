using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InverseKinamtics : MonoBehaviour
{
	public GameObject[] P = new GameObject[4];
	public float[] L = { 0, 0, 0 }; // Link_Length
	public static float Z_min = 0.130f;

	public float[] rob_coordinate = { 0, 0, 0 };


	public static float[] FR_coordinate = { 0, 0, Z_min };
	public static float[] FL_coordinate = { 0, 0, Z_min };
	public static float[] RR_coordinate = { 0, 0, Z_min };
	public static float[] RL_coordinate = { 0, 0, Z_min };
 

	public float[] FR_angle = { 0, 0, 0 };
    public float[] FL_angle = { 0, 0, 0 };
    public float[] RR_angle = { 0, 0, 0 };
    public float[] RL_angle = { 0, 0, 0 };
	public static float[] eular_ang = { 0.0f, 0.0f, 0.0f };


	public float[] FK_results = { 0, 0, 0 };

	public float _SIN = 0;

    private float L1 = 0.3150f; 
    private float L2 = 0.31500f;
	private float L3 = 0.4500f;
	private float _A = 0.6150f;
	private float _B = 0.2280f;
	private float _K = 0.2100f;


	/*private float L1 = 105.0f;
	private float L2 = 150.0f;
	private float L3 = 150.0f;
	private float _A = 175.0f;
	private float _B = 76.0f;
	private float _K = 70.0f;*/

	private float[] max_coordFR = { 0, 0, 0 };
	private float[] min_coordFR = { 0, 0, 0 };

	private double PI = 3.14159265358;

	public float r_min;
	public float r_max;
	public float[] r = { 0, 0, 0, 0 };

	private float pitch = 0.0f;
	private float roll = 0.0f;
	private float yaw = 0.0f;
	public bool[] outOfRange = { false, false, false, false };


	
     

    void Start()
    {
		//gaits.Body_heigth = rob_coordinate[2];
		for (int i = 0; i < 3; i++)
		{
			L[i] = Vector3.Distance(P[i].transform.position, P[i + 1].transform.position);
		}
		update_r();
		
	}
    // Update is called once per frame
    void Update()
    {
		if (FR_coordinate[2] < Z_min) FR_coordinate[2] = Z_min;
		if (FL_coordinate[2] < Z_min) FL_coordinate[2] = Z_min;
		if (RR_coordinate[2] < Z_min) RR_coordinate[2] = Z_min;
		if (RL_coordinate[2] < Z_min) RL_coordinate[2] = Z_min;
		//FR_coordinate[2] = rob_coordinate[2];
		pitch = eular_angles(eular_ang)[0];
		roll = eular_angles(eular_ang)[1];
		yaw = eular_angles(eular_ang)[2];

		IK_FR(FR_coordinate, FR_angle);
		IK_FL(FL_coordinate, FL_angle);
		IK_RR(RR_coordinate, RR_angle);
		IK_RL(RL_coordinate, RL_angle);
		
		joint.FR_angle = FR_angle;
		joint.FL_angle = FL_angle;
		joint.RR_angle = RR_angle;
		joint.RL_angle = RL_angle;
		
		Frwd_kin();

	}


	float[] eular_angles(float[] eularAng)
	{
		
		if (pitch != eularAng[0] || roll != eularAng[1] || yaw != eularAng[2])
		{
			pitch = Mathf.Deg2Rad * eularAng[0];
			roll = Mathf.Deg2Rad * eularAng[1];
			yaw = Mathf.Deg2Rad * eularAng[2];
		}
		float[] EulAng = { pitch, roll, yaw };
		return EulAng;
	}


	// leg: 0 = FR, 1 = FL, 2 = RR, 3 = RL
	/*
	 * Inverse Kinematics:
		* Input: coord[x, y, z] , in mm
		* Output: joint_ang[th1, th2, th3] , in Deg
		 
	*/
	void IK_FR(float[] _coord,   float[] joint_ang)//, int leg)
	{
		int leg = 0;
		
		//__________________________ROTATION ( yaw, pitch, roll)___________________________________________
		float X = 0;
		float Y = 0;
		float Z = 0;

		
		X = _A + _coord[0];
		Y = _B + _coord[1];
		Z = _coord[2];

		float XX = X * (Mathf.Cos(pitch) * Mathf.Cos(yaw)) + Y * (Mathf.Sin(pitch) * Mathf.Sin(roll) * Mathf.Cos(yaw) - Mathf.Sin(yaw) * Mathf.Cos(roll)) + Z * (Mathf.Sin(pitch) * Mathf.Cos(roll) * Mathf.Cos(yaw) + Mathf.Sin(roll) * Mathf.Sin(yaw));
		float YY = X * (Mathf.Sin(yaw) * Mathf.Cos(pitch)) + Y * (Mathf.Sin(pitch) * Mathf.Sin(roll) * Mathf.Sin(yaw) + Mathf.Cos(yaw) * Mathf.Cos(roll)) + Z * (Mathf.Sin(pitch) * Mathf.Cos(roll) * Mathf.Sin(yaw) - Mathf.Sin(roll) * Mathf.Cos(yaw));
		float ZZ = X * (-Mathf.Sin(pitch)) + Y * (Mathf.Sin(roll) * Mathf.Cos(pitch)) + Z * (Mathf.Cos(roll) * Mathf.Cos(pitch));
		float[] coord = { 0, 0, 0 };
		coord[0] = XX - _A;
		coord[1] = YY - _B;
		coord[2] = ZZ;
		
		

		
		//___________________________________________________________________________________________________

		r[leg] = Mathf.Sqrt(Mathf.Pow(coord[0],  2) + Mathf.Pow(coord[1], 2) + Mathf.Pow(coord[2], 2));

		if (r[0] > r_min && r[0] < r_max)//&& r[0] < r_max && r[1] > r_min && r[1] < r_max  && r[2] > r_min && r[2] < r_max && r[3] > r_min && r[3] < r_max)
		{
			outOfRange[0] = false;

			float r1 = Mathf.Sqrt(Mathf.Pow(coord[1], 2.0f) + Mathf.Pow(coord[2], 2.0f));

			float alpha = Mathf.Acos(coord[1] / r1);//  angle b/w y axis and r1
			joint_ang[0] = Mathf.Acos(L[0] / r1) - alpha;   //phi_1-phi_2;

			float a = 2.0f * L[1] * r1 * Mathf.Sin(joint_ang[0] + alpha);
			float b = 2.0f * L[1] * coord[0];
			float c = Mathf.Pow(coord[0], 2.0f) + Mathf.Pow(L[1], 2.0f) - Mathf.Pow(L[2], 2.0f) + Mathf.Pow(r1 * Mathf.Sin(joint_ang[0] + alpha), 2.0f);

			float r2 = Mathf.Sqrt(Mathf.Pow(a, 2.0f) + Mathf.Pow(b, 2.0f));

			float beta = 0;

			if (a / r2 > 1)
			{
				beta = Mathf.Acos(1);
				//print("a / r2 > 1");
			}
			else if (a / r2 < -1)
			{
				beta = Mathf.Acos(-1);
				//print("a / r2 < 0");
			}
			else beta = Mathf.Acos(a / r2);

			if (coord[0] < 0) beta = -beta;

			//print("beta = ");
			//print(beta);


			if (c / r2 > 1)
			{
				joint_ang[1] = beta + Mathf.Asin(1);
			}
			else if (c / r2 < -1)
			{
				joint_ang[1] = beta + Mathf.Asin(-1);
			}
			else
			{
				joint_ang[1] = beta + Mathf.Asin(c / r2);
			}
			//print("Asin(c / r)");
			//print(Mathf.Asin(c / r2));
			//print("th1");
			//print(joint_ang[1]);

			if ((coord[0] + L[1] * Mathf.Cos(joint_ang[1])) / L[2] < -1)
			{
				joint_ang[2] = Mathf.Acos(-1);
			}
			else if ((coord[0] + L[1] * Mathf.Cos(joint_ang[1])) / L[2] > 1)
			{
				joint_ang[2] = Mathf.Acos(1);
			}
			else
			{
				joint_ang[2] = Mathf.Acos((coord[0] + L[1] * Mathf.Cos(joint_ang[1])) / L[2]);
			}

			if (coord[2] < (-L[0] * Mathf.Sin(joint_ang[0]) + L[1] * Mathf.Sin(joint_ang[1]) * Mathf.Cos(joint_ang[0])) && coord[0] > 0)
			{
				joint_ang[2] = -joint_ang[2];
			}
			max_coordFR = coord; // to use when max range achieved
			min_coordFR = coord;
		}
		else if (r[0] > r_min)
		{
			outOfRange[0] = true;
			coord = max_coordFR;
		}
		else
		{
			outOfRange[0] = true;
			coord = min_coordFR;
		}

		for (int i = 0; i < 3; i++)
		{
			joint_ang[i] = Mathf.Rad2Deg * joint_ang[i];

		}

	}

	void IK_FL(float[] _coord,  float[] joint_ang)
	{
		int leg = 1;

		// pitch = DEG_TO_RAD * Robo.Eular[0];
		//__________________________ROTATION ( yaw, pitch, roll)___________________________________________
		float X = 0;
		float Y = 0;
		float Z = 0;
		
		X = _A + _coord[0];
		Y = -_B - _coord[1];
		Z = _coord[2];

		float XX = X * (Mathf.Cos(pitch) * Mathf.Cos(yaw)) + Y * (Mathf.Sin(pitch) * Mathf.Sin(roll) * Mathf.Cos(yaw) - Mathf.Sin(yaw) * Mathf.Cos(roll)) + Z * (Mathf.Sin(pitch) * Mathf.Cos(roll) * Mathf.Cos(yaw) + Mathf.Sin(roll) * Mathf.Sin(yaw));
		float YY = X * (Mathf.Sin(yaw) * Mathf.Cos(pitch)) + Y * (Mathf.Sin(pitch) * Mathf.Sin(roll) * Mathf.Sin(yaw) + Mathf.Cos(yaw) * Mathf.Cos(roll)) + Z * (Mathf.Sin(pitch) * Mathf.Cos(roll) * Mathf.Sin(yaw) - Mathf.Sin(roll) * Mathf.Cos(yaw));
		float ZZ = X * (-Mathf.Sin(pitch)) + Y * (Mathf.Sin(roll) * Mathf.Cos(pitch)) + Z * (Mathf.Cos(roll) * Mathf.Cos(pitch));
		
		float[] coord = { 0, 0, 0 };
		coord[0] = XX - _A;
		coord[1] = -YY - _B;
		coord[2] = ZZ;
		//___________________________________________________________________________________________________


		float r1 = Mathf.Sqrt(Mathf.Pow(coord[1], 2.0f) + Mathf.Pow(coord[2], 2.0f));

		float alpha = Mathf.Acos(coord[1] / r1);//  angle b/w y axis and r1
		joint_ang[0] = Mathf.Acos(L[0] / r1) - alpha;   //phi_1-phi_2;

		float a = 2.0f * L[1] * r1 * Mathf.Sin(joint_ang[0] + alpha);
		float b = 2.0f * L[1] * coord[0];
		float c = Mathf.Pow(coord[0], 2.0f) + Mathf.Pow(L[1], 2.0f) - Mathf.Pow(L[2], 2.0f) + Mathf.Pow(r1 * Mathf.Sin(joint_ang[0] + alpha), 2.0f);

		float r2 = Mathf.Sqrt(Mathf.Pow(a, 2.0f) + Mathf.Pow(b, 2.0f));

		float beta = 0;

		if (a / r2 > 1)
		{
			beta = Mathf.Acos(1);
			//print("a / r2 > 1");
		}
		else if (a / r2 < -1)
		{
			beta = Mathf.Acos(-1);
			//print("a / r2 < 0");
		}
		else beta = Mathf.Acos(a / r2);

		if (coord[0] < 0) beta = -beta;

		//print("beta = ");
		//print(beta);


		if (c / r2 > 1)
		{
			joint_ang[1] = beta + Mathf.Asin(1);
		}
		else if (c / r2 < -1)
		{
			joint_ang[1] = beta + Mathf.Asin(-1);
		}
		else
		{
			joint_ang[1] = beta + Mathf.Asin(c / r2);
		}
		//print("Asin(c / r)");
		//print(Mathf.Asin(c / r2));
		//print("th1");
		//print(joint_ang[1]);

		if ((coord[0] + L[1] * Mathf.Cos(joint_ang[1])) / L[2] < -1)
		{
			joint_ang[2] = Mathf.Acos(-1);
		}
		else if ((coord[0] + L[1] * Mathf.Cos(joint_ang[1])) / L[2] > 1)
		{
			joint_ang[2] = Mathf.Acos(1);
		}
		else
		{
			joint_ang[2] = Mathf.Acos((coord[0] + L[1] * Mathf.Cos(joint_ang[1])) / L[2]);
		}

		if (coord[2] < (-L[0] * Mathf.Sin(joint_ang[0]) + L[1] * Mathf.Sin(joint_ang[1]) * Mathf.Cos(joint_ang[0])) && coord[0] > 0)
		{
			//joint_ang[2] = -joint_ang[2];
		}


		for (int i = 0; i < 3; i++)
		{
			joint_ang[i] = Mathf.Rad2Deg * joint_ang[i];

		}
	}

	void IK_RR(float[] _coord,  float[] joint_ang)
	{
		int leg = 2;

		// pitch = DEG_TO_RAD * Robo.Eular[0];
		//__________________________ROTATION ( yaw, pitch, roll)___________________________________________
		float X = 0;
		float Y = 0;
		float Z = 0;

		X = -_A + _coord[0];
		Y = _B + _coord[1];
		Z = _coord[2];

		float XX = X * (Mathf.Cos(pitch) * Mathf.Cos(yaw)) + Y * (Mathf.Sin(pitch) * Mathf.Sin(roll) * Mathf.Cos(yaw) - Mathf.Sin(yaw) * Mathf.Cos(roll)) + Z * (Mathf.Sin(pitch) * Mathf.Cos(roll) * Mathf.Cos(yaw) + Mathf.Sin(roll) * Mathf.Sin(yaw));
		float YY = X * (Mathf.Sin(yaw) * Mathf.Cos(pitch)) + Y * (Mathf.Sin(pitch) * Mathf.Sin(roll) * Mathf.Sin(yaw) + Mathf.Cos(yaw) * Mathf.Cos(roll)) + Z * (Mathf.Sin(pitch) * Mathf.Cos(roll) * Mathf.Sin(yaw) - Mathf.Sin(roll) * Mathf.Cos(yaw));
		float ZZ = X * (-Mathf.Sin(pitch)) + Y * (Mathf.Sin(roll) * Mathf.Cos(pitch)) + Z * (Mathf.Cos(roll) * Mathf.Cos(pitch));

		float[] coord = { 0, 0, 0 };
		coord[0] = XX + _A;
		coord[1] = YY - _B;
		coord[2] = ZZ;
		//___________________________________________________________________________________________________


		float r1 = Mathf.Sqrt(Mathf.Pow(coord[1], 2.0f) + Mathf.Pow(coord[2], 2.0f));

		float alpha = Mathf.Acos(coord[1] / r1);//  angle b/w y axis and r1
		joint_ang[0] = Mathf.Acos(L[0] / r1) - alpha;   //phi_1-phi_2;

		float a = 2.0f * L[1] * r1 * Mathf.Sin(joint_ang[0] + alpha);
		float b = 2.0f * L[1] * coord[0];
		float c = Mathf.Pow(coord[0], 2.0f) + Mathf.Pow(L[1], 2.0f) - Mathf.Pow(L[2], 2.0f) + Mathf.Pow(r1 * Mathf.Sin(joint_ang[0] + alpha), 2.0f);

		float r2 = Mathf.Sqrt(Mathf.Pow(a, 2.0f) + Mathf.Pow(b, 2.0f));

		float beta = 0;

		if (a / r2 > 1)
		{
			beta = Mathf.Acos(1);
			//print("a / r2 > 1");
		}
		else if (a / r2 < -1)
		{
			beta = Mathf.Acos(-1);
			//print("a / r2 < 0");
		}
		else beta = Mathf.Acos(a / r2);

		if (coord[0] < 0) beta = -beta;

		//print("beta = ");
		//print(beta);


		if (c / r2 > 1)
		{
			joint_ang[1] = beta + Mathf.Asin(1);
		}
		else if (c / r2 < -1)
		{
			joint_ang[1] = beta + Mathf.Asin(-1);
		}
		else
		{
			joint_ang[1] = beta + Mathf.Asin(c / r2);
		}
		//print("Asin(c / r)");
		//print(Mathf.Asin(c / r2));
		//print("th1");
		//print(joint_ang[1]);

		if ((coord[0] + L[1] * Mathf.Cos(joint_ang[1])) / L[2] < -1)
		{
			joint_ang[2] = Mathf.Acos(-1);
		}
		else if ((coord[0] + L[1] * Mathf.Cos(joint_ang[1])) / L[2] > 1)
		{
			joint_ang[2] = Mathf.Acos(1);
		}
		else
		{
			joint_ang[2] = Mathf.Acos((coord[0] + L[1] * Mathf.Cos(joint_ang[1])) / L[2]);
		}

		if (coord[2] < (-L[0] * Mathf.Sin(joint_ang[0]) + L[1] * Mathf.Sin(joint_ang[1]) * Mathf.Cos(joint_ang[0])) && coord[0] > 0)
		{
			//joint_ang[2] = -joint_ang[2];
		}


		for (int i = 0; i < 3; i++)
		{
			joint_ang[i] = Mathf.Rad2Deg * joint_ang[i];

		}
	}

	void IK_RL(float[] _coord,  float[] joint_ang)
	{
		int leg = 3;

		// pitch = DEG_TO_RAD * Robo.Eular[0];
		//__________________________ROTATION ( yaw, pitch, roll)___________________________________________
		float X = 0;
		float Y = 0;
		float Z = 0;

		X = -_A + _coord[0];
		Y = -_B - _coord[1];
		Z = _coord[2];

	
		float XX = X * (Mathf.Cos(pitch) * Mathf.Cos(yaw)) + Y * (Mathf.Sin(pitch) * Mathf.Sin(roll) * Mathf.Cos(yaw) - Mathf.Sin(yaw) * Mathf.Cos(roll)) + Z * (Mathf.Sin(pitch) * Mathf.Cos(roll) * Mathf.Cos(yaw) + Mathf.Sin(roll) * Mathf.Sin(yaw));
		float YY = X * (Mathf.Sin(yaw) * Mathf.Cos(pitch)) + Y * (Mathf.Sin(pitch) * Mathf.Sin(roll) * Mathf.Sin(yaw) + Mathf.Cos(yaw) * Mathf.Cos(roll)) + Z * (Mathf.Sin(pitch) * Mathf.Cos(roll) * Mathf.Sin(yaw) - Mathf.Sin(roll) * Mathf.Cos(yaw));
		float ZZ = X * (-Mathf.Sin(pitch)) + Y * (Mathf.Sin(roll) * Mathf.Cos(pitch)) + Z * (Mathf.Cos(roll) * Mathf.Cos(pitch));

		float[] coord = { 0, 0, 0 };
		coord[0] = XX + _A;
		coord[1] = -YY - _B;
		coord[2] = ZZ;
		//___________________________________________________________________________________________________


		float r1 = Mathf.Sqrt(Mathf.Pow(coord[1], 2.0f) + Mathf.Pow(coord[2], 2.0f));

		float alpha = Mathf.Acos(coord[1] / r1);//  angle b/w y axis and r1
		joint_ang[0] = Mathf.Acos(L[0] / r1) - alpha;   //phi_1-phi_2;

		float a = 2.0f * L[1] * r1 * Mathf.Sin(joint_ang[0] + alpha);
		float b = 2.0f * L[1] * coord[0];
		float c = Mathf.Pow(coord[0], 2.0f) + Mathf.Pow(L[1], 2.0f) - Mathf.Pow(L[2], 2.0f) + Mathf.Pow(r1 * Mathf.Sin(joint_ang[0] + alpha), 2.0f);

		float r2 = Mathf.Sqrt(Mathf.Pow(a, 2.0f) + Mathf.Pow(b, 2.0f));

		float beta = 0;

		if (a / r2 > 1)
		{
			beta = Mathf.Acos(1);
			//print("a / r2 > 1");
		}
		else if (a / r2 < -1)
		{
			beta = Mathf.Acos(-1);
			//print("a / r2 < 0");
		}
		else beta = Mathf.Acos(a / r2);

		if (coord[0] < 0) beta = -beta;

		//print("beta = ");
		//print(beta);


		if (c / r2 > 1)
		{
			joint_ang[1] = beta + Mathf.Asin(1);
		}
		else if (c / r2 < -1)
		{
			joint_ang[1] = beta + Mathf.Asin(-1);
		}
		else
		{
			joint_ang[1] = beta + Mathf.Asin(c / r2);
		}
		//print("Asin(c / r)");
		//print(Mathf.Asin(c / r2));
		//print("th1");
		//print(joint_ang[1]);

		if ((coord[0] + L[1] * Mathf.Cos(joint_ang[1])) / L[2] < -1)
		{
			joint_ang[2] = Mathf.Acos(-1);
		}
		else if ((coord[0] + L[1] * Mathf.Cos(joint_ang[1])) / L[2] > 1)
		{
			joint_ang[2] = Mathf.Acos(1);
		}
		else
		{
			joint_ang[2] = Mathf.Acos((coord[0] + L[1] * Mathf.Cos(joint_ang[1])) / L[2]);
		}

		if (coord[2] < (-L[0] * Mathf.Sin(joint_ang[0]) + L[1] * Mathf.Sin(joint_ang[1]) * Mathf.Cos(joint_ang[0])) && coord[0] > 0)
		{
			//joint_ang[2] = -joint_ang[2];
		}


		for (int i = 0; i < 3; i++)
		{
			joint_ang[i] = Mathf.Rad2Deg * joint_ang[i];

		}
	}
	void update_r()
    {
        r_min = Mathf.Sqrt(Mathf.Pow(L1, 2.0f) + Mathf.Pow(_K, 2.0f)); //minimum spherical radius between bottom of foot and fixed point(the point 
                                                                        // where hip axis and upper leg axis intersect)
        r_max = Mathf.Sqrt(Mathf.Pow(L1, 2.0f) + Mathf.Pow(L2, 2.0f) + Mathf.Pow(L3, 2) - 2 * L2 * L3 * Mathf.Cos(Mathf.Deg2Rad * joint.th_max[2]));
    }

	void Frwd_kin()
	{
		FK_results[0] = -L[1] * Mathf.Cos(joint.FR_angle[1]* Mathf.Deg2Rad) + L[2] * Mathf.Cos(joint.FR_angle[2] * Mathf.Deg2Rad);
		FK_results[1] = L[0] * Mathf.Cos(joint.FR_angle[0] * Mathf.Deg2Rad) + ( L[1] * Mathf.Sin(joint.FR_angle[1] * Mathf.Deg2Rad) + L[2] * Mathf.Sin(joint.FR_angle[2] * Mathf.Deg2Rad)) * Mathf.Sin(joint.FR_angle[0] * Mathf.Deg2Rad);
		FK_results[2] = -L[0] * Mathf.Sin(joint.FR_angle[0] * Mathf.Deg2Rad) + ( L[1] * Mathf.Sin(joint.FR_angle[1] * Mathf.Deg2Rad) + L[2] * Mathf.Sin(joint.FR_angle[2] * Mathf.Deg2Rad)) * Mathf.Cos(joint.FR_angle[0] * Mathf.Deg2Rad);
		
	}
}  
