using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RTSGame
{
    public class SelectionManager : MonoBehaviour
    {

        public static SelectionManager Instance;
        public enum InputMode
        {
            Selection,
            CameraMove
        }

        [Header("Selection UI")]
        [SerializeField] private RectTransform selectionBox;
        [SerializeField] private Canvas canvas;

        [Header("Layers")]
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private LayerMask unitLayer;
        [SerializeField] private LayerMask buildingLayer;

        [Header("Camera Settings")]
        public float dragSpeed = 20f;

        [Header("Zoom")]
        public float zoomSpeed = 500f;
        public float minZoom = 10f;
        public float maxZoom = 50f;

        private Vector2 startPos;
        private Vector2 currentPos;
        private bool isDragging = false;

        private Transform cameraTransform;
        private InputMode currentMode = InputMode.Selection;
        private Action currentUpdate;

        public List<UnitBehaviour> selectedUnits = new List<UnitBehaviour>();

        private void Awake()
        {
            if(Instance==null)
            {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(Instance);
            }
        }

        void Start()
        {
            cameraTransform = Camera.main.transform;

            if (selectionBox != null)
                selectionBox.gameObject.SetActive(false);

            SetInputMode((int)InputMode.Selection);
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
                ResetSelectionBox();

            currentUpdate?.Invoke();
            HandleZoom();
        }

        public void SetInputMode(int modeIndex)
        {
            currentMode = (InputMode)modeIndex;

            currentUpdate = currentMode == InputMode.Selection
                ? (Action)HandleSelectionInput
                : HandleCameraDrag;
        }

        private void HandleCameraDrag()
        {
            if (Input.GetMouseButton(0))
            {
                float moveX = -Input.GetAxis("Mouse X") * dragSpeed * Time.deltaTime;
                float moveZ = -Input.GetAxis("Mouse Y") * dragSpeed * Time.deltaTime;

                cameraTransform.position += new Vector3(moveX, 0, moveZ);
            }
        }

        private void HandleZoom()
        {
            Vector3 pos = cameraTransform.position;
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            pos.y -= scroll * zoomSpeed * Time.deltaTime;
            pos.y = Mathf.Clamp(pos.y, minZoom, maxZoom);
            cameraTransform.position = pos;
        }

        private void HandleSelectionInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                startPos = Input.mousePosition;
                isDragging = false;
                selectionBox.gameObject.SetActive(true);
            }

            if (Input.GetMouseButton(0))
            {
                currentPos = Input.mousePosition;
                if (Vector2.Distance(startPos, currentPos) > 10f)
                {
                    isDragging = true;
                    UpdateSelectionBox();
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                selectionBox.gameObject.SetActive(false);
                if (isDragging) DragSelectUnits();
                else SingleClickSelect();
                isDragging = false;
            }

            if (Input.GetMouseButtonUp(1) && selectedUnits.Count > 0)
            {
                HandleRightClick();
            }
        }

        private void HandleRightClick()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
            {
                // 1. Building
                BuildingBehaviour building = hit.collider.GetComponent<BuildingBehaviour>();
                if (building != null)
                {
                    foreach (var unit in selectedUnits)
                        unit.ExecuteBuildingAction(building);
                    return;
                }

                // 2. Ground
                if (((1 << hit.collider.gameObject.layer) & groundLayer) != 0)
                {
                    foreach (var unit in selectedUnits)
                        unit.MoveUnit(hit.point);
                }
            }
        }

        private void UpdateSelectionBox()
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, startPos, canvas.worldCamera, out Vector2 startLocal);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, currentPos, canvas.worldCamera, out Vector2 currentLocal);

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
                    unit.OnSelected();
                }
                else
                {
                    unit.OnDeselected();
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
                    unit.OnSelected();
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
                unit.OnDeselected();

            selectedUnits.Clear();
        }

        private void ResetSelectionBox()
        {
            if (selectionBox != null)
            {
                selectionBox.sizeDelta = Vector2.zero;
                selectionBox.anchoredPosition = Vector2.zero;
                selectionBox.gameObject.SetActive(false);
            }
        }
    }
}
