namespace TKOF.Interactables
{
    /// <summary>
    /// INTERFAZ (caso 1): contrato que implementa todo objeto con el que el
    /// jugador puede interactuar con "E" (diarios, fotos, ticket, panel del
    /// carrusel). El InteractionController no necesita saber de qué tipo
    /// concreto es el objeto, solo que cumple este contrato.
    /// </summary>
    public interface IInteractable
    {
        string InteractionPrompt { get; }
        void Interact();
    }
}
