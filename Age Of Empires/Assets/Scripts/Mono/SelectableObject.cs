using UnityEngine;



namespace RTSGame
{
    public class SelectableObject : MonoBehaviour
    {
        public MeshRenderer rend;
        private Color originalColor;
        [SerializeField] private Color selectedColor = Color.green;

        public void Awake()
        {
            rend = GetComponent<MeshRenderer>();
            originalColor = rend.material.color;
        }

        public void OnSelected()
        {
            rend.material.color = selectedColor;
        }

        public void OnDeselected()
        {
            rend.material.color = originalColor;
        }
    }
}
