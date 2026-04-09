using UnityEngine;
using Manager;

namespace Game
{
    public class DeadLane : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<Entity>(out var entity))
            {
                entity.OnDead();
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.TryGetComponent<Entity>(out var entity))
            {
                entity.OnDead();
            }
        }
    }
}