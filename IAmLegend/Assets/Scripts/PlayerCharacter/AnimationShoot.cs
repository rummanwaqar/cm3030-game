using UnityEngine;

namespace PlayerCharacter
{
    public class AnimationShoot : MonoBehaviour
    {
        public Animation anim;
        // Start is called before the first frame update
        void Start()
        {
            Transform mixTransform = transform.Find("Bip001 R Clavicle");
            anim["shoot_mix"].AddMixingTransform(mixTransform);
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
