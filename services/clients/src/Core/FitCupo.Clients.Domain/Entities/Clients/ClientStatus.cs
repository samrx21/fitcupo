namespace FitCupo.Clients.Domain.Entities.Clients;

/// <summary>
/// Estado del cliente en el gimnasio. Un cliente inactivo no puede reservar
/// clases ni modificar sus datos hasta que se reactive.
/// </summary>
public enum ClientStatus
{
    Active = 1,
    Inactive = 2
}
