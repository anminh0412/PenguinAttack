using Plugins.Tick;
using UnityEngine;

namespace Manager
{
    public class BotController : Entity, ITickable, IShoot
    {
        public GamePlayManager gamePlayManager;

        public void RandomShot()
        {
            var dir   = PickRandomDirection();
            var scale = Random.Range(0f, 5f);
            var rot   = transform.eulerAngles;
            var targetRot = Quaternion.LookRotation(dir).eulerAngles;

            shotData = new ShotData
            {
                LaunchScale = scale,
                LaunchDir   = dir,
                Rot         = rot,
                Rot2        = targetRot
            };
        }

        private Vector3 PickRandomDirection()
        {
            var pick = Random.Range(0, 3);

            switch (pick)
            {
                case 0:
                    if (gamePlayManager?.plane != null)
                    {
                        var toCenter = gamePlayManager.plane.position - transform.position;
                        if (toCenter.sqrMagnitude > 0.001f)
                            return toCenter.normalized;
                    }
                    break;

                case 1:
                    if (gamePlayManager?.shootAbles != null && gamePlayManager.shootAbles.Count > 0)
                    {
                        var target = gamePlayManager.shootAbles[Random.Range(0, gamePlayManager.shootAbles.Count)];
                        if (target != null && target != this)
                        {
                            var toTarget = target.transform.position - transform.position;
                            if (toTarget.sqrMagnitude > 0.001f)
                                return toTarget.normalized;
                        }
                    }
                    break;
            }

            var randomDir = Random.insideUnitSphere;
            randomDir.y = 0f;
            return randomDir.normalized;
        }
    }
}
