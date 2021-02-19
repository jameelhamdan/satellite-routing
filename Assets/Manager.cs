
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Node
{
    public float Weight { get; set; }
    public Satellite Satellite { get; set; }
}


public class Manager : MonoBehaviour
{
    [ReadOnly]
    [SerializeField]
    public List<Satellite> satellites = new List<Satellite>();

    public int generateCount = 3;
    public float radius = 30;
    public float timeScale = 1.0f;
    public Mesh SateliteMesh;
    public Material SateliteMaterial;

    public List<Node> CalculatePath(Satellite start, Satellite end)
    {
        List<Node> path = new List<Node>();

        int targetId = end.GetInstanceID();
        
        Node current = new Node {Weight = 0, Satellite = start};

        if (targetId == start.GetInstanceID())
        {
            // no path needed start node is end node
            return new List<Node>();
        }

        // Path not found
        return new List<Node>();
    }
    
    void Start()
    {
        // Initialize satellites
        for (var i = 0; i < generateCount; i++)
        {
            // Add Game object and scripts
            var ob = new GameObject();
            ob.name = "Satellite " + (i + 1);
            ob.transform.SetParent(this.transform);
            
            var rb = ob.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            
            var satellite = ob.AddComponent<Satellite>();
            satellite.manager = this;
            // Set gameObject initial position
            ob.transform.position = new Vector3(
            Random.Range(-1f, 1f),Random.Range(-1f, 1f),Random.Range(-1f, 1f)
            ).normalized * radius;
            
            // set mesh as sphere
            ob.AddComponent<MeshFilter>().mesh = SateliteMesh;
            ob.AddComponent<MeshRenderer>().material = SateliteMaterial;
            
            // set TrailRenderer for satellite
            ob.AddComponent<TrailRenderer>();
            ob.GetComponent<TrailRenderer>().time = 10;
            
            var trailColor = new Color(Random.value, Random.value, Random.value, Random.value);
            ob.GetComponent<TrailRenderer>().material.SetColor("_Color", trailColor);
            
            satellites.Add(satellite);
        }
        Time.timeScale = timeScale;
    }
}
