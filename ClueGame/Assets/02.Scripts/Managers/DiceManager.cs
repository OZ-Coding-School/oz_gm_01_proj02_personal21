using UnityEngine;
using System.Collections;

namespace ClueGame.Managers
{
    public class DiceManager : MonoBehaviour
    {
        public static DiceManager Instance { get; private set; }

        [Header("Dice Settings")]
        [SerializeField] private int minValue = 1;
        [SerializeField] private int maxValue = 6;

        private int lastRollValue;
        public int LastRollValue => lastRollValue;

        // 주사위 굴림 이벤트
        public System.Action<int> OnDiceRolled;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // 주사위 굴리기
        public int RollDice()
        {
            lastRollValue = Random.Range(minValue, maxValue + 1);

            OnDiceRolled?.Invoke(lastRollValue);
            return lastRollValue;
        }

        // 애니메이션과 함께 주사위 굴리기
        public IEnumerator RollDiceWithAnimation(System.Action<int> callback)
        {
            // 주사위 굴리는 애니메이션 (숫자가 빠르게 변함)
            float duration = 1f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                int tempValue = Random.Range(minValue, maxValue + 1);
        
                elapsed += 0.1f;
                yield return new WaitForSeconds(0.1f);
            }

            // 최종 결과
            lastRollValue = Random.Range(minValue, maxValue + 1);
    

            OnDiceRolled?.Invoke(lastRollValue);
            callback?.Invoke(lastRollValue);
        }
    }
}