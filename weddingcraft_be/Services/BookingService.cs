using Microsoft.EntityFrameworkCore;
using weddingcraft_be.Data;
using weddingcraft_be.Dtos;
using weddingcraft_be.Models;
using weddingcraft_be.Interfaces.Services;
using Npgsql;
using System.Data;

namespace weddingcraft_be.Services
{
    public interface IBookingService
    {
        Task<BookingResponseDto> CreateBookingAsync(Guid customerId, BookingRequestDto request);
        Task<bool> ApproveBookingAsync(int bookingId, Guid vendorId, string? notes);
        Task<bool> RejectBookingAsync(int bookingId, Guid vendorId, string? notes);
        Task<bool> RequestModificationAsync(int bookingId, Guid vendorId, string? notes);
        Task<bool> ConfirmPaymentAsync(int bookingId, Guid customerId);
        Task<List<BookingResponseDto>> GetCustomerBookingsAsync(Guid customerId);
        Task<List<BookingResponseDto>> GetVendorBookingsAsync(Guid vendorId);
        Task<BookingResponseDto?> GetBookingByIdAsync(int id);
        Task<bool> CancelBookingAsync(int bookingId, Guid userId, string? notes);
    }

    public class BookingService : IBookingService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly INotificationService _notificationService;

        public BookingService(
            ApplicationDbContext context, 
            IEmailService emailService, 
            INotificationService notificationService)
        {
            _context = context;
            _emailService = emailService;
            _notificationService = notificationService;
        }

        public async Task<BookingResponseDto> CreateBookingAsync(Guid customerId, BookingRequestDto request)
        {
            var reference = $"BK-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}";
            
            var bookingIdParam = new NpgsqlParameter("p_booking_id", SqlDbType.Int) { Direction = ParameterDirection.Output };
            
            await _context.Database.ExecuteSqlRawAsync(
                "CALL sp_create_booking_request(@p_reference, @p_customer_id, @p_vendor_id, @p_product_id, @p_event_date, @p_event_time, @p_location, @p_guest_count, @p_requirements, @p_budget, @p_notes, @p_total_amount, @p_booking_id)",
                new NpgsqlParameter("@p_reference", reference),
                new NpgsqlParameter("@p_customer_id", customerId),
                new NpgsqlParameter("@p_vendor_id", request.VendorId),
                new NpgsqlParameter("@p_product_id", (object?)request.ProductId ?? DBNull.Value),
                new NpgsqlParameter("@p_event_date", request.EventDate),
                new NpgsqlParameter("@p_event_time", request.EventTime),
                new NpgsqlParameter("@p_location", request.Location),
                new NpgsqlParameter("@p_guest_count", request.GuestCount),
                new NpgsqlParameter("@p_requirements", request.Requirements),
                new NpgsqlParameter("@p_budget", request.Budget),
                new NpgsqlParameter("@p_notes", request.Notes),
                new NpgsqlParameter("@p_total_amount", request.TotalAmount),
                bookingIdParam
            );

            var bookingId = (int)bookingIdParam.Value!;
            var booking = (await GetBookingByIdAsync(bookingId))!;

            // Notify Vendor
            var vendor = await _context.Users.FindAsync(request.VendorId);
            if (vendor != null)
            {
                await _notificationService.NotifyUserAsync(vendor.Id, "New Booking Request", $"You have a new request from {booking.CustomerName} for {booking.EventDate:dd MMM yyyy}.", "success");
                await _emailService.SendAsync(vendor.Email, "New WeddingCraft Booking Request", 
                    $"<h1>New Request</h1><p>You have a new request for {booking.ServiceName} on {booking.EventDate:dd MMM yyyy}. View it in your dashboard.</p>");
            }

            return booking;
        }

