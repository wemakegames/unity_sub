using UnityEngine;
using System.Collections;

public class PlayerLineRenderer : MonoBehaviour {

	[HideInInspector]

	public Color c1 = Color.yellow;
	public Color c2 = Color.green;


	private GameManager gameManager;
	private int lengthOfLineRenderer = 3;

	void Start() {

		LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
		lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
		lineRenderer.SetColors(c1, c2);
		lineRenderer.SetWidth(1.0f, 1.0f);
		lineRenderer.SetVertexCount(lengthOfLineRenderer);

		gameManager = GameObject.Find ("GameManager").GetComponent<GameManager>();

	}
	
	// Update is called once per frame
	void Update(){

	}

	public void DrawPlayerLine (Vector3 p1, Vector3 p2) {


		LineRenderer lineRenderer = GetComponent<LineRenderer>();

		if (gameManager.nextTeam == 2){
			c1 = gameManager.team1Color;
			c2 = c1;
		} else {
			c1 = gameManager.team2Color;
			c2 = c1;
		}
		c2.a = 0f;
		lineRenderer.SetColors(c1, c1);
		lineRenderer.SetPosition(0, gameObject.transform.position);
		lineRenderer.SetPosition(1, p1);
		lineRenderer.SetColors(c1, c2);
		lineRenderer.SetPosition(2, p2);

	}
}

