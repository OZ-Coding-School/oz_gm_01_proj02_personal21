using UnityEngine;

namespace ClueGame.Data
{
    // 카드 타입
    public enum CardType
    {
        Character,
        Weapon,
        Room
    }

    // 캐릭터 카드 (6종)
    public enum CharacterCard
    {
        MissScarlet,
        ColonelMustard,
        MrsWhite,
        MrGreen,
        MrsPeacock,
        ProfessorPlum
    }

    // 무기 카드 (6종)
    public enum WeaponCard
    {
        촛대,
        칼,
        납파이프,
        권총,
        밧줄,
        렌치
    }

    // 방 카드 (9종)
    public enum RoomCard
    {
        부엌,
        무도회장,
        온실,
        식당,
        당구장,
        도서관,
        라운지,
        홀,
        서재
    }

    // 카드 클래스
    [System.Serializable]
    public class Card
    {
        public CardType cardType;
        public string cardName;
        public int cardId;

        public Card(CardType type, string name, int id)
        {
            cardType = type;
            cardName = name;
            cardId = id;
        }

        public override string ToString()
        {
            return $"{cardType}: {cardName}";
        }
    }
}