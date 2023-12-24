using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

namespace SA
{
    public class FreeClimb : MonoBehaviour
    {
        public Animator anim;
        public bool isClimbing;

        bool inPosition;
        bool isLerping;
        float t;
        Vector3 startPos;
        Vector3 targetPos;
        Quaternion startRot;
        Quaternion targetRot;
        public float possitionOffset;

        public float offsetFromWall = 0.3f;
        public float speed_multiplier = 0.2f;
        public float climbSpeed = 3.0f;
        public float rotateSpeed = 5.0f;

        public float rayTowardsMoveDir = 0.5f;
        public float rayForwardTowardsWall = 1;
        public float maxClimbAngle = 60.0f;

        public float horizontal;
        public float vertical;
        public bool isMid;
        public Transform helper;



        public PlayerStateManager ps;


        float delta;

        public IKSnapshot baseIKsnapshot;
        public FreeClimbAnimHook a_hook;
        void Start()
        {
            Init();
        }
        void Init()
        {

            CheckForClimb();
            

        }
        public void CheckForClimb()
        {
            Vector3 origin = transform.position;
            origin.y += 1.0f;
            Vector3 dir = transform.forward;
            RaycastHit hit;
            if (Physics.Raycast(origin, dir, out hit, 5))
            {
                
                helper.position = PosWithOffset(origin, hit.point);
                InitForClimb(hit);
            }
            else
            {
                isClimbing = false;
            }
            

        }

        void InitForClimb(RaycastHit hit)
        {
            isClimbing = true;

            helper.transform.rotation = Quaternion.LookRotation(-hit.normal);
            startPos = transform.position;


            targetPos = hit.point + (hit.normal * offsetFromWall);
            t = 0;
            inPosition = false;
            anim.SetTrigger("climb_ilde");
        }
        void Update()
        {
            delta = Time.deltaTime;
            Tick(delta);
            
        }
        public void Tick(float delta)
        {
            if (!inPosition)
            {
                GetInPosition();
                return;
            }

            if (!isLerping)
            {
                horizontal = Input.GetAxis("Horizontal");
                vertical = Input.GetAxis("Vertical");

                if (vertical < 0)
                {
                    vertical = 0;
                }

                float m = Mathf.Abs(horizontal) + Mathf.Abs(vertical);

                Vector3 h = helper.right * horizontal;

                Vector3 v = helper.up * vertical;

                Vector3 moveDir = (h + v).normalized;
              


                if (isMid)
                {
                    if (moveDir == Vector3.zero)
                        return;
                }
                else
                {
                    bool canMove = CanMove(moveDir);
                    if (!canMove || moveDir == Vector3.zero) return;
                }
                
                t = 0;
                isLerping = true;
                startPos = transform.position;
                startRot = transform.rotation;
                Vector3 tp = helper.position - transform.position;

                float d = Vector3.Distance(helper.position, startPos) / 2;
                tp *= possitionOffset;
                tp += transform.position;

                targetPos = (isMid) ? tp : helper.position;
                if (vertical > 0 && horizontal == 0)
                {

                    // Move Up animation
                    Debug.Log("Moving Up animation");
                    anim.SetTrigger("MoveUp");
                }
                else if (vertical > 0 && horizontal < 0)
                {
                    // Move Up + Left animation
                    Debug.Log("Moving up and left animation");
                    anim.SetTrigger("MoveUpLeft");
                }
                else if (vertical > 0 && horizontal > 0)
                {
                    // Move Up + Right animation
                    Debug.Log("Moving up and right animation");
                    anim.SetTrigger("MoveUpRight");
                }
                else if (horizontal < 0)
                {
                    // Move Left animation
                    Debug.Log("Moving left animation");
                    anim.SetTrigger("MoveLeft");
                }
                else if (horizontal > 0)
                {
                    Debug.Log("Moving right animation");
                    // Move Right animation
                    anim.SetTrigger("MoveRight");
                }


                //a_hook.CreatePositions(targetPos);


            }
            else
            {
                t += delta * climbSpeed;
                if (t > 1)
                {
                    t = 1;
                    isLerping = false;

                }

                Vector3 cp = Vector3.Lerp(startPos, targetPos, t);
                transform.position = cp;
                transform.rotation = Quaternion.Slerp(transform.rotation, helper.rotation, delta * rotateSpeed);
            }
        }


        bool CanMove(Vector3 moveDir)
        {
            Vector3 origin = transform.position;
            float dis = rayTowardsMoveDir;
            if (moveDir.y > 0  || (moveDir.y > 0 && (Mathf.Abs(moveDir.x) >0.1f)))
            {
                origin = transform.position + transform.up * 0.50f;
                
            }
            
            Vector3 dir = moveDir;

            // DebugLine.singleton.SetLine(origin, origin + (dir * dis), 0);

            // Raycast towards the direction you want to move
            RaycastHit hit;

            if (Physics.Raycast(origin, dir, out hit, dis))
            {
                // Check if it's a corner or an obstacle that should prevent movement
                return false;
            }

            origin += moveDir * dis;
            dir = helper.forward;
            float dis2 = rayForwardTowardsWall;
            // DebugLine.singleton.SetLine(origin, origin + (dir * dis2), 1);

            if (Physics.Raycast(origin, dir, out hit, dis))
            {
                helper.position = PosWithOffset(origin, hit.point);
                helper.rotation = Quaternion.LookRotation(-hit.normal);
                return true;
            }

            origin = origin + (dir * dis2);
            dir = -moveDir;
            // DebugLine.singleton.SetLine(origin, origin + dir, 1);
            if (Physics.Raycast(origin, dir, out hit, rayForwardTowardsWall))
            {
                helper.position = PosWithOffset(origin, hit.point);
                helper.rotation = Quaternion.LookRotation(-hit.normal);
                return true;
            }

            // return false;

            origin += dir * dis2;
            dir = -Vector3.up;

            // Debug.DrawRay(origin, dir * dis2, Color.yellow);
            // DebugLine.singleton.SetLine(origin, origin + dir, 2);
            if (Physics.Raycast(origin, dir, out hit, dis2))
            {
                float angle = Vector3.Angle(-helper.forward, hit.normal);
                if (angle < 40)
                {
                    helper.position = PosWithOffset(origin, hit.point);
                    helper.rotation = Quaternion.LookRotation(-hit.normal);
                    return true;
                }
            }


            return false;
        }

        public void GetInPosition()
        {
            t += delta;

            if (t > 1)
            {
                t = 1;
                inPosition = true;

                a_hook.CreatePositions(targetPos);
            }

            Vector3 tp = Vector3.Lerp(startPos, targetPos, t);
            transform.position = tp;
            transform.rotation = Quaternion.Slerp(transform.rotation, helper.rotation, delta * rotateSpeed);
        }

        Vector3 PosWithOffset(Vector3 origin, Vector3 target)
        {
            Vector3 direction = origin - target;
            direction.Normalize();
            Vector3 offset = direction * offsetFromWall;
            return target + offset;
        }
    }

    [System.Serializable]
    public class IKSnapshot
    {
        public Vector3 rh, lh;
    }

}