using UnityEngine;
using TMPro;
using ClueGame.Data;

namespace ClueGame.Board
{
    public class RoomLabel : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private TextMeshPro labelText;

        public void Initialize(RoomCard room, Vector3 position)
        {
            if (labelText == null)
            {
                GameObject textObj = new GameObject("Label");
                textObj.transform.SetParent(transform);
                textObj.transform.localPosition = Vector3.zero;

                labelText = textObj.AddComponent<TextMeshPro>();
                labelText.fontSize = 3;
                labelText.alignment = TextAlignmentOptions.Center;
                labelText.color = Color.black;
            }

            labelText.text = room.ToString();
            transform.position = position + new Vector3(0, 0, -2); // 앞에 표시
        }
    }
}