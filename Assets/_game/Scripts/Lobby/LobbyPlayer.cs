using TMPro;
using UnityEngine;

namespace _game.Scripts.Lobby
{
    public class LobbyPlayer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _playerName;
        [SerializeField] private TextMeshProUGUI _readyText;

        private LobbyPlayerData _data;

        public void SetData(LobbyPlayerData data)
        {
            if (data == null)
            {
                gameObject.SetActive(false);
                return;
            }
            
            _data = data;
            _playerName.text = _data.Gamertag;

            _readyText.text = _data.IsReady ? "[READY]" : "[NOT READY]";

            gameObject.SetActive(true);
        }
    }
}
