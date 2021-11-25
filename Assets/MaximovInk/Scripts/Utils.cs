
using UnityEngine.EventSystems;

namespace MaximovInk
{
    public static class Utils
    {
        public static bool IsOverUI()
        {
            return EventSystem.current.IsPointerOverGameObject() || EventSystem.current.currentSelectedGameObject != null;
        }
    }
}
