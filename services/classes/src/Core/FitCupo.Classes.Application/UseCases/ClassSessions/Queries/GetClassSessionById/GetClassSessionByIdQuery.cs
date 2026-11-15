using FitCupo.Classes.Application.Utilities.Mediator;

namespace FitCupo.Classes.Application.UseCases.ClassSessions.Queries.GetClassSessionById;

/// <summary>
/// Query para consultar una sesión con los datos de su clase.
/// </summary>
public sealed record GetClassSessionByIdQuery(Guid Id) : IRequest<ClassSessionDto>;
