using UnityEngine;
using UnityEngine.UI;

namespace TKOF.Core
{
    public class MainMenuPanel : UIPanel
    {
        [SerializeField] private Button playButton;
        [SerializeField] private Button controlsButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private ControlsPanel controlsPanel;

        private void Awake()
        {
            playButton.onClick.AddListener(() => GameManager.Instance.StartGame());
            controlsButton.onClick.AddListener(() => controlsPanel.Show());
            quitButton.onClick.AddListener(Application.Quit);
        }

        protected override void OnShow() => controlsPanel.Hide();
    }
}