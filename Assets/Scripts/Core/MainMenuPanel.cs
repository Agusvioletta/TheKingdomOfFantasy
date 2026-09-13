using UnityEngine;
using UnityEngine.UI;

namespace TKOF.Core
{
    public class MainMenuPanel : UIPanel
    {
        [SerializeField] private Button playButton;
        [SerializeField] private Button quitButton;

        private void Awake()
        {
            playButton.onClick.AddListener(() => GameManager.Instance.StartGame());
            quitButton.onClick.AddListener(Application.Quit);
        }
    }
}
