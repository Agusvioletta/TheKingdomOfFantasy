using UnityEngine;
using TKOF.Core;

namespace TKOF.Interactables
{
    public class Diary : Collectible
    {
        [TextArea] [SerializeField] private string entryText;
        protected override void ShowContent() => UIReadingPanel.Instance.ShowText(entryText);
    }
}
