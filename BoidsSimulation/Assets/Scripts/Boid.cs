using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
	private BoidSettings settings;
	private Vector3 velocity;
	
	public void UpdateVelocity(Vector3 accel){
		velocity += accel * Time.deltaTime;
		float speed = Mathf.Clamp(velocity.magnitude, settings.min_speed, settings.max_speed);
		Vector3 dir = velocity.normalized;
		velocity = dir * speed;
		
		transform.position += velocity * Time.deltaTime;
		transform.forward = dir;
	}
	
	public Vector3 SteerToTarget(Vector3 target){
		return SteerForce(target - transform.position) * settings.target_weight;
	}
	
	public Vector3 GetBoidForce(Vector3 align_dir, Vector3 cohesion_pos, Vector3 separate_dir){
		Vector3 accel = Vector3.zero;
		accel += SteerForce(align_dir) * settings.align_weight;
		accel += SteerForce(cohesion_pos - transform.position) * settings.cohesion_weight;
		accel += SteerForce(separate_dir) * settings.separate_weight;
		return accel;
	}
	
	public Vector3 CollisionAvoidanceForce(Vector3[] view_points){
		RaycastHit hit;
		if (Physics.SphereCast(transform.position, settings.bounds_radius, transform.forward,
								out hit, settings.collision_avoid_radius, settings.obstacle_mask)){
			foreach (Vector3 point in view_points){
				Vector3 dir = transform.TransformDirection(point);
				Ray ray = new Ray(transform.position, dir);
				if (!Physics.SphereCast(ray, settings.bounds_radius, settings.collision_avoid_radius, settings.obstacle_mask)){
					return SteerForce(dir) * settings.collision_avoid_weight;
				}
			}
			return SteerForce(transform.forward) * settings.collision_avoid_weight;
		}
		else return Vector3.zero;
	}
	
	
	public void Initialize(BoidSettings s){
		settings = s;
		velocity = transform.forward * (settings.min_speed + settings.max_speed) * 0.5f;
	}
	
	private Vector3 SteerForce(Vector3 dir){
		return Vector3.ClampMagnitude(dir.normalized * settings.max_speed - velocity, settings.max_steer_force);
	}
}




