using System;
using System.Collections.Generic;
using DG.Tweening;
using Plugins.Tick;
using SimpleSignalBus;
using Tick;
using UnityEngine;

namespace Manager
{
    public class Entity : MonoBehaviour, ITickable, IShoot
    {
        public                   GamePlayManager gamePlayManager;
        public                   EntityData      entityData;
        [HideInInspector] public bool            IsPlayer => this is PlayerController;
        public                   Transform       arrow;
        public                   Rigidbody       rb;
        public                   float           force         = 5f;
        public                   float           tweenDuration = 0.3f;

        protected        ShotData        shotData;
        private readonly List<Rigidbody> collidedRigidbodies = new();

        public Action<Entity> OnEntityDead;

        public void InitEntity(EntityData data, GamePlayManager manager)
        {
            entityData      = data;
            gamePlayManager = manager;
        }

        protected virtual void OnEnable() { TickManager.Instance.Add(this); }

        protected virtual void OnDisable() { TickManager.Instance.Remove(this); }

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
                    shotData.Rot.y        = y;
                    transform.eulerAngles = shotData.Rot;
                }, shotData.Rot2.y, tweenDuration).From(transform.eulerAngles.y)
            );

            seq.Append(
                DOTween.To(() => arrow.localScale.z, z => { arrow.localScale = new Vector3(1f, 1f, z); }, shotData.LaunchScale, tweenDuration).From(0f)
            );

            seq.AppendCallback(() =>
            {
                arrow.localScale = new Vector3(1f, 1f, 0f);

                if (rb && shotData.LaunchScale > 0f)
                {
                    rb.AddForce(shotData.LaunchDir * (shotData.LaunchScale * force), ForceMode.Impulse);
                }

                this.shotData = null;
            });
        }

        public virtual void OnDead()
        {
            transform.DOKill();

            if (rb != null)
            {
                rb.linearVelocity  = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic     = true;
            }

            OnEntityDead?.Invoke(this);

            foreach (var comp in GetComponents<MonoBehaviour>())
                comp.enabled = false;
        }
    }

    public class EntityData
    {
        public string   Name;
        public string   Id;
        public string   HeadAddress;
        public string   BodyAddress;
        public string   FootAddress;
        public string   TrailEffectAddress;
        public string   SkillEffectAddress;
        public string   KillEffectAddress;
        public string   DeathEffectAddress;
        public string[] IdleEffects;
        public int      Heath;
        public int      Attack;
    }
}