using UnityEngine;

[RequireComponent(typeof(MovementController), typeof(WireHolder))]
public class PlayerInput : MonoBehaviour
{
    private WireHolder WireHolder;
    private MovementController Movement;

    [SerializeField]
    private InputReader InputReader;

    void Start()
    {
        TryGetComponent(out WireHolder);
        TryGetComponent(out Movement);

        InputReader.MoveEvent += (float dir) =>
        {
            Movement.Direction = dir;
        };

        InputReader.JumpEvent += () =>
        {
            Movement.Jump();
        };

        InputReader.HoldEvent += () =>
        {
            WireHolder.ToggleHold();
        };
    }

    // Update is called once per frame
    void Update()
    {

    }
}
