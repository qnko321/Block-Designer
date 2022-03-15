using System;
using System.Collections.Generic;
using UnityEngine;

public class CoordinateMesh : MonoBehaviour
{
    [SerializeField] private int meshWidthX;
    [SerializeField] private int meshWidthZ;

    private LineRenderer lineRenderer;

    private List<Vector3> points = new List<Vector3>();

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        LoadPoints();
        SetupLines();
    }

    private void LoadPoints()
    {
        for (int x = 0; x < meshWidthX; x++)
        {
            for (int z = 0; z < meshWidthZ; z++)
            {
                points.Add(new Vector3(x, 0, z));
            }
        }
    }

    private void SetupLines()
    {
        lineRenderer.positionCount = points.Count;
    }

    private void Update()
    {
        for (int i = 0; i < points.Count; i++)
        {
            lineRenderer.SetPosition(i, points[i]);
        }
    }
}
