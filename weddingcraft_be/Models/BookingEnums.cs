namespace weddingcraft_be.Models
{
    public enum BookingStatus
    {
        Draft,
        Pending,
        UnderReview,
        ModificationRequested,
        Confirmed,
        Rejected,
        PaymentPending,
        Paid,
        Booked,
        Completed,
        Cancelled,
        Refunded
    }

    public enum PaymentStatus
    {
        None,
        Pending,
        PartiallyPaid,
        Paid,
        Refunded
    }
}
