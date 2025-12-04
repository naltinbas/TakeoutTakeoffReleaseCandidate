using UnityEngine;
using UnityEngine.Serialization;

public class CameraMovement : MonoBehaviour
{

    private GameObject _player;
    private AirplaneController _airplaneController;
    [SerializeField] private float followSpeed = 5f;
    [SerializeField] private float rotationSmoothTime = 0.25f;
    void Start()
    {
        _player  = GameObject.FindGameObjectWithTag("Player");
        _airplaneController = _player.GetComponent<AirplaneController>();
    }

    void LateUpdate()
    {
        if (!_player) return;

        Vector3 offset = new Vector3(0, 4f, -7f);
        float localAirplaneYaw = _airplaneController.transform.localEulerAngles.y;

        float targetCameraYaw =
            localAirplaneYaw is > 90f and < 270f ? 180f :
            localAirplaneYaw is > 90f and < 180f ? 179f :
            localAirplaneYaw is > 270f and < 360f ? 359f :
            1f;

        if (localAirplaneYaw is > 90f and < 270f)
            offset.z *= -1;

        Vector3 targetPos = _player.transform.position + offset;
        transform.position = Vector3.Slerp(transform.position, targetPos, followSpeed * Time.deltaTime);

        float currentYaw = transform.localEulerAngles.y;
        float deltaYaw = Mathf.DeltaAngle(currentYaw, targetCameraYaw); 
        // Debug.Log("Cur: " + currentYaw + ", Target: " + targetCameraYaw + ", Delta: " + deltaYaw);
        float rotationSpeed = followSpeed * 50f * Time.deltaTime; 
        float newYaw = currentYaw + Mathf.Clamp(deltaYaw, -rotationSpeed, rotationSpeed);

        Quaternion targetRot = Quaternion.Euler(30f, newYaw, 0f);
        transform.localRotation = targetRot;
    }
}