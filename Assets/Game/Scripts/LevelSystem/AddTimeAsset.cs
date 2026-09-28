using UnityEngine;

namespace Game.Scripts.LevelSystem
{
    [CreateAssetMenu(menuName = "Create AddTimeAsset", fileName = "AddTimeAsset", order = 0)]
    public class AddTimeAsset : ScriptableObject
    {
        [SerializeField] private AnimationCurve timeRewardPerScore = AnimationCurve.Constant(0, 80, 2f);
        
        public float GetTime(GameLevelController gameLevel)
        {
            var score = gameLevel.GetScore();
            var time = timeRewardPerScore.Evaluate(score);
            return time;
        }
    }
}