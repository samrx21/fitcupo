using FitCupo.Bookings.Application.Utilities.Pagination;
using FitCupo.Bookings.Domain.Entities.Bookings;

namespace FitCupo.Bookings.Application.UseCases.Bookings;

/// <summary>
/// Convierte la entidad Booking en su DTO; la entidad del dominio nunca sale de la API.
/// </summary>
public static class BookingMapperExtensions
{
    public static BookingDto ToDto(this Booking booking) => new()
    {
        Id = booking.Id,
        SessionId = booking.SessionId,
        GymClassName = booking.GymClassName,
        Room = booking.Room,
        InstructorId = booking.InstructorId,
        InstructorName = booking.InstructorName,
        StartsAt = booking.StartsAt,
        EndsAt = booking.EndsAt,
        ClientId = booking.ClientId,
        ClientName = booking.ClientName,
        ClientEmail = booking.ClientEmail,
        Status = booking.Status,
        BookedAt = booking.BookedAt,
        CancelledAt = booking.CancelledAt
    };

    public static PaginationResponse<BookingDto> ToDto(this PaginationResponse<Booking> page) =>
        new()
        {
            Items = page.Items.Select(ToDto).ToList(),
            TotalCount = page.TotalCount,
            PageNumber = page.PageNumber,
            PageSize = page.PageSize
        };
}
