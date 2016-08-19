using UnityEngine;
using System.Collections;

public class ParticleHandler : MonoBehaviour {

	private Transform m_cachedTransform;
	private ParticleSystemRenderer m_particleRenderer;
	private ParticleSystem m_parSystem;
	[SerializeField] private Vector2 m_particleScaleRange;
	[SerializeField] private float m_positionSmooth;
	[SerializeField] private float m_rotationSmooth;

	// Use this for initialization
	void Start () {
		m_cachedTransform = transform;
		m_particleRenderer = GetComponent<ParticleSystemRenderer> ();
		m_parSystem = GetComponent<ParticleSystem> ();
	}

	// Update is called once per frame
	void Update () {
		m_cachedTransform.position = Vector3.Lerp(m_cachedTransform.position,
			playerManager.player.CachedTransform.position, Time.deltaTime * m_positionSmooth);

		if (playerManager.player.onGround) {
			m_cachedTransform.rotation = Quaternion.Slerp (m_cachedTransform.rotation,
				playerManager.player.CachedTransform.rotation, Time.deltaTime * m_rotationSmooth);
		} else {

				m_cachedTransform.LookAt((2*playerManager.player.GetComponent<Rigidbody>().velocity)-playerManager.player.GetComponent<Rigidbody>().velocity);


			/*Quaternion.Slerp (m_cachedTransform.rotation,
				playerManager.player.CachedTransform.rotation, Time.deltaTime * m_rotationSmooth);*/
		}

		if (playerManager.player.mouseState) {
			m_particleRenderer.lengthScale = Mathf.Lerp(m_particleScaleRange.x,
				m_particleScaleRange.y,playerManager.player.currentSpeed.magnitude);
		}

	}
}
