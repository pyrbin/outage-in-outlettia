using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovementController : MonoBehaviour
{
    public float MovementSpeed = 3;
    public float JumpForce = 3;

    [SerializeField] private InputReader inputReader;
    private float movementDirection;
    private Rigidbody2D _rigidbody2D;

    // Start is called before the first frame update
    void Start()
    {
        inputReader.MoveEvent += (float dir) => movementDirection = dir;
        inputReader.JumpEvent += Jump;
        TryGetComponent<Rigidbody2D>(out _rigidbody2D);
    }

    // Update is called once per frame
    void Update()
    {
        if (movementDirection != 0)
            transform.position += new Vector3(movementDirection, 0, 0) * Time.deltaTime * MovementSpeed;
    }

    void Jump()
    {
        if (Mathf.Abs(_rigidbody2D.velocity.y) < 0.001f)
            _rigidbody2D.AddForce(new Vector2(0, JumpForce), ForceMode2D.Impulse);
    }

}
