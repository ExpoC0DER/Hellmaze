using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _game.Scripts
{
    public class BasicGun : MonoBehaviour, IGun
    {
        [SerializeField] private GunSettings _gunSettings;
        [SerializeField] private Transform _shootPoint;
        [SerializeField] private Transform _hitPoint;

        public void Shoot()
        {
            if (Physics.Raycast(_shootPoint.position, _shootPoint.forward, out RaycastHit hit))
            {
                Instantiate(_hitPoint.gameObject, hit.point, Quaternion.identity);
            }
        }

    }
}
