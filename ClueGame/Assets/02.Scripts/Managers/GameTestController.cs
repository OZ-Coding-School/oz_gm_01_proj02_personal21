using UnityEngine;
using ClueGame.Data;
using ClueGame.Player;

namespace ClueGame.Managers
{
    public class GameTestController : MonoBehaviour
    {
        private void Start()
        {
            //Debug.Log("GameTestController 시작!");
            //Debug.Log("===== 조작키 =====");
            //Debug.Log("Space: 주사위 굴리기");
            //Debug.Log("방향키: 한 칸씩 이동");
            //Debug.Log("M: AI 자동 이동");
            //Debug.Log("P: 비밀 통로 사용");
            //Debug.Log("S: 제안하기");
            //Debug.Log("A: 고발하기");
            //Debug.Log("N: 다음 턴");
            //Debug.Log("==================");
        }

        private void Update()
        {
            // Space: 주사위 굴리기
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (TurnManager.Instance.GetCurrentPhase() == GamePhase.RollingDice)
                {
                    TurnManager.Instance.RollDice();
                }
            }

            // 방향키: 이동
            if (TurnManager.Instance.GetCurrentPhase() == GamePhase.Moving)
            {
                PlayerData currentPlayer = TurnManager.Instance.GetCurrentPlayer();
                bool moved = false;

                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    moved = MovementManager.Instance.MovePlayerOneStep(currentPlayer, Vector2Int.up);
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    moved = MovementManager.Instance.MovePlayerOneStep(currentPlayer, Vector2Int.down);
                }
                else if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    moved = MovementManager.Instance.MovePlayerOneStep(currentPlayer, Vector2Int.left);
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    moved = MovementManager.Instance.MovePlayerOneStep(currentPlayer, Vector2Int.right);
                }

                if (moved)
                {
                    TurnManager.Instance.OnMoveComplete();
                }
            }

            // M: AI 자동 이동
            if (Input.GetKeyDown(KeyCode.M))
            {
                if (TurnManager.Instance.GetCurrentPhase() == GamePhase.Moving)
                {
                    PlayerData currentPlayer = TurnManager.Instance.GetCurrentPlayer();
                    int moves = TurnManager.Instance.RemainingMoves;

                    MovementManager.Instance.MovePlayerRandomly(currentPlayer, moves);
                    TurnManager.Instance.OnMoveComplete();
                }
            }

            // ===== P: 비밀 통로 사용=====
            if (Input.GetKeyDown(KeyCode.P))
            {
                PlayerData currentPlayer = TurnManager.Instance.GetCurrentPlayer();

                if (MovementManager.Instance.UseSecretPassage(currentPlayer))
                {
                    // 비밀 통로 사용하면 바로 InRoom 페이즈로
                    if (TurnManager.Instance.GetCurrentPhase() == GamePhase.Moving)
                    {
                        TurnManager.Instance.OnMoveComplete();
                    }
                }
            }
            // ====================================

            // S: 제안하기
            if (Input.GetKeyDown(KeyCode.S))
            {
                if (TurnManager.Instance.GetCurrentPhase() == GamePhase.InRoom)
                {
                    PlayerData currentPlayer = TurnManager.Instance.GetCurrentPlayer();
                    var allCards = CardManager.Instance.GetAllCards();

                    var charCard = allCards.Find(c => c.cardType == CardType.Character);
                    var weapCard = allCards.Find(c => c.cardType == CardType.Weapon);

                    if (currentPlayer.currentRoom.HasValue)
                    {
                        var roomCard = allCards.Find(c => c.cardType == CardType.Room &&
                                                          c.cardName == currentPlayer.currentRoom.ToString());

                        SuggestionManager.Instance.MakeSuggestion(charCard, weapCard, roomCard);
                        TurnManager.Instance.OnSuggestionComplete();
                    }
                }
            }

            // A: 고발하기
            if (Input.GetKeyDown(KeyCode.A))
            {
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
                TurnManager.Instance.EndTurn();
            }
        }
    }
}