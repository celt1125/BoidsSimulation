using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MeshChanger : MonoBehaviour
{
	public float size;
	public Material material;
	
	private Mesh mesh;
	private MeshData initial_mesh;
	
    // Use this for initialization
    void Start()
    {
		gameObject.AddComponent<MeshFilter>();
		gameObject.AddComponent<MeshRenderer>();
		mesh = GetComponent<MeshFilter>().mesh;
		GetComponent<MeshRenderer>().material = material;
		
		initial_mesh = MeshManager.GeneratePyramidMesh(size);
		SetMesh();
		mesh.RecalculateNormals();
    }
	
	private void SetMesh(){
		mesh.Clear();
		const int vertex_limit_16 = 1 << 16 - 1; // 65535
		mesh.indexFormat = (initial_mesh.vertices.items.Length < vertex_limit_16) ?
					UnityEngine.Rendering.IndexFormat.UInt16 : UnityEngine.Rendering.IndexFormat.UInt32;
		mesh.SetVertices(initial_mesh.vertices.items);
		mesh.SetTriangles(initial_mesh.triangles.items, 0, true);
	}
}
