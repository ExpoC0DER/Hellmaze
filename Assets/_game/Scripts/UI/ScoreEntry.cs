using TMPro;
using UnityEngine;

namespace _game.Scripts.UI
{
    public class ScoreEntry : MonoBehaviour
    {
        [SerializeField] private TMP_Text _playerNameText;
        [SerializeField] private TMP_Text _killsText;
        [SerializeField] private TMP_Text _deathsText;
        [SerializeField] private TMP_Text _assistsText;

        private string _playerName;
        private int _kills;
        private int _deaths;
        private int _assists;

        public string PlayerName
        {
            get { return _playerName; }
            set
            {
                _playerName = value;
                _playerNameText.text = value;
            }
        }

        public int Kills
        {
            get { return _kills; }
            set
            {
                _kills = value;
                _killsText.text = value.ToString();
            }
        }

        public int Deaths
        {
            get { return _deaths; }
            set
            {
                _deaths = value;
                _deathsText.text = value.ToString();
            }
        }

        public int Assists
        {
            get { return _assists; }
            set
            {
                _assists = value;
                _assistsText.text = value.ToString();
            }
        }
    }
}
