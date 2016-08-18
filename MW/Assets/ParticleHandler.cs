using UnityEngine;
using System.Collections;

public class ParticleHandler : MonoBehaviour {

	private Transform m_cachedTransform;
	private ParticleSystemRenderer m_particleRenderer;
	[SerializeField] private Vector2 m_particleScaleRange;
	[SerializeField] private float m_positionSmooth;
	[SerializeField] private float m_rotationSmooth;

	// Use this for initialization
	void Start () {
		m_cachedTransform = transform;
		m_particleRenderer = GetComponent<ParticleSystemRenderer> ();
	}
	
	// Update is called once per frame
	void Update () {
		m_cachedTransform.position = Vector3.Lerp(m_cachedTransform.position,
			playerManager.player.CachedTransform.position, Time.deltaTime * m_positionSmooth);

			m_cachedTransform.rotation = Quaternion.Slerp (m_cachedTransform.rotation,
				playerManager.player.CachedTransform.rotation, Time.deltaTime * m_rotationSmooth);
		

		m_particleRenderer.lengthScale = Mathf.Lerp(m_particleScaleRange.x,
			m_particleScaleRange.y,playerManager.player.currentSpeed.magnitude);
	}
}
