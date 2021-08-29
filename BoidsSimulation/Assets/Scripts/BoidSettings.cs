using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BoidSettings : ScriptableObject
{
	// parameters
	public float min_speed = 3f;
	public float max_speed = 5f;
	public float max_steer_force = 5f;

	public float align_weight = 1f;
	public float cohesion_weight = 1f;
	public float separate_weight = 1f;
	public float collision_avoid_weight = 10;
	public float target_weight = 1f;
	
	public LayerMask obstacle_mask;
	public float bounds_radius = 0.27f;
	public float collision_avoid_radius = 5f;
	public float separation_radius = 4f;
	public float perception_radius = 2.5f;
	
	public int view_point_num = 200;
}
