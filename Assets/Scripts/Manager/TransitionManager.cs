using Cysharp.Threading.Tasks;
using DG.Tweening;
using Other;
using UnityEngine.UI;

namespace Manager
{
    public class TransitionManager : MonoSingleton<TransitionManager>
    {
        public Image bg;

        public async UniTask Intro(float duration, float delay = 0)
        {
            this.bg.DOFade(0, 0);
            this.bg.gameObject.SetActive(true);
            this.bg.DOFade(1, duration).SetDelay(delay);
            await UniTask.WaitForSeconds(duration + delay);
        }
        
        public async UniTask Outro(float duration, float delay = 0)
        {
            bg.DOFade(0, duration).SetDelay(delay);
            await UniTask.WaitForSeconds(duration + delay);
            this.bg.gameObject.SetActive(false);
        }
    }
}