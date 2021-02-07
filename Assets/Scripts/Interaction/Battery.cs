
using UnityEngine;

public class Battery : Interactable
{
    [SerializeField] private Transform Hint;
    public bool Taken { get; private set; } = false;
    public bool Seen { get; private set; } = false;
    public Interactor interactor;

    protected override void InRange(Interactor user)
    {
        Seen = !Taken;
        interactor = user;
    }

    protected override void LostRange(Interactor user)
    {
        Seen = false;
        Hint.gameObject.SetActive(false);
        interactor = null;
    }

    void Update()
    {
        if (Seen && !Taken && interactor && interactor.Nearest)
        {
            Hint.gameObject.SetActive(!Taken && interactor.Nearest == this as Interactable);
        }
    }

    protected override void OnInteract(Interactor user)
    {
        if (user.TryGetComponent<MovementController>(out MovementController mc))
        {
            mc.HasBoost = true;
            Taken = true;
            this.GetComponent<Collider2D>().enabled = false;
            Invoke("DeleteSelf", 0.2f);
        }
    }


    protected void DeleteSelf()
    {
        DestroyImmediate(this.gameObject);
    }
}
