namespace TKOF.Interactables
{
    /// <summary>
    /// Cualquier objeto que pueda iluminarse al mirarlo (no todos los
    /// IInteractable necesitan esto necesariamente, por eso es una interfaz
    /// separada en vez de meterlo en IInteractable).
    /// </summary>
    public interface IHighlightable
    {
        void SetHighlighted(bool value);
    }
}
