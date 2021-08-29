using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidManager : MonoBehaviour
{
	public GameObject boid_prefab;
    //public ComputeShader compute;
	public BoidSettings settings;
	public Transform target;
	
	public int boid_size;
	public float spawn_radius;
	public bool target_enable;
	
	private const int thread_group_size = 1024;
	private List<Boid> boids;
	private Vector3[] view_points;
	
    void Awake()
    {
		if (boid_size <= 0)
			boid_size = 10;
		
		boids = new List<Boid>();
		for (int i = 0; i < boid_size; i++){
			Vector3 spawn_pos = transform.position + Random.insideUnitSphere * spawn_radius;
			GameObject boid = Instantiate(boid_prefab);
			boid.transform.position = spawn_pos;
			boid.transform.forward = Random.insideUnitSphere;
			boids.Add(boid.GetComponent<Boid>());
			boids[boids.Count - 1].Initialize(settings);
		}
		
		SetViewPoints();
    }
	
	void Update(){
		BoidData[] boid_data = new BoidData[boid_size];
		
		for (int i = 0; i < boid_size; i++){
			boid_data[i].position = boids[i].transform.position;
			boid_data[i].direction = boids[i].transform.forward;
		}
		/*
		ComputeBuffer boid_buffer = new ComputeBuffer(boid_size, BoidData.Size);
		boid_buffer.SetData(boid_data);
		
		compute.SetBuffer(0, "boids", boid_buffer);
		compute.SetInt("boid_size", boid_size);
		compute.SetFloat("perception_radius", settings.perception_radius);
		compute.SetFloat("separation_radius", settings.separation_radius);
		
		int thread_group_num = Mathf.CeilToInt((float)boid_size / thread_group_size);
		//Debug.Log(thread_group_num);
		compute.Dispatch (0, thread_group_num, 1, 1);
		boid_buffer.GetData(boid_data);
		*/
		Compute(boid_data);
		
		for (int i = 0; i < boid_size; i++){
			Vector3 accel = Vector3.zero;
			
			if (boid_data[i].detected_mates_num > 0)
				accel += boids[i].GetBoidForce(	boid_data[i].align_direction,
												boid_data[i].cohesion_position / boid_data[i].detected_mates_num,
												boid_data[i].separate_direction);
			if (target_enable)
				accel += boids[i].SteerToTarget(target.position);
			accel += boids[i].CollisionAvoidanceForce(view_points);
			boids[i].UpdateVelocity(accel);
		}
		
		//boid_buffer.Release ();
	}
	
	private void SetViewPoints(){
		view_points = new Vector3[settings.view_point_num];
		float golden_ratio = (1 + Mathf.Sqrt (5)) / 2;
		float angle_delta = Mathf.PI * 2 * golden_ratio;
		for (int i = 0; i < settings.view_point_num; i++){
			float t = (float)i / settings.view_point_num;
			float inclination = Mathf.Acos(1 - 2 * t);
			float azimuth = angle_delta * i;

			float x = Mathf.Sin(inclination) * Mathf.Cos(azimuth);
			float y = Mathf.Sin(inclination) * Mathf.Sin(azimuth);
			float z = Mathf.Cos(inclination);
			view_points[i] = new Vector3(x, y, z);
		}
	}
	
	public struct BoidData {
        public Vector3 position;
        public Vector3 direction;

        public Vector3 align_direction;
        public Vector3 cohesion_position;
        public Vector3 separate_direction;
        public int detected_mates_num;

        public static int Size {
            get {
                return sizeof(float) * 3 * 5 + sizeof(int);
            }
        }
    }
	
	private void Compute(BoidData[] boid_data){
		for (int id = 0; id < boid_size; id++){
			for (int index = 0; index < boid_size; index++){
				if (id != index){
					Vector3 offset = boid_data[index].position - boid_data[id].position;
					float magnitude = Mathf.Sqrt(Vector3.Dot(offset, offset));
					
					if (magnitude < settings.perception_radius){
						boid_data[id].align_direction += boid_data[index].direction;
						boid_data[id].cohesion_position += boid_data[index].position;
						boid_data[id].detected_mates_num++;
					}
					if (magnitude < settings.separation_radius)
						boid_data[id].separate_direction -= offset / magnitude;
				}
			}
		}
	}
}
