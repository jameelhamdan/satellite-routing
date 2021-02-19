using System.Collections.Generic;
using UnityEngine;


public class Satellite : MonoBehaviour
{
    public Rigidbody rb;
    public Vector3 OrbitPos = Vector3.zero;
    public float OrbitMass = 100000000000f;
    public float SatelliteMass = 10f;
    public float OrbitSpeed = 10f;
    public Manager manager = null;
    
    [SerializeField]
    private float G = 6.67f * Mathf.Pow(10, -11);
    
    List<Node> ReachableSatellites(float maxDistance = 0.0f)
    {
        if (maxDistance == 0.0f)
        {
            maxDistance = manager.radius * 0.77f * 2.0f;  // Default max distance
        }

        
        List<Node> reachable = new List<Node>();
        // Returns satellites in field of this satellite (ignoring other satellite interference)
        
        foreach(Satellite satellite in manager.satellites)
        {
            float distance = Vector3.Distance(satellite.transform.position, this.transform.position);
            if (0.0f < distance && distance < maxDistance)
            {
                reachable.Add(new Node { Weight = distance, Satellite = satellite });
            }
        }
        
        return reachable;
    }
    
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        rb.mass = 10f;
        rb.useGravity = false;
        var satellitePos = this.transform.position;

        var PushVector = Vector3.Normalize(Vector3.Cross(Vector3.up, (OrbitPos - satellitePos)));
        // Initial Acceleration towards Vector3.right of orbit point to get into actual orbit
        // Debug.DrawLine(PushVector, satellitePos, Color.yellow);
        rb.isKinematic = false;
        rb.AddForce(PushVector * OrbitSpeed, ForceMode.Impulse);
    }

    private Vector3 calculateForce()
    {
        var satellitePos = this.transform.position;

        float distance = Vector3.Distance(satellitePos, OrbitPos);
        float distanceSquared = distance * distance;
        
        float force = G * OrbitMass * SatelliteMass / distanceSquared;
        Vector3 heading = (OrbitPos - satellitePos);
        return force * (heading/heading.magnitude);
    }

    void FixedUpdate()
    {
        // Gravitate towards Orbit Point
        rb.AddForce(calculateForce() * 0.1f, ForceMode.Impulse);
    }
}
