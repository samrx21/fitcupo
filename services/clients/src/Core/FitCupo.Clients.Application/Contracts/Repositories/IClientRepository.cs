using FitCupo.Clients.Application.Utilities.Pagination;
using FitCupo.Clients.Domain.Entities.Clients;
using FitCupo.Clients.Domain.Entities.Clients.ValueObjects;

namespace FitCupo.Clients.Application.Contracts.Repositories;

/// <summary>
/// Operaciones propias del repositorio de clientes.
/// </summary>
public interface IClientRepository : IRepository<Client>
{
    Task<bool> ExistsByDocumentAsync(
        DocumentType documentType, string documentNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Indica si el correo ya pertenece a otro cliente. <paramref name="excludedClientId"/>
    /// permite ignorar al propio cliente cuando se actualizan sus datos.
    /// </summary>
    Task<bool> ExistsByEmailAsync(
        string email, Guid? excludedClientId = null, CancellationToken cancellationToken = default);

    Task<PaginationResponse<Client>> GetPagedListAsync(
        PaginationRequest pagination,
        string? search,
        ClientStatus? status,
        CancellationToken cancellationToken = default);
}
