using FitCupo.Classes.Application.Utilities.Mediator;

namespace FitCupo.Classes.Application.UseCases.GymClasses.Queries.GetGymClassById;

/// <summary>
/// Query para consultar una clase del catálogo.
/// </summary>
public sealed record GetGymClassByIdQuery(Guid Id) : IRequest<GymClassDto>;
