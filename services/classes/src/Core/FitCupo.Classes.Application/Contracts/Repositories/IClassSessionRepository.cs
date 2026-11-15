using FitCupo.Classes.Application.UseCases.ClassSessions;
using FitCupo.Classes.Application.Utilities.Pagination;
using FitCupo.Classes.Domain.Entities.ClassSessions;
using FitCupo.Classes.Domain.Entities.ClassSessions.ValueObjects;

namespace FitCupo.Classes.Application.Contracts.Repositories;

/// <summary>
/// Operaciones propias del repositorio de sesiones.
/// </summary>
public interface IClassSessionRepository : IRepository<ClassSession>
{
    /// <summary>
    /// Indica si el salón ya tiene una sesión programada que se cruza con la franja.
    /// </summary>
    Task<bool> IsRoomBusyAsync(string room, TimeSlot schedule, CancellationToken cancellationToken = default);

    /// <summary>
    /// Indica si el instructor ya dicta una sesión programada que se cruza con la franja.
    /// </summary>
    Task<bool> IsInstructorBusyAsync(Guid instructorId, TimeSlot schedule, CancellationToken cancellationToken = default);

    /// <summary>
    /// Consulta de lectura: devuelve la sesión junto con el nombre y la categoría de su clase.
    /// </summary>
    Task<ClassSessionDto?> GetDtoByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Consulta de lectura del horario: sesiones que empiezan entre <paramref name="from"/> y
    /// <paramref name="to"/>, ordenadas por fecha.
    /// </summary>
    Task<PaginationResponse<ClassSessionDto>> GetScheduleAsync(
        PaginationRequest pagination,
        DateTime from,
        DateTime to,
        Guid? gymClassId,
        Guid? instructorId,
        ClassSessionStatus? status,
        CancellationToken cancellationToken = default);
}
