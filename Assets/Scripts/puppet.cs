using UnityEngine;
using System.Collections;

public class puppet : receiver {

    float lastTime = 0;
    float syncTime = 0;
    float syncDelay = 0;

    Vector3 lastPos = new Vector3(0, 0, 0);
    Vector3 destPos = new Vector3(0, 0, 0);
    Quaternion lastRot;
    Quaternion destRot;

    internal bool isActiv = false;
    // Use this for initialization
    public void InitTransform(Vector3 v,Quaternion r)
    {
        syncDelay = 0f;
        lastTime = Time.time;
        lastPos = v;
        destPos = lastPos;
        lastRot = r;
        destRot = lastRot;
        syncTime = 0f;
    }
    void Awake()
    {
//        InitTransform(transform.localPosition, transform.localRotation);
    }
    void Start()
    {
        //        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        InterpolateMovement();
    }
    internal void InterpolateMovement()
    {
        syncTime += Time.deltaTime;
        if (syncDelay > 0.0f)
        {
            transform.localPosition = Vector3.Lerp(lastPos, destPos, syncTime / syncDelay);
            transform.localRotation = Quaternion.Lerp(lastRot, destRot, syncTime / syncDelay);
        }
    }
    public void SetTransform(Vector3 v,Quaternion r)
    {
        syncTime = 0f;
        syncDelay = Time.time - lastTime;
        lastTime = Time.time;

        lastPos = transform.localPosition;
        destPos = v;
        lastRot = transform.localRotation;
        destRot = r;
    }
    public virtual void Control()
    {
    }
    public virtual void SetActiv(bool a)
    {
        isActiv = a;
    }
    /*    public virtual Vector3 Control()
    {
        Vector3 speed = new Vector3(0,0,0);
        return speed;
    }
*/
}
