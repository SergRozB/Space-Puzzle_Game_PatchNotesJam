using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class CameraManager : MonoBehaviour
{
    private Camera mainCamera;
    private PixelPerfectCamera ppc;
    [Range(1, 1.5f)] [SerializeField] private float zoomSpeed = 2;
    [SerializeField] private float minZoomMultiplier = 0.5f;
    [SerializeField] private float maxZoomMultiplier = 3f;
    [SerializeField] private float panSpeed = 0.5f;
    [SerializeField] private InputActionReference zoomAction;
    [SerializeField] private InputActionReference controlAction;
    [SerializeField] private InputActionReference move;
    private int startResX;
    private int startResY;
    private float startOrthographicSize;
    [SerializeField] private float cameraMoveSpeed = 1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCamera = GetComponent<Camera>();
        startOrthographicSize = mainCamera.orthographicSize;
    }

    // Update is called once per frame
    void Update()
    {
        ZoomInOut();
        //ppc.refResolutionX = Mathf.Clamp(ppc.refResolutionX, Mathf.RoundToInt(startResX * minZoomMultiplier), Mathf.RoundToInt(startResX * maxZoomMultiplier));
        //ppc.refResolutionY = Mathf.Clamp(ppc.refResolutionY, Mathf.RoundToInt(startResY * minZoomMultiplier), Mathf.RoundToInt(startResY * maxZoomMultiplier));
        mainCamera.orthographicSize = Mathf.Clamp(mainCamera.orthographicSize, startOrthographicSize * minZoomMultiplier, startOrthographicSize * maxZoomMultiplier);
        transform.Translate(move.action.ReadValue<Vector2>() * Time.deltaTime * cameraMoveSpeed);
    }

    private void ZoomInOut() 
    {
        if (zoomAction.action.ReadValue<float>() > 0 && controlAction.action.ReadValue<float>() == 1)
        {
            mainCamera.orthographicSize *= zoomSpeed;
        }
        else if (zoomAction.action.ReadValue<float>() < 0 && controlAction.action.ReadValue<float>() == 1)
        {
            mainCamera.orthographicSize /= zoomSpeed;
        }
    }
}
