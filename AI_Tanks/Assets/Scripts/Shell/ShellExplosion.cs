using UnityEngine;

public class ShellExplosion : MonoBehaviour
{
    public LayerMask m_TankMask;
    public ParticleSystem m_ExplosionParticles;       
    public AudioSource m_ExplosionAudio;              
    public float m_MaxDamage = 100f;                  
    public float m_ExplosionForce = 0f;            
    public float m_MaxLifeTime = 10f;                  
    public float m_ExplosionRadius = 5f;

    private Rigidbody shellrb;

    private void Start()
    {
        shellrb = gameObject.GetComponent<Rigidbody>();
        Destroy(gameObject, m_MaxLifeTime);
    }

    private void Update()
    {
        transform.forward = shellrb.velocity;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != 10)
        {
            // Find all the tanks in an area around the shell and damage them.
            Collider[] colliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius, m_TankMask);

            for (int i = 00; i < colliders.Length; i++)
            {
                Rigidbody targetRigibody = colliders[i].GetComponent<Rigidbody>();

                if (!targetRigibody)
                {
                    continue;
                }

                //targetRigibody.AddExplosionForce(m_ExplosionForce, transform.position, m_ExplosionRadius);

                TankHealth targetHealth = targetRigibody.GetComponent<TankHealth>();

                if (!targetHealth)
                {
                    continue;
                }

                float damage = CalculateDamage(targetRigibody.position);

                targetHealth.TakeDamage(damage);
            }

            m_ExplosionParticles.transform.parent = null; //so when shells gets destroyed the particles still on

            m_ExplosionParticles.Play();

            m_ExplosionAudio.Play();

            Destroy(m_ExplosionParticles.gameObject, m_ExplosionParticles.main.duration);
            Destroy(gameObject);
        }
    }


    private float CalculateDamage(Vector3 targetPosition)
    {
        // Calculate the amount of damage a target should take based on it's position.

        Vector3 explosionToTarget = targetPosition - transform.position;

        float explosionDistance = explosionToTarget.magnitude;

        float relativeDistance = (m_ExplosionRadius - explosionDistance) / m_ExplosionRadius;

        float damage = relativeDistance * m_MaxDamage;

        damage = Mathf.Max(0f, damage);

        return damage;
    }
}