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


    [Action("MyActions/BBPatrol")]
    [Help("Patrol done with BB")]
    public class BBPatrol : GOAction
    {
        

        public UnityEngine.AI.NavMeshAgent Tank;
        public UnityEngine.AI.NavMeshPath path;

        private GameObject[] pointChildren;
        private GameObject Points;
        public int destPoint = -1;
        public Image m_ImageTarget;

        public LineRenderer trailRenderer;
        public bool debug = false;
        public int counter = 0;

        public float breakforce = 0.25f;
        public float speed = 3.5f;



        public override void OnStart()
        {
            path = new NavMeshPath();
            Tank = gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();//navagent
                                                                          


            trailRenderer = gameObject.GetComponent<LineRenderer>();
           

            Points = GameObject.Find("Points");
            pointChildren = new GameObject[Points.transform.childCount];

            for (int i = 0; i < Points.transform.childCount; i++)
            {
                pointChildren[i] = Points.transform.GetChild(i).gameObject;
            }


            Patrol();


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

            if (!Tank.pathPending && Tank.remainingDistance <= 3f)
                Tank.speed -= breakforce;

            if (!Tank.pathPending && Tank.remainingDistance <= 1f)
            {
                Patrol();
                Tank.speed = speed;
            }

            return TaskStatus.RUNNING;

        } // OnUpdate

        public void Patrol()
        {

            if (pointChildren.Length == 0)
                return;

            if (destPoint == -1)
            {
                Tank.destination = ClosestPatrolPoint();
            }
            else
            {
                Tank.destination = pointChildren[destPoint].transform.position;
                destPoint = (destPoint + 1) % pointChildren.Length;
            }

            Tank.angularSpeed = 200f;
            //transform.LookAt(Tank.destination);

            Debug.Log(pointChildren[2].transform.position.y);
            Debug.Log("MEMBERS:" + pointChildren.Length);
        }


        public Vector3 ClosestPatrolPoint()
        {
            float dist = -1;
            float mindist = 0;

            Vector3 closest = Vector3.zero;

            for (int i = 0; i < pointChildren.Length; i++)
            {

                //First iteration
                if (dist == -1)
                {
                    mindist = dist = Vector3.Distance(pointChildren[i].transform.position, gameObject.transform.position);
                    closest = pointChildren[i].transform.position;
                    destPoint = i;
                }
                else
                {
                    dist = Vector3.Distance(pointChildren[i].transform.position, gameObject.transform.position);

                    if (dist < mindist)
                    {
                        mindist = dist;
                        closest = pointChildren[i].transform.position;
                        destPoint = i;
                    }
                }
            }

            return closest;
        }

    }

}

