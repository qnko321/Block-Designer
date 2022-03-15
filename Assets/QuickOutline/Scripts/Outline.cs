//
//  Outline.cs
//  QuickOutline
//
//  Created by Chris Nolet on 3/30/18.
//  Copyright © 2018 Chris Nolet. All rights reserved.
//

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DisallowMultipleComponent]

public class Outline : MonoBehaviour 
{
    private static readonly HashSet<Mesh> registeredMeshes = new HashSet<Mesh>();

    public enum Mode {
        OutlineAll,
        OutlineVisible,
        OutlineHidden,
        OutlineAndSilhouette,
        SilhouetteOnly
    }

    public Mode OutlineMode {
        get { return outlineMode; }
        set {
            outlineMode = value;
            needsUpdate = true;
        }
    }

    public Color OutlineColor {
        get { return outlineColor; }
        set {
            outlineColor = value;
            needsUpdate = true;
        }
    }

    public float OutlineWidth {
        get { return outlineWidth; }
        set {
            outlineWidth = value;
            needsUpdate = true;
        }
    }

    [Serializable]
    private class ListVector3 {
        public List<Vector3> data;
    }

    [SerializeField]
    private Mode outlineMode;

    [SerializeField]
    private Color outlineColor = Color.white;

    [SerializeField, Range(0f, 10f)]
    private float outlineWidth = 2f;

    [Header("Optional")]

    [SerializeField, Tooltip("Precompute enabled: Per-vertex calculations are performed in the editor and serialized with the object. "
                             + "Precompute disabled: Per-vertex calculations are performed at runtime in Awake(). This may cause a pause for large meshes.")]
    private bool precomputeOutline;

    [SerializeField, HideInInspector]
    private List<Mesh> bakeKeys = new List<Mesh>();

    [SerializeField, HideInInspector]
    private List<ListVector3> bakeValues = new List<ListVector3>();

    private Renderer[] renderers;
    private Material outlineMaskMaterial;
    private Material outlineFillMaterial;

    private bool needsUpdate;
    
    private static readonly int zTest = Shader.PropertyToID("_ZTest");
    private static readonly int color = Shader.PropertyToID("_OutlineColor");
    private static readonly int width = Shader.PropertyToID("_OutlineWidth");

    private void Awake() 
    {
        // Cache renderers
        renderers = GetComponentsInChildren<Renderer>();

        // Instantiate outline materials
        outlineMaskMaterial = Instantiate(Resources.Load<Material>(@"Materials/OutlineMask"));
        outlineFillMaterial = Instantiate(Resources.Load<Material>(@"Materials/OutlineFill"));

        outlineMaskMaterial.name = "OutlineMask (Instance)";
        outlineFillMaterial.name = "OutlineFill (Instance)";

        // Retrieve or generate smooth normals
        LoadSmoothNormals();

        // Apply material properties immediately
        needsUpdate = true;
    }

    private void OnEnable()
    {
        foreach (var _renderer in renderers) 
        {
            // Append outline shaders
            var _materials = _renderer.sharedMaterials.ToList();

            _materials.Add(outlineMaskMaterial);
            _materials.Add(outlineFillMaterial);

            _renderer.materials = _materials.ToArray();
        }
    }

    private void OnValidate() {

        // Update material properties
        needsUpdate = true;

        // Clear cache when baking is disabled or corrupted
        if (!precomputeOutline && bakeKeys.Count != 0 || bakeKeys.Count != bakeValues.Count)
        {
            bakeKeys.Clear();
            bakeValues.Clear();
        }

        // Generate smooth normals when baking is enabled
        if (precomputeOutline && bakeKeys.Count == 0)
            Bake();
    }

    private void Update() 
    {
        if (needsUpdate) 
        {
            needsUpdate = false;

            UpdateMaterialProperties();
        }
    }

    private void OnDisable()
    {
        foreach (var _renderer in renderers) 
        {
            // Remove outline shaders
            var _materials = _renderer.sharedMaterials.ToList();

            _materials.Remove(outlineMaskMaterial);
            _materials.Remove(outlineFillMaterial);

            _renderer.materials = _materials.ToArray();
        }
    }

    

    private void OnDestroy() 
    {
        // Destroy material instances
        Destroy(outlineMaskMaterial);
        Destroy(outlineFillMaterial);
    }

