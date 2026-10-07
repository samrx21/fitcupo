using FitCupo.Clients.Application.Contracts.Repositories;
using FitCupo.Clients.Application.Utilities.Pagination;
using FitCupo.Clients.Domain.Entities.Clients;
using FitCupo.Clients.Domain.Entities.Clients.ValueObjects;
using FitCupo.Clients.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace FitCupo.Clients.Persistence.Repositories;

public sealed class ClientRepository(ClientsDbContext context) : Repository<Client>(context), IClientRepository
{
    public async Task<bool> ExistsByDocumentAsync(
        DocumentType documentType, string documentNumber, CancellationToken cancellationToken = default) =>
        await Context.Clients.AnyAsync(
            client => client.Document.Type == documentType && client.Document.Number == documentNumber,
            cancellationToken);

    public async Task<bool> ExistsByEmailAsync(
        string email, Guid? excludedClientId = null, CancellationToken cancellationToken = default) =>
        await Context.Clients.AnyAsync(
            client => client.Email.Value == email && client.Id != excludedClientId,
            cancellationToken);

    public async Task<PaginationResponse<Client>> GetPagedListAsync(
        PaginationRequest pagination,
        string? search,
        ClientStatus? status,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Client> query = Context.Clients.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            string term = search.Trim();
            query = query.Where(client =>
                client.FirstName.Contains(term)
                || client.LastName.Contains(term)
                || client.Document.Number.Contains(term)
                || client.Email.Value.Contains(term));
        }

        if (status.HasValue)
            query = query.Where(client => client.Status == status.Value);

        query = query
            .OrderBy(client => client.LastName)
            .ThenBy(client => client.FirstName)
            .ThenBy(client => client.Id);

        return await query.ToPagedListAsync(pagination, cancellationToken);
    }
}
