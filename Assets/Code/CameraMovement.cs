/* ====================================
 * Credit goes to puppeteer from the Unity Forums
 * Source: https://forum.unity.com/threads/rts-camera-script.72045/
 * ==================================== */

using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float ScrollSpeed = 15;
    public float ScrollEdge = .01f;

    public float PanSpeed = 50;

    public float cameraTurnSpeed = .1f;
    private float oldMousePosition;

    public LayerMask groundLayer;
    public float heightAboveGround;

    //zoom
    public float zoomSpeed = 3;
    public int maxHeight = 60;
    public int minHeight = 1;

    public int edgeOffset = 5;

    private bool rightFucked = true;
    private bool leftFucked = true;
    private bool backFucked = true;
    private bool frontFucked = true;

    void Start()
    {
        oldMousePosition = Input.mousePosition.x;
    }

    void Update()
    {
        //SpecificAmountAboveGround
        RaycastHit hit;
        if(Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, groundLayer)){
            transform.position = new Vector3(transform.position.x, hit.point.y + heightAboveGround, transform.position.z);
        } else {
            if(!rightFucked){
                transform.position = new Vector3(transform.position.x + (ScrollSpeed * Time.deltaTime), transform.position.y, transform.position.z);
            }
            if(!leftFucked){
                transform.position = new Vector3(transform.position.x - (ScrollSpeed * Time.deltaTime), transform.position.y, transform.position.z);
            }
            if(!frontFucked){
                transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + (ScrollSpeed * Time.deltaTime));
            }
            if(!backFucked){
                transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - (ScrollSpeed * Time.deltaTime));
            }
            //transform.position = new Vector3(0,-20,0);
        }
        Debug.DrawRay(new Vector3(transform.position.x+edgeOffset, transform.position.y, transform.position.z), Vector3.down*100);
        Debug.DrawRay(new Vector3(transform.position.x-edgeOffset, transform.position.y, transform.position.z), Vector3.down*100);
        Debug.DrawRay(new Vector3(transform.position.x, transform.position.y, transform.position.z+edgeOffset), Vector3.down*100);
        Debug.DrawRay(new Vector3(transform.position.x, transform.position.y, transform.position.z-edgeOffset), Vector3.down*100);

        if(Physics.Raycast(new Vector3(transform.position.x+edgeOffset, transform.position.y, transform.position.z), Vector3.down*100, Mathf.Infinity, groundLayer)){
            leftFucked = true;
        } else {
            Debug.Log("rigt fucked");
            leftFucked = false;
        }
        if(Physics.Raycast(new Vector3(transform.position.x-edgeOffset, transform.position.y, transform.position.z), Vector3.down*100, Mathf.Infinity, groundLayer)){
            rightFucked = true;
        }else {
            Debug.Log("left fucked");
            rightFucked = false;
        }
        if(Physics.Raycast(new Vector3(transform.position.x, transform.position.y, transform.position.z+edgeOffset), Vector3.down*100, Mathf.Infinity, groundLayer)){
            backFucked = true;
        }else {
            Debug.Log("front fucked");
            backFucked = false;
        }
        if(Physics.Raycast(new Vector3(transform.position.x, transform.position.y, transform.position.z-edgeOffset), Vector3.down*100, Mathf.Infinity, groundLayer)){
            frontFucked = true;
        }else {
            Debug.Log("back fucked");
            frontFucked = false;
        }
        
        if ((Input.GetAxis("Horizontal") < 0 || Input.mousePosition.x <= Screen.width * ScrollEdge) && rightFucked) { transform.Translate(Vector3.right * Time.deltaTime * -ScrollSpeed); }
        if ((Input.GetAxis("Horizontal") > 0 || Input.mousePosition.x >= Screen.width * (1 - ScrollEdge)) && leftFucked) { transform.Translate(Vector3.right * Time.deltaTime * ScrollSpeed); }
        if ((Input.GetAxis("Vertical") > 0 || Input.mousePosition.y >= Screen.height * (1 - ScrollEdge)) && backFucked) { transform.Translate(Vector3.forward * Time.deltaTime * ScrollSpeed); }
        if ((Input.GetAxis("Vertical") < 0 || Input.mousePosition.y <= Screen.height * ScrollEdge) && frontFucked) { transform.Translate(Vector3.forward * Time.deltaTime * -ScrollSpeed); }
        //End van code (PS: als je precies in een hoek komt... you're fucked)


        //PAN
        // if (Input.GetAxis("Horizontal") > 0 || Input.mousePosition.x >= Screen.width * (1 - ScrollEdge))
        //     transform.Translate(Vector3.right * Time.deltaTime * ScrollSpeed);
        // else if (Input.GetAxis("Horizontal") < 0 || Input.mousePosition.x <= Screen.width * ScrollEdge)
        //     transform.Translate(Vector3.right * Time.deltaTime * -ScrollSpeed);

        // if (Input.GetAxis("Vertical") > 0 || Input.mousePosition.y >= Screen.height * (1 - ScrollEdge))
        //     transform.Translate(Vector3.forward * Time.deltaTime * ScrollSpeed);
        // else if (Input.GetAxis("Vertical") < 0 || Input.mousePosition.y <= Screen.height * ScrollEdge)
        //     transform.Translate(Vector3.forward * Time.deltaTime * -ScrollSpeed);

        //ZOOM IN/OUT
        //Debug.Log("Mouse Scroll Y: " + Input.mouseScrollDelta.y);
        Vector3 rot = transform.rotation.eulerAngles;
        if(Input.GetAxis("Zoom") < 0){
            heightAboveGround -= zoomSpeed * Time.deltaTime;
            //transform.RotateAround(transform.position, transform.right, -(zoomSpeed*1.3f)*Time.deltaTime);
            rot -= new Vector3((zoomSpeed*1.3f)*Time.deltaTime, 0f, 0f);
        } else if(Input.GetAxis("Zoom") > 0){
            heightAboveGround += zoomSpeed * Time.deltaTime;
            //transform.RotateAround(transform.position, transform.right, (zoomSpeed*1.3f)*Time.deltaTime);
            rot += new Vector3((zoomSpeed*1.3f)*Time.deltaTime, 0f, 0f);
        }
        rot.x = ClampAngle(rot.x, -25f, 45f);
        transform.eulerAngles = rot;
        //transform.eulerAngles = new Vector3(Mathf.Clamp(transform.localRotation.x, -25, 45), transform.eulerAngles.y, transform.eulerAngles.z);

        if(heightAboveGround < minHeight){
            heightAboveGround = minHeight;
        }
        if(heightAboveGround > maxHeight){
            heightAboveGround = maxHeight;
        }
        
        //ROTATE
        if (Input.GetMouseButton(1) || Input.GetMouseButton(2))
        {
            float difference = Input.mousePosition.x - oldMousePosition;

            transform.eulerAngles -= new Vector3(0, -difference * cameraTurnSpeed, 0);

            oldMousePosition = Input.mousePosition.x;
        }
        else
            oldMousePosition = Input.mousePosition.x;
    }

    float ClampAngle(float angle, float from, float to)
     {
         // accepts e.g. -80, 80
         if (angle < 0f) angle = 360 + angle;
         if (angle > 180f) return Mathf.Max(angle, 360+from);
         return Mathf.Min(angle, to);
     }
}