    private void Bake() 
    {
        // Generate smooth normals for each mesh
        var _bakedMeshes = new HashSet<Mesh>();

        foreach (var _meshFilter in GetComponentsInChildren<MeshFilter>()) 
        {
            // Skip duplicates
            if (!_bakedMeshes.Add(_meshFilter.sharedMesh)) {
                continue;
            }

            // Serialize smooth normals
            var _smoothNormals = SmoothNormals(_meshFilter.sharedMesh);

            bakeKeys.Add(_meshFilter.sharedMesh);
            bakeValues.Add(new ListVector3() { data = _smoothNormals });
        }
    }

    private void LoadSmoothNormals() 
    {
        // Retrieve or generate smooth normals
        foreach (var _meshFilter in GetComponentsInChildren<MeshFilter>()) 
        {
            // Skip if smooth normals have already been adopted
            if (!registeredMeshes.Add(_meshFilter.sharedMesh)) continue;
            
            // Retrieve or generate smooth normals
            var _index = bakeKeys.IndexOf(_meshFilter.sharedMesh);
            var _smoothNormals = _index >= 0 ? bakeValues[_index].data : SmoothNormals(_meshFilter.sharedMesh);

            // Store smooth normals in UV3
            _meshFilter.sharedMesh.SetUVs(3, _smoothNormals);
        }

        // Clear UV3 on skinned mesh renderers
        foreach (var _skinnedMeshRenderer in GetComponentsInChildren<SkinnedMeshRenderer>()) {
            if (registeredMeshes.Add(_skinnedMeshRenderer.sharedMesh)) {
                _skinnedMeshRenderer.sharedMesh.uv4 = new Vector2[_skinnedMeshRenderer.sharedMesh.vertexCount];
            }
        }
    }

    private List<Vector3> SmoothNormals(Mesh _mesh) 
    {
        // Group vertices by location
        var _groups = _mesh.vertices.Select((_vertex, _index) => new KeyValuePair<Vector3, int>(_vertex, _index)).GroupBy(_pair => _pair.Key);

        // Copy normals to a new list
        var _smoothNormals = new List<Vector3>(_mesh.normals);

        // Average normals for grouped vertices
        foreach (var _group in _groups) 
        {
            // Skip single vertices
            if (_group.Count() == 1) continue;

            // Calculate the average normal
            var _smoothNormal = Vector3.zero;

            foreach (var _pair in _group)
            { 
                _smoothNormal += _mesh.normals[_pair.Value];
            }

            _smoothNormal.Normalize();

            // Assign smooth normal to each vertex
            foreach (var _pair in _group) {
                _smoothNormals[_pair.Value] = _smoothNormal;
            }
        }

        return _smoothNormals;
    }

    private void UpdateMaterialProperties() 
    {
        // Apply properties according to mode
        outlineFillMaterial.SetColor(color, outlineColor);

        switch (outlineMode) 
        {
            case Mode.OutlineAll:
                outlineMaskMaterial.SetFloat(zTest, (float)UnityEngine.Rendering.CompareFunction.Always);
                outlineFillMaterial.SetFloat(zTest, (float)UnityEngine.Rendering.CompareFunction.Always);
                outlineFillMaterial.SetFloat(width, outlineWidth);
                break;

            case Mode.OutlineVisible:
                outlineMaskMaterial.SetFloat(zTest, (float)UnityEngine.Rendering.CompareFunction.Always);
                outlineFillMaterial.SetFloat(zTest, (float)UnityEngine.Rendering.CompareFunction.LessEqual);
                outlineFillMaterial.SetFloat(width, outlineWidth);
                break;

            case Mode.OutlineHidden:
                outlineMaskMaterial.SetFloat(zTest, (float)UnityEngine.Rendering.CompareFunction.Always);
                outlineFillMaterial.SetFloat(zTest, (float)UnityEngine.Rendering.CompareFunction.Greater);
                outlineFillMaterial.SetFloat(width, outlineWidth);
                break;

            case Mode.OutlineAndSilhouette:
                outlineMaskMaterial.SetFloat(zTest, (float)UnityEngine.Rendering.CompareFunction.LessEqual);
                outlineFillMaterial.SetFloat(zTest, (float)UnityEngine.Rendering.CompareFunction.Always);
                outlineFillMaterial.SetFloat(width, outlineWidth);
                break;

            case Mode.SilhouetteOnly:
                outlineMaskMaterial.SetFloat(zTest, (float)UnityEngine.Rendering.CompareFunction.LessEqual);
                outlineFillMaterial.SetFloat(zTest, (float)UnityEngine.Rendering.CompareFunction.Greater);
                outlineFillMaterial.SetFloat(width, 0);
                break;
        }
    }
}