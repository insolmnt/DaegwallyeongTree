using UnityEngine;

public class FreeCamera : MonoBehaviour
{
	public string DataKey = "CameraPosition";

	public Camera Camera;
	public float lookSpeed = 5f;
	public float moveSpeed = 5f;
	public float sprintSpeed = 50f;

	[System.Serializable]
	public class FreeCameraData
	{
		public Vector3 Position;
		public Vector3 Rotation;
		public float Fov;
	}
	public FreeCameraData Data;
	private Vector3 DefaultPosition;
	private Vector3 DefaultRotation;
	private float DefaultFps;

	public bool m_inputCaptured
    {
		set; get;
    }
	float	m_yaw;
	float	m_pitch;

	void CaptureInput() {
		m_inputCaptured = true;

		m_yaw = transform.eulerAngles.y;
		m_pitch = transform.eulerAngles.x;
	}

	void ReleaseInput() {
		m_inputCaptured = false;
	}


	void Start()
	{
		DefaultPosition = transform.position;
		DefaultRotation = transform.localEulerAngles;
		DefaultFps = Camera.fieldOfView;
		Load();
	}

	[ContextMenu("Save")]
	public void Save()
	{
		Data.Position = transform.position;
		Data.Rotation = transform.localEulerAngles;
		Data.Fov = Camera.fieldOfView;

		DataManager.SetData<FreeCameraData>(DataKey, Data);

		UiManager.Instance.ShowMessage("저장완료");
	}
	[ContextMenu("Load")]
	public void Load()
	{
		Data = DataManager.GetData<FreeCameraData>(DataKey);
		if (Data == null)
		{
			Data = new FreeCameraData();
			Data.Position = DefaultPosition;
			Data.Rotation = DefaultRotation;
			Data.Fov = DefaultFps;
		}


		transform.position = Data.Position;
		transform.localEulerAngles = Data.Rotation;
		Camera.fieldOfView = Data.Fov;

		m_yaw = transform.eulerAngles.y;
		m_pitch = transform.eulerAngles.x;
	}
	public void Home()
	{
		Data.Position = DefaultPosition;
		Data.Rotation = DefaultRotation;
		Data.Fov = DefaultFps;

		transform.position = Data.Position;
		transform.localEulerAngles = Data.Rotation;
		Camera.fieldOfView = Data.Fov;

		m_yaw = transform.eulerAngles.y;
		m_pitch = transform.eulerAngles.x;

		UiManager.Instance.ShowMessage("되돌리기");
		//if (UIManager.Instance != null)
		//{
		//	UIManager.Instance.ShowMessage("기본값 복원 (End키를 눌러서 저장 필요)");
		//}
	}


	void Update() {
        if (Input.GetKeyDown(KeyCode.F9))
        {
			m_inputCaptured = !m_inputCaptured;
            if (m_inputCaptured)
            {
				CaptureInput();

				if (UiManager.Instance != null)
				{
					UiManager.Instance.ShowMessage("카메라 이동 활성화");
				}
			}
            else
            {
				ReleaseInput();
				if (UiManager.Instance != null)
				{
					UiManager.Instance.ShowMessage("카메라 이동 비활성화");
				}

			}
		}
		if (m_inputCaptured)
		{
			if (Input.GetKeyDown(KeyCode.Home))
			{
				Home();
			}
			if (Input.GetKeyDown(KeyCode.End))
			{
				Save();
			}
			float scroll = Input.GetAxis("Mouse ScrollWheel") * 30;
			if (scroll != 0)
			{
				Camera.fieldOfView = Mathf.Clamp(Camera.fieldOfView - scroll, 10, 120);
			}

		}

		if (!m_inputCaptured)
			return;

        if (Input.GetMouseButton(1))
		{
			var rotStrafe = Input.GetAxis("Mouse X");
			var rotFwd = Input.GetAxis("Mouse Y");

			m_yaw = (m_yaw + lookSpeed * rotStrafe) % 360f;
			m_pitch = (m_pitch - lookSpeed * rotFwd) % 360f;
			transform.rotation = Quaternion.AngleAxis(m_yaw, Vector3.up) * Quaternion.AngleAxis(m_pitch, Vector3.right);

			var speed = Time.deltaTime * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed);
			var forward = speed * Input.GetAxis("Vertical");
			var right = speed * Input.GetAxis("Horizontal");
			var up = speed * ((Input.GetKey(KeyCode.E) ? 1f : 0f) - (Input.GetKey(KeyCode.Q) ? 1f : 0f));
			transform.position += transform.forward * forward + transform.right * right + Vector3.up * up;
		}
	}
}
