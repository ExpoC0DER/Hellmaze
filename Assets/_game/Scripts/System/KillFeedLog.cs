using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using EditorAttributes;

namespace _game.Scripts.System
{
    public class KillFeedLog : MonoBehaviour
    {
        [FormerlySerializedAs("killerName_txt")]
        [SerializeField] private TextMeshProUGUI _killerNameText;
        [FormerlySerializedAs("victimName_txt")]
        [SerializeField] private TextMeshProUGUI _killedNameText;
        [FormerlySerializedAs("killIcon")]
        [SerializeField] private Image _deathIcon;
        [SerializeField] private CanvasGroup _canvasGroup;

        [Button]
        public void Spawn(string killerName, string victimName, Sprite icon)
        {
            _killerNameText.text = killerName;
            _killedNameText.text = victimName;
            _deathIcon.sprite = icon;

            gameObject.SetActive(true);
            _canvasGroup.alpha = 1f;
            _canvasGroup.DOKill();
            _canvasGroup.DOFade(0, 2f).SetDelay(3f).OnComplete(() => gameObject.SetActive(false));
        }
    }
}
