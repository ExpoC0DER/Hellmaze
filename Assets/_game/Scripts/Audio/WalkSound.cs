using UnityEngine;

namespace _game.Scripts
{
    public class WalkSound : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Floor"))
                print("floor");
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Floor"))
                print("floorE");
        }
        
        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Floor"))
                print("floorS");
        }
    }
}
