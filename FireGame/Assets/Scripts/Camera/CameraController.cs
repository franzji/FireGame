using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraController : MonoBehaviour
{
    [SerializeField]
    private float panningSpeed = 3f;
    [SerializeField]
    private float fastPanMultiplier = 3f;
    [SerializeField]
    private float zoomSpeed = 3f;
    [SerializeField]
    private float rotationSpeed = 3f;

    public float minimumHeightAboveTerrain = 20f;

    private float maximumHeight = 195f;
    public float getMaximumHeight() { return maximumHeight; }

    public float minimumHeight {
        get { return terrain.terrainData.size.y + minimumHeightAboveTerrain; }
    }

    private Terrain terrain;
    public Terrain GetTerrain() { return terrain; }

    public float minBound { get { return boundsArray[0, 0].x; } }
    public float maxBound { get { return boundsArray[1, 0].x; } }
    private bool isCamInTopQtr, isCamInBottomQtr, isCamInLeftQtr, isCamInRightQtr;
    private Vector3[,] boundsArray;

    public void setMaximumHeight()
    {
        TerrainGenerator terrainGenerator = FindObjectOfType<TerrainGenerator>();
        TerrainGenerator.sizes size = terrainGenerator.getSize();
    }

    public void setTerrain()
    {
        GameObject terrainGameObject = GameObject.Find("Terrain");
        //Found terrain.
        if (terrainGameObject)
        {
            terrain = terrainGameObject.GetComponent<Terrain>();
        }
    }

    void Update ()
    {
        setTerrain();
        if (terrain)
        {
            setMaximumHeight();
            boundsArray = TerrainBounds.playerBounds(terrain);
            panCamera();
            zoomCamera();
            rotateCamera();
        }
    }

    private void panCamera()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        bool isFastPan = Input.GetButton("Fast Pan");

        Vector3 translateVector = new Vector3(horizontalInput, 0, verticalInput) * panningSpeed * Time.deltaTime;
        if (isFastPan)
            translateVector *= fastPanMultiplier;

        transform.Translate(translateVector);
        moveCameraInBoundsIfOut();
    }

    private void setPosBools()
    {
        float xPos = transform.position.x;
        float zPos = transform.position.z;

        isCamInTopQtr = zPos > maxBound;
        isCamInBottomQtr = zPos < minBound;
        isCamInLeftQtr = xPos < minBound;
        isCamInRightQtr = xPos > maxBound;
    }

    public void moveCameraInBoundsIfOut()
    {
        setPosBools();
        float positionX = transform.position.x, positionZ = transform.position.z;

        if (isCamInLeftQtr)
            positionX = minBound;
        else if (isCamInRightQtr)
            positionX = maxBound;
        if (isCamInBottomQtr)
            positionZ = minBound;
        else if (isCamInTopQtr)
            positionZ = maxBound;

        transform.position = new Vector3(positionX, transform.position.y, positionZ);
    }

    private void zoomCamera()
    {
        float zoomInput = Input.GetAxis("Mouse ScrollWheel");

        Vector3 translateVector = Camera.main.transform.TransformDirection(Vector3.forward) * zoomInput * zoomSpeed;
        transform.Translate(translateVector, Space.World);
        zoomCameraInBoundsIfOut();
    }

    public void zoomCameraInBoundsIfOut()
    {
        float posY = transform.position.y;

        if (posY > maximumHeight || posY < minimumHeight)
        {
            float targetHeight = (posY > maximumHeight) ? maximumHeight : minimumHeight;

            Vector3 forward = Camera.main.transform.TransformDirection(Vector3.forward);
            float forwardY = Mathf.Abs(forward.y);
            float heightDifference = transform.position.y - targetHeight;
            transform.Translate(forward * heightDifference / forwardY, Space.World);
        }
    }

    private void rotateCamera()
    {
        float rotateInput = Input.GetAxis("Rotation");

        transform.Rotate(new Vector3(0, rotateInput, 0) * rotationSpeed * Time.deltaTime, Space.World);
    }

    private void OnValidate()
    {
        setTerrain();
        if (terrain)
        {
            setMaximumHeight();

            //If camera was moved too far up
            if (getMaximumHeight() < minimumHeight)
                minimumHeightAboveTerrain -= (minimumHeight - getMaximumHeight());
            //If camera was moved too far down
            if (minimumHeightAboveTerrain < 0)
                minimumHeightAboveTerrain = 0;
        }
    }
}
