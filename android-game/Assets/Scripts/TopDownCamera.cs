using UnityEngine;
using System.Collections;

public class TopDownCamera : MonoBehaviour
{

    #region Public Classes

    public Transform target;

    [System.Serializable]
    //distance from our target.
    //bools for zooming and smooth following
    // min and max zoom settings
    public class PositionSettings
    {
        public Vector3 targetPosOffset = new Vector3(0, 3.4f, 0);
        public float distanceFromTarget = -50;
        public bool allowZoom = true;
        public float zoomSmooth = 100;
        public float zoomStep = 2;
        public float maxZoom = -30;
        public float minZoom = -60;
        public bool smoothFollow = true;
        public float smooth = 0.05f;

        [HideInInspector]
        public float newDistance = -50;
        [HideInInspector]
        public float adjustmentDistance = -8;

        //Used for smooth zooming


    }

    [System.Serializable]
    //holding our curretn x and y rotation for our camera
    //bool for allowing orbit
    public class OrbitSettings
    {
        public float xRotation = -65;
        public float yRotaion = -180;
        public bool allowOrbit = true;
        public float yOrbitSmooth = 0.5f;

    }

    [System.Serializable]
    public class InputSettings
    {
        public string MOUSE_ORBIT = "MouseOrbit";
        public string ZOOM = "Mouse ScrollWheel";

    }

    [System.Serializable]
    public class DebugSettings
    {
        public bool drawDesiredCollisionLines = true;
        public bool drawAdjustedCollisionLines = true;
    }


    public PositionSettings position = new PositionSettings();
    public OrbitSettings orbit = new OrbitSettings();
    public InputSettings input = new InputSettings();
    public DebugSettings debug = new DebugSettings();
    public CollisionHandler collision = new CollisionHandler();

    #endregion

    #region Private Variables

    Vector3 targetPos = Vector3.zero;
    Vector3 destination = Vector3.zero;
    Vector3 camVelocity = Vector3.zero;
    Vector3 currentMousePosition = Vector3.zero;
    Vector3 adjustedDestination = Vector3.zero;
    Vector3 camVel = Vector3.zero;
    Vector3 previousMousePosition = Vector3.zero;
    float mouseOrbitInput, zoomInput;

    #endregion

    // Use this for initialization
    void Start()
    {
        //setting camera target
        SetCameraTarget(target);

        if (target)
        {
            MoveToTarget();

            collision.Initialize(Camera.main);
            collision.UpdateCameraClipPoints(transform.position, transform.rotation, ref collision.adjustedCameraClipPoints);
            collision.UpdateCameraClipPoints(destination, transform.rotation, ref collision.desiredCameraClipPoints);
        }


    }

    void SetCameraTarget(Transform t)
    {
        //if we want to set a new target at runtime
        target = t;
        if (target == null)
        {
            Debug.LogError("Your Camera needs a target");
        }
    }

    void GetInput()
    {
        //filling the values for our input variables
        mouseOrbitInput = Input.GetAxisRaw(input.MOUSE_ORBIT);
        zoomInput = Input.GetAxisRaw(input.ZOOM);
    }

    // Update is called once per frame
    void Update()
    {
        //getting input and zooming
        GetInput();
        // if (position.allowZoom) {
        // ZoomInOnTarget ();
        //}

    }

    void FixedUpdate()
    {
        //movetotarget
        //lookattarget
        //orbit
        if (target)
        {
            MoveToTarget();
            LookAtTarget();
            // MouseOrbitTarget();

            collision.UpdateCameraClipPoints(transform.position, transform.rotation, ref collision.adjustedCameraClipPoints);
            collision.UpdateCameraClipPoints(destination, transform.rotation, ref collision.desiredCameraClipPoints);


            for (int i = 0; i < 5; i++)
            {
                if (debug.drawDesiredCollisionLines)
                {
                    Debug.DrawLine(targetPos, collision.desiredCameraClipPoints[i], Color.white);
                }

                if (debug.drawAdjustedCollisionLines)
                {
                    Debug.DrawLine(targetPos, collision.adjustedCameraClipPoints[i], Color.green);
                }
            }

            collision.CheckColliding(targetPos);
            position.adjustmentDistance = collision.GetAdjustedDistanceWithRayFrom(targetPos);

        }

    }

