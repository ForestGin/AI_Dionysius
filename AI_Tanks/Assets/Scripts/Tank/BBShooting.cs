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


    [Action("MyActions/BBShooting")]
    [Help("Shooting done with BB")]
    public class BBShooting : GOAction
    {
        public UnityEngine.AI.NavMeshAgent Tank;
        public UnityEngine.AI.NavMeshPath path;

        public GameManager gameManager;
        public TankShooting tankShooting;

        public override void OnStart()
        {
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            tankShooting= gameObject.GetComponent<TankShooting>();

            

            base.OnStart();
        }

        public override TaskStatus OnUpdate()
        {
            //Turret pointing at closest tank
            tankShooting.m_ClosestTankPosition = tankShooting.GetClosestTankAlivePosition();
            tankShooting.m_ClosestTankPosition.y = 1.2f;

            tankShooting.m_Turret.transform.LookAt(tankShooting.m_ClosestTankPosition);

            //Fire when enemy is in range
            tankShooting.m_ClosestTankDistance = Vector3.Distance(tankShooting.m_ClosestTankPosition, tankShooting.m_FireTransform.position);

            tankShooting.m_MaxShootingRange = tankShooting.CalculateShootingRange(tankShooting.m_MaxShootingRangeAngle, tankShooting.m_InitialVelocity, tankShooting.m_InitialHeight);

            if (tankShooting.m_ClosestTankDistance < tankShooting.m_MaxShootingRange)
            {
                tankShooting.m_ShootingAngle = tankShooting.CalculateShootingAngle(tankShooting.m_ClosestTankDistance, 0, tankShooting.m_InitialVelocity, tankShooting.m_InitialHeight);

                if (!float.IsNaN(tankShooting.m_ShootingAngle))
                {
                    tankShooting.m_Turret.transform.Rotate(-tankShooting.m_ShootingAngle, 0, 0);
                }

                //Fire Delay
                if (tankShooting.m_ShootingTimer < Time.time && !float.IsNaN(tankShooting.m_ShootingAngle))
                {
                    //Fire shell
                    tankShooting.Fire();
                    tankShooting.m_ShootDelay = true;
                    tankShooting.m_ShootingTimer = Time.time + tankShooting.m_RateOfFire;
                }

                if (!float.IsNaN(tankShooting.m_ShootingAngle))
                {
                    tankShooting.m_Turret.transform.Rotate(tankShooting.m_ShootingAngleOffset, 0, 0);
                }
            }
            else
            {
                tankShooting.m_Turret.transform.LookAt(tankShooting.m_ClosestTankPosition);
            }

            return TaskStatus.RUNNING;

        } // OnUpdate


    }  

}

