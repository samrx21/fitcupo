using FitCupo.Classes.Application.Utilities.Mediator;

namespace FitCupo.Classes.Application.UseCases.ClassSessions.Commands.CancelClassSession;

/// <summary>
/// Comando para cancelar una sesión. <paramref name="CanCancelAnySession"/> es verdadero
/// para el administrador; un instructor solo puede cancelar sus propias sesiones.
/// </summary>
public sealed record CancelClassSessionCommand(
    Guid Id,
    string Reason,
    Guid RequestedBy,
    bool CanCancelAnySession) : IRequest;
