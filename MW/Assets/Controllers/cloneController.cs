using UnityEngine;
using System.Collections;

public class cloneController : MonoBehaviour {
	private float timer;
	// Use this for initialization
	void Start () {
		transform.localPosition = new Vector3 (playerManager.player.transform.localPosition.x, playerManager.player.transform.localPosition.y, playerManager.player.transform.localPosition.z);
		Renderer rend = GetComponent<Renderer>();


		if (playerManager.player.currentSpeed.magnitude <= 11) {
			rend.material.color = new Color (0.66f, .05f, 255f, 0.9f); //11
		} else if (playerManager.player.currentSpeed.magnitude <= 25) {
			rend.material.color = new Color (1f, 0f, .67f, 0.9f);//25
		} else if (playerManager.player.currentSpeed.magnitude <= 38) {
			rend.material.color = new Color (1f, .05f, 0f, 0.9f); //38
		} else if (playerManager.player.currentSpeed.magnitude <= 45) {
			rend.material.color = new Color (.9f, .27f, 0f, 0.9f); //45
		} else{
			rend.material.color = new Color (1f, .73f, 0f, 0.9f); //50
		}





	}
	
	// Update is called once per frame
	void Update () {
		

		timer += Time.deltaTime; 

		if (timer >= 12) {
			Destroy(this.gameObject);
		}
	}
}
