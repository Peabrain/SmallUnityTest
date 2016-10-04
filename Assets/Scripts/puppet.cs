using UnityEngine;
using System.Collections;

public class puppet : receiver {

    public const int trans_flag_position = 1;
    public const int trans_flag_rotation = 2;

    float lastTime = 0;
    float syncTime = 0;
    float syncDelay = 0;

    Vector3 lastPos = new Vector3(0, 0, 0);
    Vector3 destPos = new Vector3(0, 0, 0);
    Quaternion lastRot;
    Quaternion destRot;

    internal Vector3 Movevector = new Vector3(0, 0,0);

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
        InterpolateMovement(puppet.trans_flag_position | puppet.trans_flag_rotation);
    }
    internal void InterpolateMovement(int flags)
    {
        syncTime += Time.deltaTime;
        if (syncDelay > 0.0f)
        {
            if ((flags & trans_flag_position) == trans_flag_position)
                transform.localPosition = Vector3.Lerp(lastPos, destPos, syncTime / syncDelay);
            if ((flags & trans_flag_rotation) == trans_flag_rotation)
                transform.localRotation = Quaternion.Lerp(lastRot, destRot, syncTime / syncDelay);
        }
    }
    public void SetTransform(Vector3 v,Quaternion r,int flags)
    {
        syncTime = 0f;
        syncDelay = Time.time - lastTime;
        lastTime = Time.time;
        lastPos = transform.localPosition;
        lastRot = transform.localRotation;
        if ((flags & trans_flag_position) == trans_flag_position)
            destPos = v;

        if ((flags & trans_flag_rotation) == trans_flag_rotation)
            destRot = r;
    }

    public virtual void Control()
    {
    }
    public virtual void SetActiv(bool a)
    {
        isActiv = a;
    }
    public void SetMoveVector(Vector3 m)
    {
        Movevector = m;
    }
}
