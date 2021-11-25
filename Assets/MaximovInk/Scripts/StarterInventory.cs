
using UnityEngine;

public class StarterInventory : MonoBehaviour
{
    private void Start()
    {
        GameManager.instance.hotbar.AddItem(new ItemData() {
            ID="Lift",
            Count = 1
        });
    }
}

