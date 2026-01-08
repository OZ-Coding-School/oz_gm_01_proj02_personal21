using UnityEngine;
using System.Collections.Generic;
using ClueGame.Data;
using ClueGame.Player;
using ClueGame.Managers;

namespace ClueGame.UI
{
    public class PlayerHandUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject cardPrefab;
        [SerializeField] private Transform cardContainer;

        private List<CardUI> cardUIList = new List<CardUI>();

        private void Start()
        {
            // 게임 시작 후 카드 표시
            Invoke(nameof(DisplayPlayerHand), 0.5f);
        }

        private void DisplayPlayerHand()
        {
            // 현재 플레이어(사람)의 카드 가져오기
            PlayerData player = GameManager.Instance.GetPlayers()[0]; // 첫 번째 플레이어

            ClearCards();

            foreach (var card in player.hand)
            {
                CreateCardUI(card);
            }

            Debug.Log($"{player.playerName}의 카드 {player.hand.Count}장 표시됨");
        }

        private void CreateCardUI(Card card)
        {
            GameObject cardObj = Instantiate(cardPrefab, cardContainer);
            CardUI cardUI = cardObj.GetComponent<CardUI>();

            if (cardUI != null)
            {
                cardUI.SetupCard(card);
                cardUIList.Add(cardUI);
            }
        }

        private void ClearCards()
        {
            foreach (var cardUI in cardUIList)
            {
                if (cardUI != null)
                {
                    Destroy(cardUI.gameObject);
                }
            }
            cardUIList.Clear();
        }
    }
}