using UnityEngine;

public class PeopleJump : MonoBehaviour
{
    [Header("Jump Settings")]
    public float jumpHeight = 0.5f;    // how high the person jumps
    public float jumpSpeed = 3f;       // how fast the person jumps

    private Vector3 _baseLocalPosition; // starting position of the person
    private float _randomOffset;        // random timing offset so not all jump in sync

    void Start()
    {
        // Save the starting local position so jumps happen around it
        _baseLocalPosition = transform.localPosition;

        // Randomize the offset to desync different people
        _randomOffset = Random.Range(0f, Mathf.PI * 2f);
    }

    void Update()
    {
        // Make it jump smoothly up and down with sine wave, desynced by _randomOffset
        float newY = _baseLocalPosition.y + Mathf.Abs(Mathf.Sin(Time.time * jumpSpeed + _randomOffset)) * jumpHeight;
        transform.localPosition = new Vector3(
            _baseLocalPosition.x,
            newY,
            _baseLocalPosition.z
        );
    }
}
