using System.Collections.Generic;
using UnityEngine;
using ClueGame.Data;
using System.Linq;

namespace ClueGame.Managers
{
    public class CardManager : MonoBehaviour
    {
        public static CardManager Instance { get; private set; }

        // 모든 카드 리스트
        private List<Card> allCards = new List<Card>();

        // 정답 카드 (봉투에 들어갈 카드)
        private Card solutionCharacter;
        private Card solutionWeapon;
        private Card solutionRoom;

        private void Awake()
        {
            // 싱글톤 패턴
            if (Instance == null)
            {
                Instance = this;
                // 재시작 오류로 주석 처리
                //DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            InitializeCards();
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
        // 모든 카드 생성
        private void InitializeCards()
        {
            allCards.Clear();
            int cardId = 0;

            // 캐릭터 카드 생성 (6장)
            foreach (CharacterCard character in System.Enum.GetValues(typeof(CharacterCard)))
            {
                allCards.Add(new Card(CardType.Character, character.ToString(), cardId++));
            }

            // 무기 카드 생성 (6장)
            foreach (WeaponCard weapon in System.Enum.GetValues(typeof(WeaponCard)))
            {
                allCards.Add(new Card(CardType.Weapon, weapon.ToString(), cardId++));
            }

            // 방 카드 생성 (9장)
            foreach (RoomCard room in System.Enum.GetValues(typeof(RoomCard)))
            {
                allCards.Add(new Card(CardType.Room, room.ToString(), cardId++));
            }


            // 정답 설정
            SetupSolution();
        }

        // 정답 카드 설정 (각 타입에서 1장씩 랜덤 선택)
        private void SetupSolution()
        {
            // 캐릭터 카드 중 1장
            var characterCards = allCards.Where(c => c.cardType == CardType.Character).ToList();
            solutionCharacter = characterCards[Random.Range(0, characterCards.Count)];

            // 무기 카드 중 1장
            var weaponCards = allCards.Where(c => c.cardType == CardType.Weapon).ToList();
            solutionWeapon = weaponCards[Random.Range(0, weaponCards.Count)];

            // 방 카드 중 1장
            var roomCards = allCards.Where(c => c.cardType == CardType.Room).ToList();
            solutionRoom = roomCards[Random.Range(0, roomCards.Count)];

 
        }

        // 정답 확인
        public bool CheckSolution(Card character, Card weapon, Card room)
        {
            return character.cardId == solutionCharacter.cardId &&
                   weapon.cardId == solutionWeapon.cardId &&
                   room.cardId == solutionRoom.cardId;
        }

        // 정답 카드 가져오기
        public Card GetAnswerCard(CardType type)
        {
            switch (type)
            {
                case CardType.Character:
                    return solutionCharacter;

                case CardType.Weapon:
                    return solutionWeapon;

                case CardType.Room:
                    return solutionRoom;

                default:
                    return null;
            }
        }

        // 플레이어들에게 나눠줄 카드 리스트 반환 (정답 제외)
        public List<Card> GetCardsForDistribution()
        {
            var cardsToDistribute = new List<Card>(allCards);

            // 정답 카드는 제외
            cardsToDistribute.Remove(solutionCharacter);
            cardsToDistribute.Remove(solutionWeapon);
            cardsToDistribute.Remove(solutionRoom);

            // 섞기
            ShuffleCards(cardsToDistribute);

            return cardsToDistribute;
        }

        // 카드 섞기
        private void ShuffleCards(List<Card> cards)
        {
            for (int i = 0; i < cards.Count; i++)
            {
                int randomIndex = Random.Range(i, cards.Count);
                Card temp = cards[i];
                cards[i] = cards[randomIndex];
                cards[randomIndex] = temp;
            }
        }

        // 전체 카드 리스트 반환
        public List<Card> GetAllCards()
        {
            return new List<Card>(allCards);
        }
    }
}