using Game.Scripts.Figures;
using Game.Scripts.LevelSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UISystem
{
    public class NextFigureUI : MonoBehaviour
    {
        [SerializeField] private Image nextFigureImage;

        private GameLevelController _gameLevel;
        
        public void SetupUI(GameLevelController gameLevel)
        {
            _gameLevel = gameLevel;
        }

        public void EnableUI()
        {
            _gameLevel.GetNextFigure(out var nextFigureType, out var nextFigureRotation);
            OnNextFigureChanged(nextFigureType, nextFigureRotation);
            _gameLevel.NextFigureChanged += OnNextFigureChanged;
            nextFigureImage.gameObject.SetActive(true);
        }

        public void DisableUI()
        {
            _gameLevel.NextFigureChanged -= OnNextFigureChanged;
            //nextFigureImage.gameObject.SetActive(false);
        }
        
        private void OnNextFigureChanged(FigureType figureType, FigureRotation figureRotation)
        {
            var figureAsset = _gameLevel.GetGridWorld().GetFigureStorage().GetFigureProfile(figureType).GetFigureAsset();
            nextFigureImage.sprite = figureAsset.GetIcon();
            nextFigureImage.transform.localRotation = Quaternion.Euler(0f, 0f, 90f * (int)figureRotation);
        }
    }
}