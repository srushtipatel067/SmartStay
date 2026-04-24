
/* =========================
   SP: get dashboard summary
   ========================= */
GO
CREATE OR ALTER PROCEDURE sp_Dashboard_Summary
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        (SELECT COUNT(*) FROM tbl_Hotels WHERE IsDeleted = 0) AS TotalHotels,

        (SELECT COUNT(*) FROM tbl_Rooms WHERE IsDeleted = 0) AS TotalRooms,

        (SELECT COUNT(*) FROM tbl_Bookings) AS TotalBookings,

        (SELECT COUNT(*) FROM tbl_Bookings WHERE BookingStatus = 'Confirmed') AS ConfirmedBookings,

        (SELECT COUNT(*) FROM tbl_Bookings WHERE BookingStatus = 'Completed') AS CompletedBookings,

        (SELECT COUNT(*) FROM tbl_Bookings WHERE BookingStatus = 'Cancelled') AS CancelledBookings,

        (SELECT ISNULL(SUM(TotalAmount),0) FROM tbl_Bookings WHERE PaymentStatus = 'Paid') AS TotalRevenue;
END;


/* =========================
   SP: get hotels with manager
   ========================= */
GO
CREATE OR ALTER PROCEDURE sp_Dashboard_HotelsWithManager
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        h.HotelId,
        h.HotelName,
        h.City,
        h.StarRating,
        u.UserId AS ManagerId,
        u.FullName AS ManagerName,
        u.Email AS ManagerEmail
    FROM tbl_Hotels h
    LEFT JOIN tbl_Users u ON h.CreatedBy = u.UserId
    WHERE h.IsDeleted = 0;
END;


/* =========================
   SP: get recent booking 
   ========================= */
GO
CREATE OR ALTER PROCEDURE sp_Dashboard_RecentBookings
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 10
        b.BookingId,
        b.GuestName,
        h.HotelName,
        r.RoomType,
        b.CheckInDate,
        b.CheckOutDate,
        b.TotalAmount,
        b.BookingStatus,
        b.PaymentStatus,
        b.CreatedAt
    FROM tbl_Bookings b
    JOIN tbl_Hotels h ON b.HotelId = h.HotelId
    JOIN tbl_Rooms r ON b.RoomId = r.RoomId
    ORDER BY b.CreatedAt DESC;
END;