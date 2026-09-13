using UnityEngine;
using TKOF.Core;

namespace TKOF.Interactables
{
    public class Photograph : Collectible
    {
        [SerializeField] private Sprite photoSprite;
        [TextArea] [SerializeField] private string caption;
        protected override void ShowContent() => UIReadingPanel.Instance.ShowImage(photoSprite, caption);
    }
}
