using System.Collections.Generic;
using EditorAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace _game.Scripts.Player
{
    public class NetworkPlayerAnimations : MonoBehaviour
    {
        [SerializeField] private Animator _animator;

        [SerializeField] private int _characterSelected = 0;
        [SerializeField] private GameObject[] _characters;
        [SerializeField] private Avatar[] _characterAvatars;
        [SerializeField] private GameObject[] _weaponsModels;
        [SerializeField] private List<TransformArray> _weaponPositions = new List<TransformArray>();

        private int _weaponSet;

        private static readonly int AnimatorSpeedX = Animator.StringToHash("SpeedX");
        private static readonly int AnimatorSpeedY = Animator.StringToHash("SpeedY");

        public void AnimateMovement(Vector2 movementInput)
        {
            _animator.SetFloat(AnimatorSpeedX, movementInput.x);
            _animator.SetFloat(AnimatorSpeedY, movementInput.y);
        }

        [Button]
        public void SetWeapon(int index)
        {
            for(int i = 0; i < _weaponsModels.Length; i++)
            {
                _weaponsModels[i].SetActive(i == index);
            }
            
            _weaponSet = index;
        }

        public void SetAvatar(int index)
        {
            _animator.enabled = false;

            if (index == -1)
            {
                for(int i = 0; i < _characters.Length; i++)
                {
                    _characters[i].SetActive(i == index);
                }
            }

            for(int i = 0; i < _characters.Length; i++)
            {
                _characters[i].SetActive(i == index);
            }

            for(int i = 0; i < _weaponsModels.Length; i++)
            {
                _weaponsModels[i].transform.SetParent(_weaponPositions[index].Transforms[i], false);
            }

            _animator.avatar = _characterAvatars[index];
            _animator.enabled = true;
        }
    }
}
