using UnityEngine;
using ClueGame.Data;

namespace ClueGame.Managers
{
    public class GameTestController : MonoBehaviour
    {
        private void Start()
        {
            Debug.Log("GameTestController 시작!");
        }

        private void Update()
        {
            // Space: 주사위 굴리기
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log($"Space 키 입력! 현재 Phase: {TurnManager.Instance.GetCurrentPhase()}");

                if (TurnManager.Instance.GetCurrentPhase() == GamePhase.RollingDice)
                {
                    TurnManager.Instance.RollDice();
                    Debug.Log("주사위를 굴렸습니다!");
                }
                else
                {
                    Debug.LogWarning($"주사위를 굴릴 수 없는 Phase입니다: {TurnManager.Instance.GetCurrentPhase()}");
                }
            }

            // S: 제안하기 (테스트용 랜덤 제안)
            if (Input.GetKeyDown(KeyCode.S))
            {
                Debug.Log($"S 키 입력! 현재 Phase: {TurnManager.Instance.GetCurrentPhase()}");

                if (TurnManager.Instance.GetCurrentPhase() == GamePhase.InRoom)
                {
                    var allCards = CardManager.Instance.GetAllCards();

                    var charCard = allCards.Find(c => c.cardType == CardType.Character);
                    var weapCard = allCards.Find(c => c.cardType == CardType.Weapon);
                    var roomCard = allCards.Find(c => c.cardType == CardType.Room);

                    SuggestionManager.Instance.MakeSuggestion(charCard, weapCard, roomCard);
                    TurnManager.Instance.OnSuggestionComplete();
                }
            }

            // A: 고발하기 (테스트용 랜덤 고발)
            if (Input.GetKeyDown(KeyCode.A))
            {
                Debug.Log($"A 키 입력!");

                var allCards = CardManager.Instance.GetAllCards();

                var charCard = allCards.Find(c => c.cardType == CardType.Character);
                var weapCard = allCards.Find(c => c.cardType == CardType.Weapon);
                var roomCard = allCards.Find(c => c.cardType == CardType.Room);

                bool result = AccusationManager.Instance.MakeAccusation(charCard, weapCard, roomCard);
                TurnManager.Instance.OnAccusationComplete(result);
            }

            // N: 다음 턴
            if (Input.GetKeyDown(KeyCode.N))
            {
                Debug.Log("N 키 입력! 다음 턴으로...");
                TurnManager.Instance.EndTurn();
            }
        }
    }
}