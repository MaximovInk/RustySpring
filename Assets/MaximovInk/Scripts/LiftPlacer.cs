
using UnityEngine;

public static class LiftPlacer
    {

    private static Lift Lift;

    public static void BeginPlace() {
        if (Lift == null)
        {
            var prefab = Resources.Load<GameObject>("Prefabs/Lift");
            Lift = Object.Instantiate(prefab).GetComponent<Lift>();
        }

        Lift.State = LiftState.Placing;
    }

    public static void TerminatePlace() {
        if(Lift.State == LiftState.Placing)
        Object.Destroy(Lift.gameObject);
    }
}

