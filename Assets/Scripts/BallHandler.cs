using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class BallHandler : MonoBehaviour
{
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Rigidbody2D pivot;
    [SerializeField] private float detachDelay;
    [SerializeField] private float respawnDelay;

    private Rigidbody2D currentBallRigidbody;
    private SpringJoint2D currentBallSprintJoint;

    private Camera mainCamera;
    private bool isDragging;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;

        SpawnNewBall();
    }

    void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentBallRigidbody == null) { return; }

        if (Touch.activeTouches.Count == 0)
        {
            if (isDragging)
            {
                LaunchBall();
            }

            isDragging = false;

            return;
        }

        isDragging = true;
        currentBallRigidbody.isKinematic = true;

        Vector2 touchPosition = new Vector2();

        foreach(Touch touch in Touch.activeTouches)
        {
            touchPosition += touch.screenPosition;
        }

        touchPosition /= Touch.activeTouches.Count;

        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(touchPosition);

        currentBallRigidbody.position = worldPosition;
    }

    private void SpawnNewBall()
    {
        GameObject ballInstance = Instantiate(ballPrefab, pivot.position, Quaternion.identity);

        currentBallRigidbody = ballInstance.GetComponent<Rigidbody2D>();
        currentBallSprintJoint = ballInstance.GetComponent<SpringJoint2D>();

        currentBallSprintJoint.connectedBody = pivot;
    }

    private void LaunchBall()
    {
        currentBallRigidbody.isKinematic = false;
        currentBallRigidbody = null;

        Invoke(nameof(DetachBall), detachDelay);
    }

    private void DetachBall()
    {
        currentBallSprintJoint.enabled = false;
        currentBallSprintJoint = null;

        Invoke(nameof(SpawnNewBall), respawnDelay);
    }
}
