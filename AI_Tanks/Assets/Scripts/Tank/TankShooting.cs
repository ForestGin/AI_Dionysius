using UnityEngine;
using UnityEngine.UI;

public class TankShooting : MonoBehaviour
{
    public int m_PlayerNumber = 1;       
    public Rigidbody m_Shell;            
    public Transform m_FireTransform;    
    //public Slider m_AimSlider;           
    public AudioSource m_ShootingAudio;  
    //public AudioClip m_ChargingClip;     
    public AudioClip m_FireClip;         
    //public float m_MinLaunchForce = 15f; //this should be constant                          
    //public float m_MaxLaunchForce = 30f; //and be controlled via parabola
    //public float m_MaxChargeTime = 0.75f;
    public GameManager m_manager;

    private string m_FireButton;         
    private float m_CurrentLaunchForce = 30f;  
    //private float m_ChargeSpeed;         
    private bool m_Fired;                

    private void OnEnable()
    {
        //m_CurrentLaunchForce = m_MinLaunchForce;
        //m_AimSlider.value = m_MinLaunchForce;
    }


    private void Start()
    {
        m_FireButton = "Fire" + m_PlayerNumber;

        //m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;
    }
    

    private void Update()
    {
        //Fire when enemy is in range


        //Fire();
        

    }


    private void Fire()
    {
        // Instantiate and launch the shell.

        m_Fired = true;

        Rigidbody shellInstance = Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;

        shellInstance.velocity = m_CurrentLaunchForce * m_FireTransform.forward;

        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();
    }
}