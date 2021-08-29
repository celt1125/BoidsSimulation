using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshManager : MonoBehaviour
{
    // Start is called before the first frame update
    static public MeshData GenerateSphereMesh(int divisions, float radius){
		// Basis octahedron parameters
		Vector3[] basic_vertices = new Vector3[] {	Vector3.up * radius, Vector3.left * radius, Vector3.forward * radius,
													Vector3.right * radius, Vector3.back * radius, Vector3.down * radius};
		int[] edge_list = new int[] {0,1,0,2,0,3,0,4,1,2,2,3,3,4,4,1,5,1,5,2,5,3,5,4};
		//                           0   1   2   3   4   5   6   7   8   9   10  11
		int[] face_list = new int[] {1,0,4, 2,1,5, 3,2,6, 0,3,7, 8,9,4, 9,10,5, 10,11,6, 11,8,7};
		int face_vertices = (divisions + 2) * (divisions + 3) / 2;
		int vertices_size = face_vertices * 8 - (divisions + 2) * 12 + 6;
		int triangles_size = (divisions + 1) * (divisions + 1) * 8 * 3;
		
		// Set the basic octahedron
		MeshData sphere = new MeshData(vertices_size, triangles_size);
		sphere.vertices.AddRange(basic_vertices);
		
		// Set the vertices on edges
		Edge[] edges = new Edge[12];
		for (int i = 0; i < edge_list.Length; i += 2){
			// The vertices at the two ends of the edge
			Vector3 start_v = sphere.vertices.items[edge_list[i]];
			Vector3 end_v = sphere.vertices.items[edge_list[i + 1]];
			
			// Set the vertices on the edge
			int[] e = new int[divisions + 2];
			e[0] = edge_list[i];
			for (int j = 1; j < divisions + 1; j++){
				e[j] = sphere.vertices.NextIndex();
				sphere.vertices.Add(Vector3.Slerp(start_v, end_v, (float)j / (divisions + 1f)));
			}
			e[divisions + 1] = edge_list[i + 1];
			
			edges[i / 2] = new Edge(e);
		}
		
		// Set the vetices and triangles on each face
		for (int i = 0; i < face_list.Length; i += 3){
			// Set the three sides
			Edge left = edges[face_list[i]];
			Edge right = edges[face_list[i + 1]];
			Edge bottom = i >= 12 ? edges[face_list[i + 2]] : edges[face_list[i + 2]].Reverse(); // index form left to right
			
			// Set vertices on the inner bottom edges
			Edge[] inner_edges = new Edge[divisions + 2];
			inner_edges[0] = new Edge(new int[] {left.indices[0]});
			inner_edges[divisions + 1] = bottom;
			for (int j = 1; j < divisions + 1; j++){
				Vector3 start_v = sphere.vertices.items[left.indices[j]];
				Vector3 end_v = sphere.vertices.items[right.indices[j]];
				int[] sub_bottom = new int[j + 1];
				sub_bottom[0] = left.indices[j];
				for (int k = 1; k < j; k++){
					sub_bottom[k] = sphere.vertices.NextIndex();
					sphere.vertices.Add(Vector3.Slerp(start_v, end_v, (float)k / j));
				}
				sub_bottom[j] = right.indices[j];
				inner_edges[j] = new Edge(sub_bottom);
			}
			
			// Set triangles
			for (int j = 1; j < divisions + 1; j++){
				for (int k = 0; k < j; k++){
					// upward triangle
					sphere.triangles.Add(inner_edges[j].indices[k]);
					sphere.triangles.Add(inner_edges[j - 1].indices[k]);
					sphere.triangles.Add(inner_edges[j].indices[k + 1]);
					
					// downward triangle
					sphere.triangles.Add(inner_edges[j].indices[k]);
					sphere.triangles.Add(inner_edges[j].indices[k + 1]);
					sphere.triangles.Add(inner_edges[j + 1].indices[k + 1]);
				}
			}
			for (int k = 0; k < divisions + 1; k++){
				sphere.triangles.Add(bottom.indices[k]);
				sphere.triangles.Add(inner_edges[divisions].indices[k]);
				sphere.triangles.Add(bottom.indices[k + 1]);
			}
		}
		return sphere;
	}
	
	static public MeshData GeneratePyramidMesh(float size){
		MeshData pyramid = new MeshData(5, 18);
		Vector3[] basic_vertices = new Vector3[] {Vector3.right * 0.5f - Vector3.back, Vector3.down * 0.5f - Vector3.back,
									Vector3.left * 0.5f - Vector3.back, Vector3.up * 0.5f  - Vector3.back, Vector3.forward * 2f};
		for (int i = 0; i < 5; i++)
			basic_vertices[i] *= size;
		pyramid.vertices.AddRange(basic_vertices);
		pyramid.triangles.AddRange(new int[] { 0, 1, 3, 1, 2, 3, 0, 4, 1, 1, 4, 2, 2, 4, 3, 3, 4, 0});
		
		return pyramid;
	}
}

public class MeshData{
	public Array<Vector3> vertices;
	public Array<int> triangles;
	
	public MeshData(int v_size, int t_size){
		vertices = new Array<Vector3>(v_size);
		triangles = new Array<int>(t_size);
	}
	
	public MeshData(MeshData other){
		vertices = new Array<Vector3>(other.vertices.items.Length);
		triangles = new Array<int>(other.triangles.items.Length);
		foreach (Vector3 v in other.vertices.items)
			vertices.Add(v);
		foreach (int t in other.triangles.items)
			triangles.Add(t);
	}
}

public class Edge{
	public int[] indices;
	
	public Edge(int[] edge_indices){
		indices = edge_indices;
	}
	
	public Edge Reverse(){
		int[] res = new int[indices.Length];
		for (int i = 0; i < indices.Length; i++)
			res[i] = indices[indices.Length - 1 - i];
		return new Edge(res);
	}
}

public class Array<T>{
	public T[] items;
	private int idx;
	
	public Array(int size){
		idx = 0;
		items = new T[size];
	}
	
	public int NextIndex(){
		return idx;
	}
	
	public void Add(T item){
		items[idx] = item;
		idx++;
	}
	
	public void AddRange(IEnumerable<T> arr){
		foreach (T item in arr)
			Add(item);
	}
}
