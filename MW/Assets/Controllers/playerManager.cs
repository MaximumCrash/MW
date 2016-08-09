using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class playerManager : MonoBehaviour {
	public static playerManager player;

	public enum WalkSetting {A, B, C, Custom};

	[Space(5)]
	[Header ("Player Walk Setting")]
	public WalkSetting walkSetting;

	//FLOATS
	[Space(10)]
	[Header ("Horizontal Forces")]
	[SerializeField]
	private Vector2 _currentSpeed;
	public Vector2 currentSpeed {get {return _currentSpeed;}}
	[Space(10)]
	[SerializeField]
	private float _forceAcceleration;
	public float forceAcceleration {get {return _forceAcceleration;} set {_forceAcceleration = value;}}
	[SerializeField]
	 private float _maximumSpeed;
	public float maximumSpeed {get {return _maximumSpeed;} set {_maximumSpeed = value;}}
	[SerializeField]
	private float _timeToStop;
	public float timetoStop {get {return _timeToStop;} set {_timeToStop = value;}}
	public float airControlRatio;
	[Space(10)]
	public float _timeToStopVolx;
	public float _timeToStopVolz;
	[Space(10)]
	[Header ("Vertical Forces")]
	public float verticalAddForce;
	public float dropVelocity;
	public float maxSlope;
	[Space(10)]
	[Header ("Player Checks")]
	[SerializeField]
	 private bool _onGround = false;
	public bool onGround {get {return _onGround;}}
	[SerializeField]
	 private bool _flip;
	public bool flip {get {return _flip;}}
	[SerializeField]
	private bool _hardFall = false;
	public bool hardFall {get {return _hardFall;}}
	[SerializeField]
	 private bool _mouseState = false;
	public bool mouseState {get {return _mouseState;}}
	[Space(10)]
	[Header ("External Attachments")]
	public Camera cam;
	[SerializeField]
	private float _bobSpeed;
	public float bobSpeed {get {return _bobSpeed;}}
	[SerializeField]
	private float _bobAmount;
	public float bobAmount {get {return _bobAmount;}}
	[Space(10)]
	public Transform thisTransform;
	private float timer;

	private bool flipInit;
	private float floorOldY;
	public float myRot = 0f;

	void Awake()
	{
		player = this;

		switch (walkSetting) {

		case WalkSetting.A : {
				_forceAcceleration = 6440f;
				_maximumSpeed = 10f;
				_timeToStop = 0.2f;
				airControlRatio = 1f;
				verticalAddForce = 1000f;
				dropVelocity = 20f;
				maxSlope = 60f;
				break;
			}
		case WalkSetting.B : {
				_forceAcceleration = 6440f;
				_maximumSpeed = 8f;
				_timeToStop = 0.2f;
				airControlRatio = 0.1f;
				verticalAddForce = 20f;
				dropVelocity = 20f;
				maxSlope = 60f;
				break;
			}
		case WalkSetting.C : {
				_forceAcceleration = 6440f;
				_maximumSpeed = 8f;
				_timeToStop = 0.2f;
				airControlRatio = 0.1f;
				verticalAddForce = 20f;
				dropVelocity = 20f;
				maxSlope = 60f;
				break;
			}
		case WalkSetting.Custom : {

				break;
			}
		}

		timer = 0;
		_flip = false;
	}

	void FixedUpdate()
	{

		_currentSpeed = new Vector2(GetComponent<Rigidbody>().velocity.x,GetComponent<Rigidbody>().velocity.z);
		timer += Time.deltaTime;

		/*if (_flip) {

			if (camRot < 180) {
				camRot += Time.deltaTime * 300;
				camera.transform.localRotation = Quaternion.AngleAxis (camRot, this.transform.forward);
			}
		} else {
			if (camRot > 0) {
				camRot -= Time.deltaTime * 300;
				camera.transform.Rotate (0, 0, camRot);
				camera.transform.localRotation = Quaternion.AngleAxis (camRot, this.transform.forward);
			}
		}*/
		//Debug.Log (_currentSpeed.magnitude);
//
		if(Input.GetAxis("Vertical") > 0.3)
		{
			//If Moving Forward
			if (_onGround) {
				_maximumSpeed += Time.deltaTime * 2;
				_forceAcceleration += Time.deltaTime *8;
			}


			if (_currentSpeed.magnitude <= 11) {
				_timeToStop = 0.1f;
				_bobSpeed = 0.2f;
				_bobAmount = 0.025f;

			} else if (_currentSpeed.magnitude <= 25) {
				_timeToStop = 0.5f;
				_bobSpeed = 0.35f;
				_bobAmount = 0.032f;

			} else if (_currentSpeed.magnitude <= 38) {
				_timeToStop = 0.64f;
				_bobSpeed = 0.45f;
				_bobAmount = 0.04f;
			}
			else {
				_bobSpeed = 0.55f;
				_bobAmount = 0.06f;
			}

		}
		else if (Input.GetAxis("Vertical") < -0.3)
		{
			//If Moving Backwards
			if (_onGround) {
				_maximumSpeed += Time.deltaTime;
				_forceAcceleration += Time.deltaTime * 3;
			}

			if (_currentSpeed.magnitude <= 11) {
				_timeToStop = 0.1f;
			} else if (_currentSpeed.magnitude <= 25) {
				_timeToStop = 0.3f;

			} else if (_currentSpeed.magnitude > 25) {
				_timeToStop = 0.64f;
			}
		}
		else
		{
			if (_onGround && _currentSpeed.magnitude < 1) {
				//Complete Stop.
				_maximumSpeed = 10f;
				_forceAcceleration = 5640f;
				_timeToStop = 0.2f;
			}
			if (cam.fieldOfView > 60) {
				cam.fieldOfView -= Time.deltaTime*3;
			}

		}

		if (_currentSpeed.magnitude < 3) {
			//_maximumSpeed = 8;
			//_forceAcceleration = 5640f;
			//_timeToStop = 0.2f;
		}



		if (_currentSpeed.magnitude > _maximumSpeed)
		{
			_currentSpeed = _currentSpeed.normalized;
			_currentSpeed *= _maximumSpeed;

		}

		if (_maximumSpeed >= 50) {

			_maximumSpeed = 50;
			_timeToStop = 0.9f;
		}

		Vector3 v = GetComponent<Rigidbody>().velocity;
		v.x = _currentSpeed.x;
		v.z = _currentSpeed.y;

		if (_onGround) {

			v.x =  Mathf.SmoothDamp(v.x, 0f, ref _timeToStopVolx, _timeToStop);
			v.z =  Mathf.SmoothDamp(v.z, 0f, ref _timeToStopVolz, _timeToStop);
		}

		GetComponent<Rigidbody>().velocity = v;

		if (Input.GetAxis("Vertical") > 0) {
		if (_onGround)
		{
			GetComponent<Rigidbody>().useGravity = false;
			GetComponent<Rigidbody>().AddRelativeForce(Input.GetAxis("Horizontal") * _forceAcceleration * Time.deltaTime, 0, Input.GetAxis("Vertical") * _forceAcceleration * Time.deltaTime);
		}
		else
		{
			GetComponent<Rigidbody>().useGravity = true;
			GetComponent<Rigidbody>().AddRelativeForce(Input.GetAxis("Horizontal") * _forceAcceleration * airControlRatio * Time.deltaTime, 0, Input.GetAxis("Vertical") * _forceAcceleration * airControlRatio * Time.deltaTime);
		}
		}
		else {
			if (_onGround)
			{
				GetComponent<Rigidbody>().useGravity = false;
				GetComponent<Rigidbody>().AddRelativeForce(Input.GetAxis("Horizontal") * _forceAcceleration/1.5f * Time.deltaTime, 0, Input.GetAxis("Vertical") * _forceAcceleration/1.5f * Time.deltaTime);
			}
			else
			{
				GetComponent<Rigidbody>().useGravity = true;
				GetComponent<Rigidbody>().AddRelativeForce(Input.GetAxis("Horizontal") * _forceAcceleration * airControlRatio * Time.deltaTime, 0, Input.GetAxis("Vertical") * _forceAcceleration * airControlRatio * Time.deltaTime);
			}
		}
	}

	void FloorMeasure() {
		RaycastHit hitDown = new RaycastHit();
		RaycastHit hitUp = new RaycastHit();


		if (Physics.Raycast(transform.position, -Vector3.up, out hitDown)) {
			Debug.Log("Floor Distance:" + hitDown.distance);
		}

		if (Physics.Raycast(transform.position, Vector3.up, out hitUp)) {
			Debug.Log("Ceiling Distance:" + hitUp.distance);
		}
	}

	void flipGrav() {

		if (!_flip) { //UP
			if (_onGround) {

				GetComponent<Rigidbody> ().AddForce (0, -verticalAddForce, 0);

				Physics.gravity = new Vector3 (0, -9.81f, 0);
				if (!flipInit) {
					flipInit = true;
				}
			}
			else {
				Physics.gravity = new Vector3 (0, -15.81f, 0);
			}

		}
		else { //DOWN
			if (_onGround) {
				GetComponent<Rigidbody> ().AddForce (0, verticalAddForce, 0);
					Physics.gravity = new Vector3 (0, 9.81f, 0);
			}
			else {
				Physics.gravity = new Vector3 (0, 15.81f, 0);
			}


		}

	}

	void Update()
	{
		FloorMeasure();
		if (_flip) {
			if (myRot > -0.8) {
				myRot -= Time.deltaTime*3;
			}
			if (myRot < -0.76) {
				myRot = -0.8f;
			}
		}
		else {
			if (myRot < 0.8) {
				myRot += Time.deltaTime*3;
			}
			if (myRot > 0.76) {
				myRot =  0.8f;
			}
		}

cam.GetComponent<carHeadBob>().midpoint= myRot;

		/*if (Input.GetButton("Jump")) {
			_hardFall = true;
		}
		else {
			_hardFall = false;
		}*/

		//Debug.Log(GetComponent<Rigidbody>().velocity.y);

		if (Input.GetMouseButtonDown(0)) {
				if (!_flip)
					_flip = true;
				else
					_flip = false;

				flipGrav();

		}

		if (Input.GetMouseButton(0)){
			_mouseState = true;
		}
		else {
			_mouseState = false;
		}
		_hardFall = _mouseState;

		if (Input.GetButtonDown ("Jump")) {

			if (!_flip) { //If _flip DOWN
				if (_onGround)
				{ //If on the floor.
					if (!_hardFall) {
						//Boost off the Ground.


						_hardFall = true;
					}
				}
				else
				{ //If not on the floor.
					if (_hardFall) {
						//If holding the _flipKey, increase Drop Speed.

					}
					else {
						//If not holding the _flipKey, continue Drop

					}
				}


				_flip = true;
			}
			else { //If _flip UP
				GetComponent<Rigidbody> ().AddForce (0, -verticalAddForce, 0);

				_flip = false;
			}
		}
	}

	void OnCollisionStay (Collision collision)
	{
		foreach (ContactPoint contact in collision.contacts)
		{
			if (Vector3.Angle(contact.normal, Vector3.up) < maxSlope)
			{

				_hardFall = false;
				if (!_onGround) {
					cam.GetComponent<carHeadBob>().Landing();
					_onGround = true;
				}

			}
		}

		if (CollisionFlags.Above != 0) {
			if (!_onGround) {
				cam.GetComponent<carHeadBob>().Landing();
				_onGround = true;
			}
		}
	}

	void OnCollisionExit()
	{
		_onGround = false;
	}
}
