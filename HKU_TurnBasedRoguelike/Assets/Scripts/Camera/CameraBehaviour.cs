using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraBehaviour : MonoBehaviour
{
    [SerializeField] private Camera cam;
    Vector3 targetPos = new Vector3(0,0,-10);
    [SerializeField] float transitionSpeed;
    bool isMoving = false;

    [Header("Debugging")]
    private bool canMoveCameraManually = false;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        targetPos = cam.transform.position;

        // get aspect ratio
        float width = Screen.width;
        Debug.Log("Camera; width is: " + width);
        float height = Screen.height;
        Debug.Log("Camera; height is: " + height);
        float aspectFactor = width / height;
        Debug.Log("Camera; aspectFactor is: " + aspectFactor);

        if (aspectFactor > 1.7f && aspectFactor < 1.8f)
        {
            Debug.Log("Camera; Aspect ratio is 16:9");
            cam.orthographicSize = 6f;
        }
        else
        {
            Debug.Log("Camera; Aspect ratio is not 16:9, zooming out");
            cam.orthographicSize = 6.5f;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (cam.transform.position != targetPos)
        {
            // if camera is not on the targetposition move the camera towards the target
            cam.transform.position = Vector3.MoveTowards(cam.transform.position, targetPos, transitionSpeed * Time.deltaTime);
        }
        else if (isMoving)
        {
            isMoving = false;
        }

        if(canMoveCameraManually)
        {
            if (Input.GetKeyDown(KeyCode.W) && !isMoving) { TransitionCamera(new Vector2(0, 1)); }
            if (Input.GetKeyDown(KeyCode.A) && !isMoving) { TransitionCamera(new Vector2(-1, 0)); }
            if (Input.GetKeyDown(KeyCode.S) && !isMoving) { TransitionCamera(new Vector2(0, -1)); }
            if (Input.GetKeyDown(KeyCode.D) && !isMoving) { TransitionCamera(new Vector2(1, 0)); }
        }
    }

    // move the camera in a given direction
    public void TransitionCamera(Vector2 direction)
    {
        Debug.Log("Camera; transitioning");
        isMoving = true;
        Vector3 targetPos = new Vector3(cam.transform.position.x + direction.x * 21, cam.transform.position.y + direction.y * 11, -10);
        SetTargetPoint(targetPos);
    }

    // sets the target position
    public void SetTargetPoint(Vector3 pos)
    {
        targetPos = pos;
    }
}
