using UnityEngine;

[RequireComponent(typeof(MovementController), typeof(WireHolder))]
public class PlayerInput : MonoBehaviour
{
    private WireHolder WireHolder;
    private MovementController Movement;
    private Interactor Interactor;

    [SerializeField]
    private InputReader InputReader;

    void Start()
    {
        InputReader.EnableGameplayInput();

        TryGetComponent(out WireHolder);
        TryGetComponent(out Movement);
        TryGetComponent(out Interactor);

        InputReader.MoveEvent += (float dir) =>
        {
            Movement.Direction = dir;
        };

        InputReader.InteractEvent += () =>
        {
            Interactor.InteractNearest();
        };

        InputReader.JumpEvent += () =>
        {
            if (!WireHolder.IsHanging)
                Movement.Jump();
        };

        InputReader.HoldEvent += () =>
        {
            WireHolder.ToggleHold();
        };

        InputReader.RetractEvent += () =>
        {
            if (WireHolder.IsHanging)
                WireHolder.ToggleRetract();
        };

        InputReader.UsePowerEvent += () =>
        {
            Movement.Boost();
        };
    }

    // Update is called once per frame
    void Update()
    {

    }
}
