using UnityEngine;

[AddComponentMenu ("Vistage/BirdSpawner")]
public class BirdSpawner : MonoBehaviour {

	static public BirdSpawner Instance;

	public int numBirds = 100;
	public GameObject birdPrefab;
	public float spawnRadius = 100f;
	public float spawnVel = 10f;
	public float minVel = 0f;
	public float maxVel = 30f;
	public float nearDist = 30f;
	public float collisionDist = 5f;
	public float velocityMatchAmt = 0.01f;
	public float flockCenteringAmt = 0.15f;
	public float collisionAvoidanceAmt = -0.5f;
	public float mouseAttractionAmt = 0.01f;
	public float mouseAvoidAmt = 0.75f;
	public float mouseAvoidDist = 15f;
	public float velocityLerpAmt = 0.25f;

	public Vector3 mousePos;

	private void Start ()
	{
		Instance = this;

		for (int i = 0; i < numBirds; i++) {
			Instantiate (birdPrefab);
		}
	}

	private void LateUpdate ()
	{
		Vector3 mousePos2D = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, transform.position.y);
		mousePos = Camera.main.ScreenToWorldPoint (mousePos2D);
	}
}
