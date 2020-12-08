using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.AI;

using Pada1.BBCore;           // Code attributes
using Pada1.BBCore.Tasks;     // TaskStatus
using Pada1.BBCore.Framework;

namespace BBUnity.Actions
{

   
    [Action("MyActions/BBWander")]
    [Help("Wander done with BB")]
    public class BBWander : GOAction
    {
        //public TankMovement tankmovement;
        //public GameObject vehicle;

        //Wander
        public Vector3 wayPoint = new Vector3(0.0f, 0.0f, 0.0f);
        public float Range = 20f;
        public UnityEngine.AI.NavMeshAgent Tank;
        public UnityEngine.AI.NavMeshPath path;
        public bool walkable = true;

        //Wander Frontiers
        public Transform TopFrontier;
        public Transform BotFrontier;
        public Transform LeftFrontier;
        public Transform RightFrontier;

        public LineRenderer trailRenderer;
        public bool debug = false;
        public int counter = 0;

        public float breakforce = 0.25f;
        public float speed = 3.5f;

       

        public override void OnStart()
        {
            
            path = new NavMeshPath();
            Tank = gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();//navagent
            //path = Tank.path;//nav path
            
            TopFrontier = GameObject.Find("TopFrontier").GetComponent<Transform>();
            BotFrontier = GameObject.Find("BotFrontier").GetComponent<Transform>();
            LeftFrontier = GameObject.Find("LeftFrontier").GetComponent<Transform>();
            RightFrontier = GameObject.Find("RightFrontier").GetComponent<Transform>();

            trailRenderer = gameObject.GetComponent<LineRenderer>();
            //tankmovement = gameObject.GetComponent<TankMovement>();

            Wander();

            base.OnStart();
        }

        public override TaskStatus OnUpdate()
        {

            //path debug
            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.red);

            }

            if (Input.GetKeyDown(KeyCode.P))
            {

                if (debug)
                {
                    debug = false;
                }
                else
                    debug = true;
            }


            if (Tank.hasPath && debug)
            {
                trailRenderer.positionCount = Tank.path.corners.Length;
                trailRenderer.SetPositions(Tank.path.corners);
                trailRenderer.enabled = true;
            }
            else
            {
                trailRenderer.enabled = false;
            }

            if ((gameObject.transform.position - Tank.destination).magnitude <= 3f && Tank.speed > 1f)
            {
                Tank.speed -= breakforce;
            }

            if ((gameObject.transform.position - Tank.destination).magnitude <= Tank.stoppingDistance)
            {
                Tank.speed = speed;
                Wander();
            }

            return TaskStatus.RUNNING;

        } // OnUpdate

        public void Wander()
        {
            Tank.angularSpeed = 200f;
            wayPoint.x = Random.Range(gameObject.transform.position.x - Range, gameObject.transform.position.x + Range);
            wayPoint.z = Random.Range(gameObject.transform.position.z - Range, gameObject.transform.position.z + Range);
            //transform.LookAt(wayPoint);

            if (wayPoint.x > LeftFrontier.position.x && wayPoint.x < RightFrontier.position.x && wayPoint.z < TopFrontier.position.z && wayPoint.z > BotFrontier.position.z)
            {
                Tank.destination = wayPoint;
                walkable = Tank.CalculatePath(Tank.destination, path);//returns true if path is find
            }
            else if (wayPoint.x < LeftFrontier.position.x && wayPoint.x > RightFrontier.position.x && wayPoint.z > TopFrontier.position.z && wayPoint.z < BotFrontier.position.z)
            {
                wayPoint.x = Random.Range(gameObject.transform.position.x - Range, gameObject.transform.position.x + Range);
                wayPoint.z = Random.Range(gameObject.transform.position.z - Range, gameObject.transform.position.z + Range);
                //transform.LookAt(wayPoint);

                Tank.destination = wayPoint;
                walkable = Tank.CalculatePath(Tank.destination, path);//repath the next waypoint
            }

            trailRenderer.SetPositions(path.corners);

            Debug.Log("SUPER" + walkable);
            Debug.Log("SUPER" + path.status);
            Debug.Log("SUPER" + wayPoint);

        }

    } 

}
