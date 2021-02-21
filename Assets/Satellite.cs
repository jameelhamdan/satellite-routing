using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Satellite : MonoBehaviour
{
    public int Id = -1;
    public Rigidbody rb;
    public Vector3 orbitPos = Vector3.zero;
    public float orbitMass = 100000000000f;
    public float satelliteMass = 10f;
    public float orbitSpeed = 10f;
    public Manager manager = null;

    [SerializeField] private float G = 6.67f * Mathf.Pow(10, -11);

    public void VisualizeReachable()
    {
        float alpha = 1.0f;
        var list = ReachableSatellites().OrderByDescending(x => x.Weight).ToList();
        var count = list.Count();
        foreach (var node in list)
        {
            // draw line between current and node
            var color = new Color(1.0f, 0.0f, 0.0f, alpha);
            // reduce alpha by percentage
            alpha -= (1.0f / (count));
            
            Debug.DrawLine(
                transform.position,
                node.Satellite.transform.position,
                color,
                10f
            );
        }
    }
    
    public List<Node> ReachableSatellites(Satellite target = null, float maxDistance = 0.0f)
    {
        if (maxDistance == 0.0f)
        {
            maxDistance = manager.radius * 0.77f * 2.0f; // Default max distance
        }

        List<Node> reachable = new List<Node>();
        var currentPos = transform.position;
        // Returns satellites in field of this satellite (ignoring other satellite interference)
        foreach (Satellite satellite in manager.satellites)
        {
            var distance = Vector3.Distance(satellite.transform.position, currentPos);
            var targetDistance = target != null ? Vector3.Distance(target.transform.position, currentPos): 0.0f;

            if (0.0f < distance && distance < maxDistance)
            {
                // Normalized distances to get weight
                var weight = (float) Math.Sqrt((distance * distance) + (targetDistance * targetDistance));
                reachable.Add(new Node {Weight = weight, Satellite = satellite});
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

        var pushVector = Vector3.Normalize(Vector3.Cross(Vector3.up, (orbitPos - satellitePos)));
        // Initial Acceleration towards Vector3.right of orbit point to get into actual orbit
        // Debug.DrawLine(PushVector, satellitePos, Color.yellow);
        rb.isKinematic = false;
        rb.AddForce(pushVector * orbitSpeed, ForceMode.Impulse);
    }

    private Vector3 CalculateForce()
    {
        var satellitePos = this.transform.position;

        var distance = Vector3.Distance(satellitePos, orbitPos);
        var distanceSquared = distance * distance;

        float force = G * orbitMass * satelliteMass / distanceSquared;
        Vector3 heading = (orbitPos - satellitePos);
        return force * (heading / heading.magnitude);
    }

    void FixedUpdate()
    {
        // Gravitate towards Orbit Point
        rb.AddForce(CalculateForce() * 0.1f, ForceMode.Impulse);
    }
}