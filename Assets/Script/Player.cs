using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public Animator anim;
    public CharacterController charContrl;
    public GameObject Axe;
    private Rigidbody axeRb;
    private ParticleSystem axeParticale;
    public Transform mainCamara,curvePoint,axeParent;

    private Vector3 oldPosition, targetPosition, targetRotation;

    public float moveSpeed = 3f, turnSmoothTime = 0.1f, throwPower = 10f;
    private float TurnSmoothVelocity,hMove,vMove,returnTime = 0.0f;
     
    bool canWalk = true, canthrow = true, isReturning = false;


    // Start is called before the first frame update
    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        axeRb = Axe.gameObject.GetComponent<Rigidbody>();
        targetPosition = axeRb.transform.localPosition;
        targetRotation = axeRb.transform.localEulerAngles;
        axeParticale =  Axe.GetComponent<ParticleSystem>();
        axeParticale.Stop();
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {

        hMove = Input.GetAxisRaw("Horizontal");
        vMove = Input.GetAxisRaw("Vertical");

        
        if (canWalk)
        {
            Walk();
        }
        if (Input.GetMouseButtonDown(0) && canthrow)
        {
            anim.SetTrigger("Throw");
            //Throw();
        }

        if(Input.GetMouseButtonDown(1))
        {
            ReturnAxe();
        }

        if(isReturning)
        {
            if(returnTime < 1.0f)
            {
                axeRb.position = GetQuadraticCurvePoint(returnTime, oldPosition, curvePoint.position, axeParent.position);
                returnTime += Time.deltaTime;
                //Axe.gameObject.GetComponent<Axe>().activated = true;
            }
            else
            {
                ResetAxe();
            }
        }

        //Can Walk Check.
        if (this.anim.GetCurrentAnimatorStateInfo(0).IsName("Standing Melee Attack Downward"))
        {
            canWalk = false;
        }
        else
            canWalk = true;

    }


    void Walk()
    {
        Vector3 direction = new Vector3(hMove, 0f, vMove).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + mainCamara.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref TurnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            anim.SetBool("Walking", true);
            charContrl.Move(moveDir.normalized * moveSpeed * Time.deltaTime);
        }
        else
        {
            anim.SetBool("Walking", false);
        }
    }


    void Throw()
    {
        isReturning = false;
        Axe.GetComponent<TrailRenderer>().emitting = true;
        axeRb.isKinematic = false;
        axeRb.transform.parent = null;
        Axe.gameObject.GetComponent<Axe>().activated = true;
        axeRb.AddForce(Camera.main.transform.TransformDirection(Vector3.forward) * throwPower, ForceMode.Impulse);

        canthrow = false;
        
    }
    
    void ReturnAxe()
    {
        Axe.gameObject.GetComponent<Axe>().activated = false;
        Axe.gameObject.GetComponent<Axe>().returning = true;
        oldPosition = axeRb.transform.position; 
        isReturning = true;
        //axeRb.isKinematic = true;
        axeRb.velocity = Vector3.zero;
        
    }

    void ResetAxe()
    {
        axeRb.isKinematic = true;
        Axe.GetComponent<TrailRenderer>().emitting = false;
        Axe.gameObject.GetComponent<Axe>().returning = false;
        returnTime = 0.0f;
        axeRb.transform.parent = axeParent;
        axeRb.transform.localPosition = targetPosition;
        axeRb.transform.localEulerAngles = targetRotation;
        oldPosition = Vector3.zero;
        isReturning = false;
        canthrow = true;
        axeParticale.Stop();
    }


    public Vector3 GetQuadraticCurvePoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        return (uu * p0) + (2 * u * t * p1) + (tt * p2);
    }

}