using System;
using System.Collections.Generic;
using System.Linq;
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
    [ReadOnly] [SerializeField] public List<Satellite> satellites = new List<Satellite>();

    public int generateCount = 3;
    public float radius = 30;
    public float timeScale = 1.0f;
    public Mesh sateliteMesh;
    public Material sateliteMaterial;

    public void VisualizePath(List<Node> path)
    {
        var current = path.First();

        // if path empty
        if (current == null) return;
        path.Remove(current);

        foreach (var node in path)
        {
            // draw line between current and node
            Debug.DrawLine(
                current.Satellite.transform.position,
                node.Satellite.transform.position,
                Color.blue
            );
        }
    }

    public List<Node> CalculatePath(Satellite start, Satellite end)
    {
        List<Node> path = new List<Node>();
        List<Node> visited = new List<Node>();

        int targetId = end.GetInstanceID();

        Node current = new Node {Weight = 0, Satellite = start};

        if (targetId == start.GetInstanceID())
        {
            // no path needed start node is end node
            return new List<Node>();
        }

        // Hill climb algorithm
        while (true)
        {
            visited.Add(current);
            var possibleNodes = current.Satellite.ReachableSatellites(end);

            // Check if ID is within possible nodes
            Node possibleEnd = possibleNodes.Find(x => x.Satellite.Id == current.Satellite.Id);
            // Reached end
            if (possibleEnd != null)
            {
                current = possibleEnd;
                path.Add(current);
                break;
            }
            else
            {
                // Get best Node depending on current weight
                Node bestNode = possibleNodes.OrderBy(x => x.Weight).Except(visited).First();

                // Path leads nowhere
                if (bestNode == null) break;

                // Set current as new node
                current = bestNode;
                path.Add(current);
            }
        }

        return path;
    }

    public void Update()
    {
        // Visualise all possible paths for current satellites
        if (Input.GetKeyDown(KeyCode.Space))
        {
            foreach (var satellite in satellites) satellite.VisualizeReachable();
            Debug.Log("Visualizing nodes");
        }
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
            satellite.Id = i;
            satellite.manager = this;
            // Set gameObject initial position
            ob.transform.position = new Vector3(
                Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)
            ).normalized * radius;

            // set mesh as sphere
            ob.AddComponent<MeshFilter>().mesh = sateliteMesh;
            ob.AddComponent<MeshRenderer>().material = sateliteMaterial;

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