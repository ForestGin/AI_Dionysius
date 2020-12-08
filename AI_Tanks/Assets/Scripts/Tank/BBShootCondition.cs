using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.AI;

using Pada1.BBCore;           // Code attributes
using Pada1.BBCore.Tasks;     // TaskStatus
using Pada1.BBCore.Framework;

namespace BBUnity.Conditions
{
    [Condition("MyConditions/BBShootCondition")]
    [Help("")]
    public class BBShootCondition : GOCondition
    {
        
      
        public TankShooting tankShooting;

        public override bool Check()
        {        
            if(tankShooting == null)
                tankShooting = gameObject.GetComponent<TankShooting>();
            
            if (tankShooting.m_EmptyMagazine)
                return false;
            else
                return true;
        }
    }
}
