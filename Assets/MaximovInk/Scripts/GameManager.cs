using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Hotbar hotbar { get; private set; }
    public Player player { get; private set; }

    public LayerMask GroundMask;

    private void Awake()
    {
        instance = this;


        hotbar = FindObjectOfType<Hotbar>();
        player = FindObjectOfType<Player>();
    }

    private void Start()
    {
    }
}

