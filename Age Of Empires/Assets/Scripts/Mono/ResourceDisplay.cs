using UnityEngine;
using TMPro; // Optional if using TextMeshPro

namespace Platformer
{
    public class ResourceDisplay : MonoBehaviour
    {
        public string resourceName;
        public TMP_Text displayText; // Assign in inspector

        public void OnResourceChanged(int value)
        {
            displayText.text = $"{resourceName}: {value}";
            Debug.Log($"{resourceName} updated to {value}");
        }
    }
}
