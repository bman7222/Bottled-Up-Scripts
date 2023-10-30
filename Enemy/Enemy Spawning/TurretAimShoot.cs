using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretAimShoot : MonoBehaviour
{
    [Header("The object being shot at")]
    [SerializeField] private Transform target;
    [Header("The point from which the projectile is shot from")]
    //[SerializeField] private Transform shootPoint;
    public Transform shootPoint;
    [SerializeField] LineRenderer _line;
    public GameObject projectilePrefab;

    

    //the launch velocity
    private Vector3 launchVelocity;

    [Header("Turret path parameters")]
    [SerializeField] [Range(10, 100)] private int linePoints = 25;
    [SerializeField] [Range(0.01f, 0.25f)] private float timeBetweenPoints = 0.1f;

    public float h = 25;
    public float gravity = -18;

    //
    public Rigidbody ball;

    //Shooting and aiming 
    public float detectionRadius = 20f;

    [SerializeField] private bool shootOnCD = false;
    //time before the turret should start aiming again
    public float shootInterval = 3f;
    //the default time for aiming
    [SerializeField] private float aimTime = 3f;
    //the countdown timer for aiming, starts counting down from aimTime
    [SerializeField] private float aimCountdown;

    public Rigidbody rb;
    public float rotationSpeed;
    

    private void Awake()
    {
        //get the target object
        target = GameObject.Find("Player_02").transform;

        //set the number of position points to number of line points
        _line.positionCount = linePoints;

        //initialize the turret's shooting path line position to (0, 0, 0)
        _line.transform.position = Vector3.zero;

        //get the release point of the projectile
        //shootPoint = shootPoint.s//transform.GetChild(1).transform;

        //initialize the aim countdown
        aimCountdown = aimTime;

        //ball.useGravity = false;
    }

    private void Update()
    {
        //rotate

        transform.LookAt(target, Vector3.up);

        transform.rotation*= Quaternion.Euler(0, 90, 0);

        /*
        if(Input.GetMouseButtonDown(1)){
            LaunchProjectile();
        }
        Debug.Log(Vector3.Distance(transform.position, target.transform.position));
        */
        //check for stuff if shoot not on cooldown
        if (!shootOnCD)
        {
            //if the target is within range and is still aiming, update the path and decrement the aiming counter
            if (Vector3.Distance(transform.position, target.transform.position) <= detectionRadius)
            {
                _line.enabled = true;
                DrawPath();
                aimCountdown -= Time.deltaTime;
            }
            //if the target has left the range and is not in the process of shooting, stop showing the path
            else if (Vector3.Distance(transform.position, target.transform.position) > detectionRadius)
            {
                aimCountdown = aimTime;
                _line.enabled = false;
            }

            //once the countdown reaches 0, launch the projectile, and go into cooldown
            if (aimCountdown <= 0)
            {
                shootOnCD = true;
                LaunchProjectile();
                StartCoroutine("ShootCD");
            }
        }
        

    }

    void LaunchProjectile()
    {
        Physics.gravity = Vector3.up * gravity;

        //spawn the projectile and launch it with the initial velocity
        Rigidbody projectileRB = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity).GetComponent<Rigidbody>();
        projectileRB.useGravity = true;
        projectileRB.velocity = CalculateLaunchData().initialVelocity;

        //test if the ball is even shooting proper to begin with
        //ball.useGravity = true;
        //ball.velocity = CalculateLaunchData().initialVelocity;

        //for debugging purposes
        //Instantiate(projectileRB, target.position, Quaternion.identity);
    }

    LaunchData CalculateLaunchData()
    {
        //physics stuff
        float displacementY = target.position.y - shootPoint.position.y;
        Vector3 displacementXZ = new Vector3(target.position.x - shootPoint.position.x, 0f, target.position.z - shootPoint.position.z);
        h = displacementXZ.magnitude / 2f;
        float time = Mathf.Sqrt(-2 * h/gravity) + Mathf.Sqrt(2 * (displacementY - h) / gravity);
        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * h);
        Vector3 velocityXZ = displacementXZ / time;

        return new LaunchData(velocityXZ + velocityY * -Mathf.Sign(gravity), time);
    }

    void DrawPath()
    {
        //Debug.Log("number of line points: " + linePoints);
        LaunchData launchData = CalculateLaunchData();
        Vector3 previousDrawPoint = shootPoint.position;
        //Debug.Log("shoot point: " + shootPoint.position);

        //int resolution = 30;
        for (int i = 1; i < linePoints; i++)
        {
            float simulationTime = i / (float)linePoints * launchData.timeToTarget;
            //the next point based on time (v0t + (1/2)at^2)
            Vector3 displacement = launchData.initialVelocity * simulationTime + Vector3.up * gravity * simulationTime * simulationTime / 2f;
            Vector3 drawPoint = shootPoint.position + displacement;
            _line.SetPosition(i - 1, previousDrawPoint);
            _line.SetPosition(i, drawPoint);
            previousDrawPoint = drawPoint;
            //Debug.Log("current line point number: " + i);
        }
    }

    struct LaunchData
    {
        public readonly Vector3 initialVelocity;
        public readonly float timeToTarget;

        public LaunchData(Vector3 initialVelocity, float timeToTarget) {
            this.initialVelocity = initialVelocity;
            this.timeToTarget = timeToTarget;
        }

    }

    IEnumerator ShootCD()
    {
        //reset the shoot cd and aim countdown after a set amount of time
        yield return new WaitForSeconds(shootInterval);
        shootOnCD = false;
        aimCountdown = aimTime;
    }
}
