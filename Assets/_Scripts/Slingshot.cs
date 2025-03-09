using UnityEngine;
using System.Collections;

public class Slingshot : MonoBehaviour
{
    static private Slingshot S;

    [Header("Set in Inspector")]
    public GameObject prefabProjectile1;
    public GameObject prefabProjectile2;
    public float velocityMult = 10f;
    public LineRenderer rubberBand;
    public AudioClip snapSound;

    [Header("Set Dynamically")]
    private AudioSource audioSource;
    public GameObject launchPoint;
    public Vector3 launchPos;
    public GameObject projectile;
    public bool aimingMode;
    private Rigidbody projectileRigidbody;
    static public Vector3 LAUNCH_POS
    {
        get
        {
            if(S == null) return Vector3.zero;
            return S.launchPos;
        }
    }

    private void Awake()
    {
        S = this;

        Transform launchPointTrans = transform.Find("LaunchPoint"); // a
        launchPoint = launchPointTrans.gameObject;
        launchPoint.SetActive(false);
        launchPos = launchPointTrans.position;

        rubberBand.enabled = false;

        audioSource = GetComponent<AudioSource>();

    }
    void OnMouseEnter()
    {
        launchPoint.SetActive(true);
        //print("Slingshot:OnMouseEnter()");
    }

    void OnMouseExit()
    {
        launchPoint.SetActive(false);
        //print("Slingshot:OnMouseExit()");
    }

    private void OnMouseOver()
    {

        if (Input.GetMouseButtonDown(0))
        {
            aimingMode = true;
            projectile = Instantiate(prefabProjectile1) as GameObject;
            projectile.transform.position = launchPos;
            projectile.GetComponent<Rigidbody>().isKinematic = true;

            projectileRigidbody = projectile.GetComponent<Rigidbody>();
            projectileRigidbody.isKinematic = true;
            rubberBand.enabled = true;

        }
        if (Input.GetMouseButtonDown(1))
        {
            aimingMode = true;
            projectile = Instantiate(prefabProjectile2) as GameObject;
            projectile.transform.position = launchPos;
            projectile.GetComponent<Rigidbody>().isKinematic = true;

            projectileRigidbody = projectile.GetComponent<Rigidbody>();
            projectileRigidbody.isKinematic = true;
            rubberBand.enabled = true;

        }

    }

    private void Update()
    {
        if(!aimingMode)
            return;
        Vector3 mousePos2d = Input.mousePosition;
        mousePos2d.z = -Camera.main.transform.position.z;
        Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(mousePos2d);

        Vector3 mouseDelta = mousePos3D - launchPos;
        float maxMagnitude = this.GetComponent<SphereCollider>().radius;
        if(mouseDelta.magnitude > maxMagnitude)
        {
            mouseDelta.Normalize();
            mouseDelta *= maxMagnitude;
        }

        Vector3 projPos = launchPos + mouseDelta;
        projectile.transform.position = projPos;

        UpdateRubberBand(projPos); 
    
        if (Input.GetMouseButtonUp(0))
        {
            aimingMode = false;
            projectileRigidbody.isKinematic = false;
            projectileRigidbody.velocity = -mouseDelta * velocityMult;
            FollowCam.POI = projectile;
            projectile = null;
            MissionDemolition.ShotFired();
            ProjectileLine.S.poi = projectile;
            rubberBand.enabled = true;

        }
        if (Input.GetMouseButtonUp(1))
        {
            aimingMode = false;
            projectileRigidbody.isKinematic = false;
            projectileRigidbody.velocity = -mouseDelta * velocityMult;
            FollowCam.POI = projectile;
            projectile = null;
            MissionDemolition.ShotFired();
            ProjectileLine.S.poi = projectile;
            rubberBand.enabled = true;

        }

        rubberBand.enabled = false;
        PlaySnapSound();

    }

    private void UpdateRubberBand(Vector3 projPos)
    {
        rubberBand.SetPosition(0, launchPos);
        rubberBand.SetPosition(1, projPos);

    }

    private void PlaySnapSound()
    {
        if (snapSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(snapSound);
        }
    }


}