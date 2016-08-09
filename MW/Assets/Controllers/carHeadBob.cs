using UnityEngine;
using System.Collections;

public class carHeadBob : MonoBehaviour {

	private float timer = 0.0f;
	public float midpoint;


	// Use this for initialization
	void Start () {

	}

		public void Landing() {

		}

	// Update is called once per frame
	void Update () {
		float waveslice = 0.0f;
		float horizontal = Input.GetAxis("Horizontal");
		float vertical = Input.GetAxis("Vertical");
		if (playerManager.player.currentSpeed.magnitude < 0.2) {
			timer = 0.0f;
		}
		else {
			waveslice = Mathf.Sin(timer);
			timer = timer + playerManager.player.bobSpeed;
			if (timer > Mathf.PI * 2) {
				timer = timer - (Mathf.PI * 2);
			}
		}
		if (waveslice != 0) {
			float translateChange = waveslice * playerManager.player.bobAmount;
			float totalAxes = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
			totalAxes = Mathf.Clamp(totalAxes, 0.0f, 1.0f);
			translateChange = totalAxes * translateChange;
			transform.localPosition = new Vector3(transform.localPosition.x, midpoint + translateChange, transform.localPosition.z);

		}
		else {
			transform.localPosition = new Vector3(transform.localPosition.x, midpoint, transform.localPosition.z);
		}


	}
}
