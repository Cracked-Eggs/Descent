using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public class FreeClimbAnimHook : MonoBehaviour
    {
        Animator anim;

        IKSnapshot ikBase;
        IKSnapshot current  = new IKSnapshot();
        IKSnapshot next = new IKSnapshot();

        public float w_rh;
        public float w_lh;

        Vector3 rh, lh;

        Transform h;

        public void Init(FreeClimb c, Transform helper)
        {
            anim = c.anim;
            ikBase = c.baseIKsnapshot;
            h = helper;
        }

        public void CreatePositions(Vector3 origin)
        {
            IKSnapshot ik = CreateSnapShot(origin);
            CopySnapShot(ref current , ik);

            UpdateIKPosition(AvatarIKGoal.RightHand, current.rh);
            UpdateIKPosition(AvatarIKGoal.LeftHand, current.lh);

            UpdateIKWeight(AvatarIKGoal.RightHand, 1);
            UpdateIKWeight(AvatarIKGoal.LeftHand, 1);
        }

        public IKSnapshot CreateSnapShot(Vector3 o)
        {
            IKSnapshot r = new IKSnapshot();
            r.lh = LocalToWorld(ikBase.lh);
            r.rh = LocalToWorld(ikBase.rh);
            return r;
        }

        Vector3 LocalToWorld(Vector3 p)
        {
            Vector3 r = h.position;
            r += h.right * p.x;
            r += h.forward * p.z;
            r += h.up * p.y;
            return r;
        }

        public void CopySnapShot(ref IKSnapshot to, IKSnapshot from)
        {
            to.rh = from.rh;
            to.lh = from.lh;
        }

        public void UpdateIKPosition(AvatarIKGoal goal,Vector3 pos)
        {
            switch (goal)
            {
                case AvatarIKGoal.LeftHand:
                    lh = pos;
                    break;
                case AvatarIKGoal.RightHand:
                    rh = pos;
                    break;
                default:
                    break;

            }

        }
        public void UpdateIKWeight(AvatarIKGoal goal, float w)
        {
            switch (goal)
            {
                case AvatarIKGoal.LeftHand:
                    w_lh = w;
                    break;
                case AvatarIKGoal.RightHand:
                    w_rh = w;
                    break;
                default:
                    break;

            }

        }

        void OnAnimatorIK()
        {
            SetIKPos(AvatarIKGoal.LeftHand, lh, w_lh);
            SetIKPos(AvatarIKGoal.RightHand, rh, w_rh);
        }

        void SetIKPos(AvatarIKGoal goal, Vector3 tp, float w)
        {
            anim.SetIKPositionWeight(goal, w);
            anim.SetIKPosition(goal, tp);

        }
    }
     
  }


