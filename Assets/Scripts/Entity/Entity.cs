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

        public virtual void OnDead()
        {
            transform.DOKill();

            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
            }

            foreach (var comp in GetComponents<MonoBehaviour>())
            {
                comp.enabled = false;
            }

            // foreach (var col in GetComponents<Collider>())
            // {
            //     col.enabled = false;
            // }
        }
    }
}
