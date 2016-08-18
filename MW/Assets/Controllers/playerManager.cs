using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class playerManager : MonoBehaviour {

	public Text speedUI;

	public static playerManager player;

	public enum WalkSetting {Code, Custom};

	public Transform CachedTransform { get; private set; }

	public float SpeedFactor
	{
		get
		{
			return _currentSpeed.magnitude;
		}
	}

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
	private float _minimumSpeed;
	public float minimumSpeed {get {return _minimumSpeed;} set {_minimumSpeed = value;}}
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
	private float originalDropVol;
	public float maxSlope;
	[Space(10)]
	[Header ("Player Checks")]
	[SerializeField]
	 private bool _onGround;
	public bool onGround {get {return _onGround;}}
	[SerializeField]
	 private bool _flip;
	public bool flip {get {return _flip;}}
	[SerializeField]
	 private bool _mouseState;
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

	private float _floorCaster;
	public float floorCaster {get {return _floorCaster;}}

	private float _ceilCaster;
	public float ceilCast {get {return _ceilCaster;}}

	public GameObject clone;

	CursorLockMode wantedMode;

	void Awake()
	{
		CachedTransform = transform;
		wantedMode = CursorLockMode.Locked;
		SetCursorState ();
		player = this;

		if (walkSetting == WalkSetting.Code)
		{
			_forceAcceleration = 6440f;
			_minimumSpeed = 7f;
			_maximumSpeed = _minimumSpeed;
			_timeToStop = 0.2f;
			airControlRatio = 1f;
			verticalAddForce = 1000f;
			dropVelocity = 100f;
			maxSlope = 60f;
		}

		originalDropVol = dropVelocity;
		timer = 0;
		_flip = false;
	}

	void SetCursorState ()
	{
		Cursor.lockState = wantedMode;
		// Hide cursor when locking
		Cursor.visible = (CursorLockMode.Locked != wantedMode);
	}

	void FixedUpdate()
	{
		_currentSpeed = new Vector2(GetComponent<Rigidbody>().velocity.x,GetComponent<Rigidbody>().velocity.z);
		timer += Time.deltaTime;

		if (_currentSpeed.magnitude < minimumSpeed) {
			_maximumSpeed = minimumSpeed;
		}

		if (timer >= 0.1)
		{
			timer = 0;
			Instantiate(clone);
		}

		if(Input.GetAxis("Vertical") > 0.3)
		{
			if (_currentSpeed.magnitude <= minimumSpeed+1)
			{
				_timeToStop = 0.1f;
				_bobSpeed = 0.2f;
				_bobAmount = 0.025f;

			}
			else if (_currentSpeed.magnitude <= 25)
			{
				_timeToStop = 0.5f;
				_bobSpeed = 0.35f;
				_bobAmount = 0.032f;

			}
			else if (_currentSpeed.magnitude <= 38)
			{
				_timeToStop = 0.64f;
				_bobSpeed = 0.45f;
				_bobAmount = 0.04f;
			}
			else
			{
				_bobSpeed = 0.55f;
				_bobAmount = 0.06f;
			}
		}
		else if (Input.GetAxis("Vertical") < -0.3)
		{
			if (_currentSpeed.magnitude <= minimumSpeed+1)
			{
				_timeToStop = 0.1f;
			}
			else if (_currentSpeed.magnitude <= 25)
			{
				_timeToStop = 0.3f;
			}
			else if (_currentSpeed.magnitude > 25)
			{
				_timeToStop = 0.64f;
			}
		}
		else
		{
			if (_onGround && _currentSpeed.magnitude < minimumSpeed);
			{
				//Complete Stop.
				eBrake();
			}
		}

		if (_currentSpeed.magnitude > _maximumSpeed)
		{
			_currentSpeed = _currentSpeed.normalized;
			_currentSpeed *= _maximumSpeed;

		}

		if (_maximumSpeed >= 50)
		{
			maxSpeed();
		}

		Vector3 v = GetComponent<Rigidbody>().velocity;
		v.x = _currentSpeed.x;
		v.z = _currentSpeed.y;

		if (_onGround)
		{
			v.x =  Mathf.SmoothDamp(v.x, 0f, ref _timeToStopVolx, _timeToStop);
			v.z =  Mathf.SmoothDamp(v.z, 0f, ref _timeToStopVolz, _timeToStop);
		}

		GetComponent<Rigidbody>().velocity = v;

		if (Input.GetAxis("Vertical") > 0)
		{
			if (_onGround)
			{
				GetComponent<Rigidbody>().AddRelativeForce(Input.GetAxis("Horizontal") * _forceAcceleration * Time.deltaTime, 0, Input.GetAxis("Vertical") * _forceAcceleration * Time.deltaTime);
			}
			else
			{
				GetComponent<Rigidbody>().AddRelativeForce(Input.GetAxis("Horizontal") * _forceAcceleration * airControlRatio * Time.deltaTime, 0, Input.GetAxis("Vertical") * _forceAcceleration * airControlRatio * Time.deltaTime);
			}
		}
		else
		{
				if (_onGround)
				{
					GetComponent<Rigidbody>().AddRelativeForce(Input.GetAxis("Horizontal") * _forceAcceleration/1.5f * Time.deltaTime, 0, Input.GetAxis("Vertical") * _forceAcceleration/1.5f * Time.deltaTime);
				}
				else
				{
					GetComponent<Rigidbody>().AddRelativeForce(Input.GetAxis("Horizontal") * _forceAcceleration * airControlRatio * Time.deltaTime, 0, Input.GetAxis("Vertical") * _forceAcceleration * airControlRatio * Time.deltaTime);
				}
		}
	}

	void accelerate(float speedInc, float accelInc)
	{
		_maximumSpeed += Time.deltaTime * speedInc;
		_forceAcceleration += Time.deltaTime * accelInc;
	}

	void decelerate(float speedInc, float accelInc)
	{
		_maximumSpeed -= Time.deltaTime * speedInc;
		_forceAcceleration -= Time.deltaTime * accelInc;
	}

	void eBrake()
	{
		if (_onGround) {
			_maximumSpeed = _minimumSpeed;
		}
		_forceAcceleration = 5640f;
		_timeToStop = 0.2f;
	}

	void maxSpeed()
	{
		_maximumSpeed = 50;
		_timeToStop = 0.9f;
	}

	void floorCheck()
	{
		RaycastHit hitDown = new RaycastHit();
		RaycastHit hitUp = new RaycastHit();

			if (Physics.Raycast(transform.position, -Vector3.up, out hitDown))
			{
				_floorCaster = hitDown.distance;
				//Debug.Log("FLOOR: " + _floorCaster);
			}

			if (Physics.Raycast(transform.position, Vector3.up, out hitUp))
			{
					_ceilCaster = hitUp.distance;
					//Debug.Log("CEILING: " + _ceilCaster);
			}
	}


	void flipGrav()
	{
		if (!_flip)
		{ //UP
			if (_onGround)
			{
				GetComponent<Rigidbody>().AddForce(0, -verticalAddForce, 0);
				Physics.gravity = new Vector3 (0, -15.81f, 0);
						}
			else
			{
				GetComponent<Rigidbody>().AddForce(0, -verticalAddForce/2, 0);
				Physics.gravity = new Vector3 (0, -15.81f, 0);
			}

		}
		else
		{ //DOWN
			if (_onGround)
			{
				GetComponent<Rigidbody>().AddForce(0, verticalAddForce, 0);
				Physics.gravity = new Vector3 (0, 15.81f, 0);
			}
			else
			{
				GetComponent<Rigidbody>().AddForce(0, verticalAddForce/2, 0);
				Physics.gravity = new Vector3 (0, 15.81f, 0);
			}
		}
		_onGround = false;
	}

	void camPosReset()
	{
		if (_flip)
		{
			if (myRot > -0.8)
				myRot -= Time.deltaTime*3;

			if (myRot < -0.76)
				myRot = -0.8f;
		}
		else
		{
			if (myRot < 0.8)
				myRot += Time.deltaTime*3;

			if (myRot > 0.76)
				myRot =  0.8f;
		}

		cam.GetComponent<carHeadBob>().midpoint= myRot;
	}

	void Update()
	{
		speedUI.text = "Speed:" + Mathf.Floor(_currentSpeed.magnitude).ToString();
		if (Input.GetKeyDown (KeyCode.Escape))
			Cursor.lockState = wantedMode = CursorLockMode.None;

		floorCheck();
		camPosReset();

		if (_mouseState)
		{
			//If Accelerating
			if (_onGround)
			{
				accelerate(3f,10f);
			}
			else
			{
				if (_flip)
				{
					GetComponent<Rigidbody> ().AddRelativeForce(0, dropVelocity, 0);
					dropVelocity += Time.deltaTime * 10;
					accelerate(.5f, 2f);
				}
				else
				{
					GetComponent<Rigidbody> ().AddRelativeForce (0, -dropVelocity, 0);
					dropVelocity -= Time.deltaTime * 10;
					accelerate(.5f, 2f);
				}
			}
		}
		else
		{
				if (_maximumSpeed > _minimumSpeed && _onGround)
				{
					decelerate(1.5f, 15f);
				}
		}

		if (Input.GetMouseButtonDown(0) && _flip)
		{
				_flip = false;
				flipGrav();
		}

		if (Input.GetMouseButtonDown (1) && !_flip)
		{
				_flip = true;
				flipGrav();
		}

		if (_flip)
		{
			if (Input.GetMouseButton(1))
				_mouseState = true;
			else
				_mouseState = false;
		}
		else
		{
			if (Input.GetMouseButton(0))
				_mouseState = true;
			else
				_mouseState = false;
		}
	}

	void onLanded(string tag)
	{
		cam.GetComponent<carHeadBob>().Landing();
		if (_flip)
			dropVelocity = -originalDropVol;
		else
			dropVelocity = originalDropVol;


		if (_mouseState && tag == "Floor")
		{
			Debug.Log("HARD LANDING!");
			if (_maximumSpeed > _minimumSpeed)
				_maximumSpeed -= _maximumSpeed/4;
		}
	}

	void OnCollisionStay (Collision collision)
	{
		foreach (ContactPoint contact in collision.contacts)
		{
			//Debug.Log(Vector3.Angle(contact.normal, Vector3.up));
		 	if (Vector3.Angle(contact.normal, Vector3.up) < maxSlope)
			{
				if (!_onGround)
				{
					cam.GetComponent<carHeadBob>().Landing();
					_onGround = true;
					GetComponent<Rigidbody>().useGravity = false;
				}
			}
		}

		if (CollisionFlags.Above != 0 && _flip)
		{
			if (!_onGround)
			{
				cam.GetComponent<carHeadBob>().Landing();
				GetComponent<Rigidbody>().useGravity = false;
				_onGround = true;
			}
		}
	}

	void OnCollisionExit()
	{
		_onGround = false;
		GetComponent<Rigidbody>().useGravity = true;
	}
}
