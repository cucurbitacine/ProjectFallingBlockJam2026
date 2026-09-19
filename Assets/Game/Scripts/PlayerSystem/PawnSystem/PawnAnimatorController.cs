using UnityEngine;

namespace Game.Scripts.PlayerSystem.PawnSystem
{
    public class PawnAnimatorController : MonoBehaviour
    {
        [SerializeField] private PawnController pawn;
        
        [Header("Animator")]
        [SerializeField] private Animator animator;
        
        private static readonly int IsMoving = Animator.StringToHash("IsMoving");
        private static readonly int Grounded = Animator.StringToHash("Grounded");
        
        private void Flip(float x)
        {
            var scale = transform.localScale;
            
            if (x > 0f)
            {
                scale.x = 1f;
            }
            else if(x < 0f)
            {
                scale.x = -1f;
            }

            transform.localScale = scale;
        }
        
        private void LateUpdate()
        {
            if (pawn.IsMoving)
            {
                Flip(pawn.MoveInput.x);
            }
            
            animator.SetBool(IsMoving, pawn.IsMoving);
            animator.SetBool(Grounded, pawn.Grounded);
        }
    }
}