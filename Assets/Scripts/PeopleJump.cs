using UnityEngine;

public class PeopleJump : MonoBehaviour
{
    [Header("Jump Settings")]
    public float jumpHeight = 0.5f;    // how high the person jumps
    public float jumpSpeed = 3f;       // how fast the person jumps

    private Vector3 baseLocalPosition; // starting position of the person
    private float randomOffset;        // random timing offset so not all jump in sync

    void Start()
    {
        // Save the starting local position so jumps happen around it
        baseLocalPosition = transform.localPosition;

        // Randomize the offset to desync different people
        randomOffset = Random.Range(0f, Mathf.PI * 2f);
    }

    void Update()
    {
        // Make it jump smoothly up and down with sine wave, desynced by randomOffset
        float newY = baseLocalPosition.y + Mathf.Abs(Mathf.Sin(Time.time * jumpSpeed + randomOffset)) * jumpHeight;
        transform.localPosition = new Vector3(
            baseLocalPosition.x,
            newY,
            baseLocalPosition.z
        );
    }
}
