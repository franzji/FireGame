using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AIManager : MonoBehaviour {

    public GlobalLevelData LvlData;
    private GameObject terrain;
    private GameObject AOE;

    // List of units currently visible on the screen. Automatically maintained by any GameObjects inheriting from GameUnit.
    [HideInInspector]
    public List<GameObject> unitsVisible;
    // Store the screen rectangle of the area being selected.
    private Rect selection;
    // Canvas that the SelectionBox Image sits on.
    [SerializeField]
    private Canvas canvas;
    // RectTransform of the SelectionBox Image.
    [SerializeField]
    private RectTransform selectionBoxRect;
    // List of selected units.
    [HideInInspector]
    public List<GameObject> unitsSelected = new List<GameObject>();

    // Use this for initialization
    void Start () {
        AOE = GameObject.Find("AOE");
        AOE.SetActive(false);
        selectionBoxRect.gameObject.GetComponent<Image>().enabled = false;
        terrain = GameObject.Find("Terrain");
    }
	
	// Update is called once per frame
	void Update () {

        //START DRAGGING logic
        if (Input.GetMouseButtonDown(0) && (LvlData.PlayerState == State.BeforeSelected || LvlData.PlayerState == State.UnitSelected))
        {
            if (!Input.GetButton("Fast Pan")) // Left shift by default
            {
                foreach (GameObject unit in unitsSelected)
                    unit.GetComponent<GameUnit>().DeselectUnit();
                unitsSelected.Clear();
            }
            // Set up selection to be 0x0 rectangle at mousePosition.
            selection.min = Input.mousePosition;
            selection.max = selection.min;
            selectionBoxRect.gameObject.GetComponent<Image>().enabled = true;
            LvlData.PlayerState = State.Dragging;
        }

        //DRAGGING logic
        if (Input.GetMouseButton(0) && LvlData.PlayerState == State.Dragging)
        {
            // Modify selection max to be other corner of rectangle.
            selection.max = Input.mousePosition;
            // Re-set lower-left corner of SelectionBox to selection.min.
            Vector2 screenToTransformPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, selection.min, canvas.worldCamera, out screenToTransformPoint);
            selectionBoxRect.anchoredPosition = screenToTransformPoint;
            // Set upper-right corner of SelectionBox to selection.max via subtracting the upper right corner from the lower left corner.
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, selection.max, canvas.worldCamera, out screenToTransformPoint);
            selectionBoxRect.sizeDelta = screenToTransformPoint - selectionBoxRect.anchoredPosition;
            // Since RectTransform's width and height must be greater than 0, if the width is less than 0 we move the anchored position (-width) units left and flip the sign on width.
            if (selection.width < 0)
            {
                selectionBoxRect.anchoredPosition = new Vector2(
                    selectionBoxRect.anchoredPosition.x + selectionBoxRect.sizeDelta.x,
                    selectionBoxRect.anchoredPosition.y
                );
                selectionBoxRect.sizeDelta = new Vector2(
                    -selectionBoxRect.sizeDelta.x,
                    selectionBoxRect.sizeDelta.y
                );
            }
            // Since RectTransform's width and height must be greater than 0, if the height is less than 0 we move the anchored position (-height) units down and flip the sign on height.
            if (selection.height < 0)
            {
                selectionBoxRect.anchoredPosition = new Vector2(
                    selectionBoxRect.anchoredPosition.x,
                    selectionBoxRect.anchoredPosition.y + selectionBoxRect.sizeDelta.y
                );
                selectionBoxRect.sizeDelta = new Vector2(
                    selectionBoxRect.sizeDelta.x,
                    -selectionBoxRect.sizeDelta.y
                );
            }
        }
        //STOP DRAGGING logic to play gameunits into selection and stop dragging.
        if (Input.GetMouseButtonUp(0) && LvlData.PlayerState == State.Dragging)
        {
            selectionBoxRect.gameObject.GetComponent<Image>().enabled = false;
            foreach (GameObject unit in unitsVisible)
            {
                //Debug.Log(Camera.main.WorldToScreenPoint(unit.transform.position));
                //Debug.Log(selection.Contains(Camera.main.WorldToScreenPoint(unit.transform.position), true));
                if (selection.Contains(Camera.main.WorldToScreenPoint(unit.transform.position), true))
                    unit.GetComponent<GameUnit>().SelectUnit();
            }

            if (unitsSelected.Count > 0)
                LvlData.PlayerState = State.UnitSelected;
            else
                LvlData.PlayerState = State.BeforeSelected;
        }


        //SET new location for units
        if (Input.GetMouseButtonDown(1) && LvlData.PlayerState == State.UnitSelected)
        {

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            foreach (GameObject unit in unitsSelected)
            {
                if (terrain.GetComponent<Collider>().Raycast(ray, out hit, Mathf.Infinity))
                {
                    unit.GetComponent<GroundCrew>().SetHome(hit.point);
                    unit.GetComponent<GameUnit>().DeselectUnit();
                }
            }
            unitsSelected.Clear();

            LvlData.PlayerState = State.BeforeSelected;
        }

        if (LvlData.PlayerState != State.UnitSelected)
        {
            //remove UI indicator
            AOE.SetActive(false);
        }
            //MOVE UI
        if (unitsSelected.Count > 0)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (terrain.GetComponent<Collider>().Raycast(ray, out hit, Mathf.Infinity))
            {
                //spawn ghost and set its transform position to the hit point.
                AOE.transform.position = hit.point;
            }
            AOE.SetActive(true);
        }
    }
}