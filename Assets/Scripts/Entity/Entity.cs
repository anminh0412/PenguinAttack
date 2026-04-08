using System.Collections.Generic;
using DG.Tweening;
using Plugins.Tick;
using Tick;
using UnityEngine;

namespace Manager
{
    public class Entity : MonoBehaviour, ITickable, IShoot
    {
        public Transform  arrow;
        public Rigidbody  rb;
        public float      force         = 5f;
        public float      tweenDuration = 0.3f;

        protected ShotData shotData;
        private readonly List<Rigidbody> collidedRigidbodies = new();

        protected virtual void OnEnable()
        {
            TickManager.Instance.Add(this);
        }

        protected virtual void OnDisable()
        {
            TickManager.Instance.Remove(this);
        }

        public virtual void Tick() { }

        // private void OnCollisionEnter(Collision collision)
        // {
        //     var other = collision.gameObject.GetComponent<Entity>();
        //     if (other == null) return;
        //     if (other.rb == null) return;
        //     if (collidedRigidbodies.Contains(other.rb)) return;
        //     collidedRigidbodies.Add(other.rb);
        //
        //     var directionToTarget = (other.transform.position - transform.position).normalized;
        //     var pushDirection     = -directionToTarget;
        //     var pushForce         = pushDirection * (shotData.LaunchScale * force);
        //
        //     rb.linearVelocity  = Vector3.zero;
        //     rb.angularVelocity = Vector3.zero;
        //
        //     other.rb.linearVelocity  = Vector3.zero;
        //     other.rb.angularVelocity = Vector3.zero;
        //     other.rb.WakeUp();
        //     other.rb.AddForce(pushForce, ForceMode.Impulse);
        //     Debug.DrawRay(other.transform.position, pushForce, Color.red, 1.5f);
        //     Debug.LogError(other.gameObject.name + "_" + pushForce);
        // }

        public virtual void Shoot()
        {
            collidedRigidbodies.Clear();
            if (shotData == null) return;
            rb.linearVelocity  = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            var seq = DOTween.Sequence();
            seq.Append(
                DOTween.To(() => transform.eulerAngles.y, y =>
                {
                    shotData.Rot.y = y;
                    transform.eulerAngles = shotData.Rot;
                }, shotData.Rot2.y, tweenDuration).From(transform.eulerAngles.y)
            );

            seq.Append(
                DOTween.To(() => arrow.localScale.z, z =>
                {
                    arrow.localScale = new Vector3(1f, 1f, z);
                }, shotData.LaunchScale, tweenDuration).From(0f)
            );

            seq.AppendCallback(() =>
            {
                arrow.localScale = new Vector3(1f, 1f, 0f);
                if (rb && shotData.LaunchScale > 0f)
                {
                    rb.AddForce(shotData.LaunchDir * (shotData.LaunchScale * force), ForceMode.Impulse);
                    // Debug.LogWarning(gameObject.name + "_" + shotData.LaunchDir * (shotData.LaunchScale * force));
                }

                this.shotData = null;
            });
        }
    }
}