    void MoveToTarget()
    {
        //handling getting our camera to its destination position
        targetPos = target.position + Vector3.up * position.targetPosOffset.y + Vector3.forward * position.targetPosOffset.z + transform.TransformDirection(Vector3.right * position.targetPosOffset.x);
        destination = Quaternion.Euler(orbit.xRotation, orbit.yRotaion + target.eulerAngles.y, 0) * -Vector3.forward * position.distanceFromTarget;
        destination += targetPos;

        if (collision.colliding)
        {
            adjustedDestination = Quaternion.Euler(orbit.xRotation, orbit.yRotaion + target.eulerAngles.y, 0) * Vector3.forward * position.adjustmentDistance;
            adjustedDestination += targetPos;
            if (position.smoothFollow)
            {
                transform.position = Vector3.SmoothDamp(transform.position, adjustedDestination, ref camVelocity, position.smooth);
            }
            else
            {
                transform.position = adjustedDestination;
            }
        }
        else
        {

            if (position.smoothFollow)
            {
                transform.position = Vector3.SmoothDamp(transform.position, destination, ref camVelocity, position.smooth);
            }
            else
            {
                transform.position = destination;
            }

        }
    }



    void LookAtTarget()
    {
        //handling getting our camera to look at the target at all times
        Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position);
        transform.rotation = targetRotation;
    }

    void MouseOrbitTarget()
    {
        previousMousePosition = currentMousePosition;
        currentMousePosition = Input.mousePosition;

        if (mouseOrbitInput > 0)
        {
            orbit.yRotaion += (currentMousePosition.x - previousMousePosition.x) * orbit.yOrbitSmooth;
        }
    }

    void ZoomInOnTarget()
    {
        position.newDistance += position.zoomStep * zoomInput;

        position.distanceFromTarget = Mathf.Lerp(position.distanceFromTarget, position.newDistance, position.zoomSmooth * Time.deltaTime);

        if (position.distanceFromTarget > position.maxZoom)
        {
            position.distanceFromTarget = position.maxZoom;
            position.newDistance = position.maxZoom;
        }

        if (position.distanceFromTarget < position.minZoom)
        {
            position.distanceFromTarget = position.minZoom;
            position.newDistance = position.minZoom;
        }

    }


    [System.Serializable]
    public class CollisionHandler
    {
        public LayerMask collisionLayer;

        [HideInInspector]
        public bool colliding = false;
        [HideInInspector]
        public Vector3[] adjustedCameraClipPoints;
        [HideInInspector]
        public Vector3[] desiredCameraClipPoints;

        Camera camera;

        public void Initialize(Camera cam)
        {
            camera = cam;
            adjustedCameraClipPoints = new Vector3[5];
            desiredCameraClipPoints = new Vector3[5];

        }

        public void UpdateCameraClipPoints(Vector3 cameraPosition, Quaternion atRotation, ref Vector3[] intoArray)
        {
            if (!camera)
                return;
            //clear the contents of intorarray
            intoArray = new Vector3[5];

            float z = camera.nearClipPlane;
            float x = Mathf.Tan(camera.fieldOfView / 3.41f) * z;
            float y = x / camera.aspect;

            //find the 4 clipPoints
            //top left
            intoArray[0] = (atRotation * new Vector3(x, y, z)) + cameraPosition;//added and rotated the point relative to the camera.
                                                                                //top right
            intoArray[1] = (atRotation * new Vector3(-x, y, z)) + cameraPosition;//added and rotated the point relative to the camera.
                                                                                 //bot left
            intoArray[2] = (atRotation * new Vector3(-x, -y, z)) + cameraPosition;//added and rotated the point relative to the camera.
                                                                                  //bot right
            intoArray[3] = (atRotation * new Vector3(-x, -y, z)) + cameraPosition;//added and rotated the point relative to the camera.
                                                                                  //cameras position
            intoArray[4] = cameraPosition - camera.transform.forward;
        }

        bool CollisionDetectedAtClipPoints(Vector3[] clipPoints, Vector3 fromPosition)
        {
            for (int i = 0; i < clipPoints.Length; i++)
            {
                Ray ray = new Ray(fromPosition, clipPoints[i] - fromPosition);
                float distacne = Vector3.Distance(clipPoints[i], fromPosition);
                if (Physics.Raycast(ray, distacne, collisionLayer))
                {
                    return true;
                }
            }
            return false;
        }



        public float GetAdjustedDistanceWithRayFrom(Vector3 from)
        {
            float distance = -1;

            for (int i = 0; i < desiredCameraClipPoints.Length; i++)
            {
                Ray ray = new Ray(from, desiredCameraClipPoints[i] - from);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {

                    if (distance == -1)
                        distance = hit.distance;
                    else
                    {
                        if (hit.distance < distance)
                            distance = hit.distance;
                    }
                }

            }

            if (distance == -1)
                return 0;
            else
                return distance;
        }

        public void CheckColliding(Vector3 targetPosition)
        {
            if (CollisionDetectedAtClipPoints(desiredCameraClipPoints, targetPosition))
                colliding = true;
            else
            {
                colliding = false;
            }

        }

    }
}
