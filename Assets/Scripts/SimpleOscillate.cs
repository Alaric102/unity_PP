using UnityEngine;
using System.Collections;

public class SimpleOscillate : MonoBehaviour
{
    public GameObject Ball, Box, Chair, BaseStation;
    //public float c = 0.2f;

    //private float XBall, YBall, ZBall, XBox, YBox, ZBox, XChair, YChair, ZChair, XBaseStation, YBaseStation, ZBaseStation;


    void Start()
    {
        Ball.SetActive(false);
        Box.SetActive(false);
        Chair.SetActive(false);
        BaseStation.SetActive(false);

        //XBall = Ball.transform.position.x;
        //YBall = Ball.transform.position.y;
        //ZBall = Ball.transform.position.z;

        //XBox = Box.transform.position.x;
        //YBox = Box.transform.position.y;
        //ZBox = Box.transform.position.z;
        
        //XChair = Chair.transform.position.x;
        //YChair = Chair.transform.position.y;
        //ZChair = Chair.transform.position.z;

        //XBaseStation = BaseStation.transform.position.x;
        //YBaseStation = BaseStation.transform.position.y;
        //ZBaseStation = BaseStation.transform.position.z;

    }

    void FixedUpdate()
    {
        if (Input.GetKey("1"))
        {
            Ball.SetActive(true);
        }
        else if (Input.GetKey("2"))
        {
            Box.SetActive(true);
        }
        else if (Input.GetKey("3"))
        {
            Chair.SetActive(true);
        }
        else if (Input.GetKey("4")) 
        {
            BaseStation.SetActive(true);
        }


        //System.Random random = new System.Random();

        //float deltaXBall = (float)(random.NextDouble() * 2 * c - c);
        //float deltaZBall = (float)(random.NextDouble() * 2 * c - c);            

        //float deltaXBox = (float)(random.NextDouble() * 2 * c - c);
        //float deltaZBox = (float)(random.NextDouble() * 2 * c - c);

        //float deltaXChair = (float)(random.NextDouble() * 2 * c - c);
        //float deltaZChair = (float)(random.NextDouble() * 2 * c - c);

        //float deltaXBaseStation = (float)(random.NextDouble() * 2 * c - c);
        //float deltaZBaseStation = (float)(random.NextDouble() * 2 * c - c);

        //Ball.transform.position = new Vector3(XBall + deltaXBall, YBall, ZBall + deltaZBall);
        //Box.transform.position = new Vector3(XBox + deltaXBox, YBox, ZBox + deltaZBox);
        //Chair.transform.position = new Vector3(XChair + deltaXChair, YChair, ZChair + deltaZChair);
        //BaseStation.transform.position = new Vector3(XBaseStation + deltaXBaseStation, YBaseStation, ZBaseStation + deltaZBaseStation);
    }

}