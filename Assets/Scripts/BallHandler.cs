using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
using UnityEditor.U2D.Animation;
#endif

public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField] private GameObject mBallPrefab;
    [SerializeField] private Rigidbody2D mPivot;
    [SerializeField] private float mDetachDelay;
    [SerializeField] private float mRespawnDelay; 

    private Rigidbody2D mCurrentBallRigidbody;
    private SpringJoint2D mCurrentBallSpringJoint; 
    private Camera mMainCamera;
    private bool mIsDragging;

    // Start is called before the first frame update
    void Start()
    {
        mMainCamera = Camera.main;
        SpawnNewBall();
    }

    // Update is called once per frame
    void Update()
    {
        if (mCurrentBallRigidbody == null) 
        {
            return;
        }

        if (!Touchscreen.current.primaryTouch.press.isPressed) 
        {
            if (mIsDragging) 
            {
                LaunchBall();
            }
            mIsDragging = false;
            
            return;
        }

        mIsDragging = true;
        mCurrentBallRigidbody.isKinematic = true;
        Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();

        Vector3 worldPosition = mMainCamera.ScreenToWorldPoint(touchPosition);

        mCurrentBallRigidbody.position = worldPosition;

        Debug.Log(worldPosition);
    }

    private void SpawnNewBall()
    {
        GameObject ballInstance = Instantiate(mBallPrefab, mPivot.position, Quaternion.identity);   
        mCurrentBallRigidbody = ballInstance.GetComponent<Rigidbody2D>();

        mCurrentBallSpringJoint = ballInstance.GetComponent<SpringJoint2D>();
        mCurrentBallSpringJoint.connectedBody = mPivot;
        mCurrentBallSpringJoint.enabled = true;
    }

    private void LaunchBall() 
    {
        mCurrentBallRigidbody.isKinematic = false;
        mCurrentBallRigidbody = null;
        Invoke(nameof(DetachBall), mDetachDelay);
    }

    private void DetachBall() 
    {
        mCurrentBallSpringJoint.enabled = false;
        mCurrentBallSpringJoint = null;
        Invoke(nameof(SpawnNewBall), mRespawnDelay);
    }
}