        public async Task<bool> ApproveBookingAsync(int bookingId, Guid vendorId, string? notes)
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "CALL sp_approve_booking(@p_booking_id, @p_vendor_id, @p_notes)",
                    new NpgsqlParameter("@p_booking_id", bookingId),
                    new NpgsqlParameter("@p_vendor_id", vendorId),
                    new NpgsqlParameter("@p_notes", (object?)notes ?? DBNull.Value)
                );

                // Notify Customer
                var booking = await _context.Bookings.Include(b => b.Customer).FirstOrDefaultAsync(b => b.Id == bookingId);
                if (booking != null)
                {
                    await _notificationService.NotifyUserAsync(booking.CustomerId, "Booking Confirmed!", "Your booking has been approved. Please proceed to payment.", "success");
                    await _emailService.SendAsync(booking.Customer.Email, "Wedding Booking Approved!", 
                        $"<h1>Great News!</h1><p>Your booking with {booking.VendorId} has been approved. You can now complete the payment in your dashboard.</p>");
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> RejectBookingAsync(int bookingId, Guid vendorId, string? notes)
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "CALL sp_reject_booking(@p_booking_id, @p_vendor_id, @p_notes)",
                    new NpgsqlParameter("@p_booking_id", bookingId),
                    new NpgsqlParameter("@p_vendor_id", vendorId),
                    new NpgsqlParameter("@p_notes", (object?)notes ?? DBNull.Value)
                );

                // Notify Customer
                var booking = await _context.Bookings.Include(b => b.Customer).FirstOrDefaultAsync(b => b.Id == bookingId);
                if (booking != null)
                {
                    await _notificationService.NotifyUserAsync(booking.CustomerId, "Booking Update", "Your booking request was unfortunately rejected by the vendor.", "error");
                }

                return true;
            }
            catch { return false; }
        }

        public async Task<bool> RequestModificationAsync(int bookingId, Guid vendorId, string? notes)
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "CALL sp_request_booking_modification(@p_booking_id, @p_vendor_id, @p_notes)",
                    new NpgsqlParameter("@p_booking_id", bookingId),
                    new NpgsqlParameter("@p_vendor_id", vendorId),
                    new NpgsqlParameter("@p_notes", (object?)notes ?? DBNull.Value)
                );

                // Notify Customer
                var booking = await _context.Bookings.Include(b => b.Customer).FirstOrDefaultAsync(b => b.Id == bookingId);
                if (booking != null)
                {
                    await _notificationService.NotifyUserAsync(booking.CustomerId, "Action Required", "The vendor has requested a modification to your booking details.", "warning");
                }

                return true;
            }
            catch { return false; }
        }

        public async Task<bool> ConfirmPaymentAsync(int bookingId, Guid customerId)
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "CALL sp_confirm_payment(@p_booking_id, @p_customer_id)",
                    new NpgsqlParameter("@p_booking_id", bookingId),
                    new NpgsqlParameter("@p_customer_id", customerId)
                );
                return true;
            }
            catch { return false; }
        }

        public async Task<bool> CancelBookingAsync(int bookingId, Guid userId, string? notes)
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "CALL sp_cancel_booking(@p_booking_id, @p_user_id, @p_notes)",
                    new NpgsqlParameter("@p_booking_id", bookingId),
                    new NpgsqlParameter("@p_user_id", userId),
                    new NpgsqlParameter("@p_notes", (object?)notes ?? DBNull.Value)
                );
                return true;
            }
            catch { return false; }
        }

        public async Task<List<BookingResponseDto>> GetCustomerBookingsAsync(Guid customerId)
        {
            return await _context.Bookings
                .Where(b => b.CustomerId == customerId)
                .Include(b => b.Vendor)
                .Include(b => b.Product)
                .Include(b => b.StatusHistory)
                .OrderByDescending(b => b.CreatedAt)
                .Select(b => MapToDto(b))
                .ToListAsync();
        }

        public async Task<List<BookingResponseDto>> GetVendorBookingsAsync(Guid vendorId)
        {
            return await _context.Bookings
                .Where(b => b.VendorId == vendorId)
                .Include(b => b.Customer)
                .Include(b => b.Product)
                .Include(b => b.StatusHistory)
                .OrderByDescending(b => b.CreatedAt)
                .Select(b => MapToDto(b))
                .ToListAsync();
        }

        public async Task<BookingResponseDto?> GetBookingByIdAsync(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Vendor)
                .Include(b => b.Product)
                .Include(b => b.StatusHistory)
                .FirstOrDefaultAsync(b => b.Id == id);

            return booking == null ? null : MapToDto(booking);
        }

        private static BookingResponseDto MapToDto(Booking b)
        {
            return new BookingResponseDto
            {
                Id = b.Id,
                BookingReference = b.BookingReference,
                CustomerId = b.CustomerId,
                CustomerName = b.Customer?.Name ?? "Unknown",
                VendorId = b.VendorId,
                VendorName = b.Vendor?.VendorProfile?.BusinessName ?? b.Vendor?.Name ?? "Unknown",
                ServiceName = b.Product?.Name ?? "General Service",
                EventDate = b.EventDate,
                EventTime = b.EventTime,
                Location = b.Location,
                GuestCount = b.GuestCount,
                Status = b.Status,
                PaymentStatus = b.PaymentStatus,
                TotalAmount = b.TotalAmount,
                CreatedAt = b.CreatedAt,
                StatusHistory = b.StatusHistory.Select(h => new BookingStatusHistoryDto
                {
                    OldStatus = h.OldStatus,
                    NewStatus = h.NewStatus,
                    ChangedBy = h.ChangedBy,
                    ChangedAt = h.ChangedAt,
                    Notes = h.Notes
                }).OrderByDescending(h => h.ChangedAt).ToList()
            };
        }
    }
}
