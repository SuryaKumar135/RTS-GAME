using System.Collections.Generic;
using UnityEngine;

namespace RTSGame
{
    public class SelectionManager : MonoBehaviour
    {
        [Header("Selection Box UI")]
        [SerializeField] private RectTransform selectionBox;
        [SerializeField] private Canvas canvas;

        [Header("Layers")]
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private LayerMask unitLayer;

        private Vector2 startPos;
        private Vector2 currentPos;
        private bool isDragging = false;

        public List<UnitBehaviour> selectedUnits = new List<UnitBehaviour>();

        private void Update()
        {
            HandleSelectionInput();
        }

        private void HandleSelectionInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                startPos = Input.mousePosition;
                isDragging = false;
                selectionBox.gameObject.SetActive(true);
            }

            if (Input.GetKey(KeyCode.Mouse0))
            {
                currentPos = Input.mousePosition;
                if (Vector2.Distance(startPos, currentPos) > 10f)
                {
                    isDragging = true;
                    UpdateSelectionBox();
                }
            }

            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                selectionBox.gameObject.SetActive(false);

                if (isDragging)
                {
                    DragSelectUnits();
                }
                else
                {
                    SingleClickSelect();
                }

                isDragging = false;
            }

            if (Input.GetMouseButtonUp(1))
            {
                if (isDragging) return;

                if (selectedUnits.Count > 0 && TryGetGroundPoint(out Vector3 groundPoint))
                {
                    foreach (var unit in selectedUnits)
                    {
                        unit.ExecuteAction(groundPoint);
                        unit.OnDeselected();
                    }
                }
                else
                {
                    SingleClickSelect();
                }
            }


        }

        private void UpdateSelectionBox()
        {
            Vector2 startLocal, currentLocal;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                startPos,
                canvas.worldCamera,
                out startLocal);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                currentPos,
                canvas.worldCamera,
                out currentLocal);

            Vector2 size = currentLocal - startLocal;

            selectionBox.anchoredPosition = startLocal;
            selectionBox.sizeDelta = new Vector2(Mathf.Abs(size.x), Mathf.Abs(size.y));
            selectionBox.pivot = new Vector2(size.x >= 0 ? 0 : 1, size.y >= 0 ? 0 : 1);
        }

        private void DragSelectUnits()
        {
            selectedUnits.Clear();
            Vector2 min = Vector2.Min(startPos, currentPos);
            Vector2 max = Vector2.Max(startPos, currentPos);

            foreach (UnitBehaviour unit in FindObjectsOfType<UnitBehaviour>())
            {
                Vector3 screenPos = Camera.main.WorldToScreenPoint(unit.transform.position);
                if (screenPos.x >= min.x && screenPos.x <= max.x &&
                    screenPos.y >= min.y && screenPos.y <= max.y)
                {
                    selectedUnits.Add(unit);
                    unit.GetComponent<SelectableObject>().OnSelected();
                }
                else
                {
                    unit.GetComponent<SelectableObject>().OnDeselected();
                }
            }
        }

        private void SingleClickSelect()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, unitLayer))
            {
                UnitBehaviour unit = hit.collider.GetComponent<UnitBehaviour>();
                if (unit != null)
                {
                    DeselectAll();
                    selectedUnits.Add(unit);
                    unit.GetComponent<SelectableObject>().OnSelected();
                }
            }
            else
            {
                DeselectAll();
            }
        }

        private void DeselectAll()
        {
            foreach (var unit in selectedUnits)
            {
                unit.GetComponent<SelectableObject>().OnDeselected();
            }
            selectedUnits.Clear();
        }

        private bool TryGetGroundPoint(out Vector3 point)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, groundLayer))
            {
                point = hit.point;
                if (hit.transform.TryGetComponent(out BuildingBehaviour component) && selectedUnits.Count > 0)
                {
                    foreach (var unit in selectedUnits) {

                        component.AssignBuilder(unit);
                            
                    }
                }
                return true;
            }

            point = Vector3.zero;
            return false;
        }
    }
}
