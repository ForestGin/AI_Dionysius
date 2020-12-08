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


    [Action("MyActions/BBRamps")]
    [Help("Ramp movement done with BB")]
    public class BBRamps : GOAction
    {
        public UnityEngine.AI.NavMeshAgent Tank;
        public UnityEngine.AI.NavMeshPath path;

        public GameObject[] rampChildren;
        public GameObject Ramp;
        public TankMovement tankManager;
        public TankShooting tankShooting;

        public override void OnStart()
        {
            path = new NavMeshPath();
            Tank = gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();//navagent

            tankManager = gameObject.GetComponent<TankMovement>();

            Ramp = GameObject.Find("RampPoints");
            rampChildren = new GameObject[Ramp.transform.childCount];

            tankShooting = gameObject.GetComponent<TankShooting>();

            for (int i = 0; i < Ramp.transform.childCount; i++)
            {
                rampChildren[i] = Ramp.transform.GetChild(i).gameObject;
            }

            base.OnStart();
        }

        public override TaskStatus OnUpdate()
        {
            Tank.speed = 3.5f;

            if (tankManager.m_TeamNumber == 1)
                Tank.destination = rampChildren[1].transform.position;
            else if (tankManager.m_TeamNumber == 2)
                Tank.destination = rampChildren[0].transform.position;//red
            else if (tankManager.m_TeamNumber == 3)
                Tank.destination = rampChildren[2].transform.position;//green
            else if (tankManager.m_TeamNumber == 4)
                Tank.destination = rampChildren[3].transform.position;//yellow

            if (Tank.destination.x + 3 >= gameObject.transform.position.x && Tank.destination.x - 3 <= gameObject.transform.position.x
                && Tank.destination.z + 3 >= gameObject.transform.position.z && Tank.destination.z - 3 <= gameObject.transform.position.z)
            {
                tankShooting.m_CurrentMagazine = 10;
                tankShooting.m_EmptyMagazine = false;
                return TaskStatus.COMPLETED;
            }
            else
                return TaskStatus.RUNNING;

        } // OnUpdate


    }

}
