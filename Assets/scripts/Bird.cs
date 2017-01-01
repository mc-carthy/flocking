using UnityEngine;
using System.Collections.Generic;

[AddComponentMenu ("Vistage/Bird")]
public class Bird : MonoBehaviour {

	static public List<Bird> birds;

	public Vector3 vel;
	public Vector3 newVel;
	public Vector3 newPos;

	public List<Bird> neighbours;
	public List<Bird> collisionRisks;
	public Bird closest;

	private void Awake ()
	{
		if (birds == null) {
			birds = new List<Bird> ();
		}

		birds.Add (this);

		Vector3 randPos = Random.insideUnitSphere * BirdSpawner.Instance.spawnRadius;
		randPos.y = 0;
		vel = Random.onUnitSphere;
		vel *= BirdSpawner.Instance.spawnVel;

		neighbours = new List<Bird> ();
		collisionRisks = new List<Bird> ();
		
		transform.parent = GameObject.Find("flock").transform;

		Color randCol = Color.black;
		while (randCol.r + randCol.g + randCol.b < 1.0f) {
			randCol = new Color (Random.value, Random.value, Random.value);
		}

		Renderer[] rends = gameObject.GetComponentsInChildren<Renderer> ();

		foreach (Renderer r in rends) {
			r.material.color = randCol;
		}
	}

	private void Update ()
	{
		neighbours = GetNeighbours(this);

		newVel = vel;
		newPos = transform.position;

		// Match velocity to similar to that of neighbours
		Vector3 neighbourVel = GetAverageVelocity (neighbours);
		newVel += neighbourVel * BirdSpawner.Instance.velocityMatchAmt;

		// Move towards middle of neighbours
		Vector3 neighbourCenterOffset = GetAveragePosition (neighbours) - transform.position;
		newVel += neighbourCenterOffset * BirdSpawner.Instance.flockCenteringAmt;

		// Avoid collisions
		Vector3 dist;
		if (collisionRisks.Count > 0)
		{
			Vector3 collisionAveragePos = GetAveragePosition(collisionRisks);
			dist = collisionAveragePos - transform.position;
			newVel += dist * BirdSpawner.Instance.collisionAvoidanceAmt;
		}

		// Mouse attraction
		dist = BirdSpawner.Instance.mousePos - transform.position;
		if (dist.magnitude > BirdSpawner.Instance.mouseAvoidDist)
		{
			newVel += dist * BirdSpawner.Instance.mouseAttractionAmt;
		}
		else
		{
			newVel -= dist.normalized * BirdSpawner.Instance.mouseAvoidDist * BirdSpawner.Instance.mouseAvoidAmt;
		}
	}

	private void LateUpdate ()
	{
		vel = (1 - BirdSpawner.Instance.velocityLerpAmt) * vel + BirdSpawner.Instance.velocityLerpAmt * newVel;

		if (vel.magnitude > BirdSpawner.Instance.maxVel)
		{
			vel = vel.normalized * BirdSpawner.Instance.maxVel;
		}
		if (vel.magnitude < BirdSpawner.Instance.minVel)
		{
			vel = vel.normalized * BirdSpawner.Instance.minVel;
		}

		newPos = transform.position + vel * Time.deltaTime;
		newPos.y = 0;

		transform.LookAt(newPos);

		transform.position = newPos;
	}

	public List<Bird> GetNeighbours (Bird bird)
	{
		float closestDist = float.MaxValue;
		Vector3 delta;
		float dist;
		neighbours.Clear ();
		collisionRisks.Clear ();

		foreach (Bird b in birds)
		{
			if (b == bird)
			{
				continue;
			}
			
			delta = b.transform.position - bird.transform.position;
			dist = delta.magnitude;

			if (dist < closestDist)
			{
				closestDist = dist;
				closest = b;
			}

			if (dist < BirdSpawner.Instance.nearDist)
			{
				neighbours.Add (b);
			}

			if (dist < BirdSpawner.Instance.collisionDist)
			{
				collisionRisks.Add (b);
			}
		}

		if (neighbours.Count == 0)
		{
			neighbours.Add (closest);
		}

		return neighbours;
	}

	public Vector3 GetAverageVelocity (List<Bird> neighbours)
	{
		Vector3 sum = Vector3.zero;

		foreach (Bird b in neighbours)
		{
			sum += b.vel;
		}

		Vector3 avg = sum / neighbours.Count;
		
		return avg;
	}

	public Vector3 GetAveragePosition (List<Bird> neighbours)
	{
		Vector3 sum = Vector3.zero;

		foreach (Bird b in neighbours)
		{
			sum += b.transform.position;
		}

		Vector3 avg = sum / neighbours.Count;
		
		return avg;	}
}
