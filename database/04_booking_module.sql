/* =========================================================
   04_booking_module.sql
   SmartStay - booking Module
   ========================================================= */

/* =========================
   TABLE: tbl_Bookings
   ========================= */
GO
CREATE TABLE tbl_Bookings
(
    BookingId INT IDENTITY(1,1) PRIMARY KEY,

    UserId INT NULL, -- nullable for guest booking

    GuestName NVARCHAR(150) NOT NULL,
    GuestEmail NVARCHAR(150) NOT NULL,
    GuestPhone NVARCHAR(20) NOT NULL,

    HotelId INT NOT NULL,
    RoomId INT NOT NULL,

    CheckInDate DATE NOT NULL,
    CheckOutDate DATE NOT NULL,

    Adults INT NOT NULL,
    Children INT NOT NULL,

    RoomsBooked INT NOT NULL,

    PricePerNight DECIMAL(10,2) NOT NULL,
    TotalNights INT NOT NULL,
    TotalAmount DECIMAL(12,2) NOT NULL,

    SpecialRequest NVARCHAR(500) NULL,

    BookingStatus NVARCHAR(20) NOT NULL DEFAULT 'Pending',
    PaymentStatus NVARCHAR(20) NOT NULL DEFAULT 'Pending',

    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),

    FOREIGN KEY (UserId) REFERENCES tbl_Users(UserId),
    FOREIGN KEY (HotelId) REFERENCES tbl_Hotels(HotelId),
    FOREIGN KEY (RoomId) REFERENCES tbl_Rooms(RoomId)
);


/* =========================
   SP: Create Booking
   ========================= */
GO
CREATE OR ALTER PROCEDURE sp_Booking_Create
(
    @UserId INT = NULL,

    @GuestName NVARCHAR(150),
    @GuestEmail NVARCHAR(150),
    @GuestPhone NVARCHAR(20),

    @HotelId INT,
    @RoomId INT,

    @CheckInDate DATE,
    @CheckOutDate DATE,

    @Adults INT,
    @Children INT,
    @RoomsBooked INT,

    @SpecialRequest NVARCHAR(500) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    -- Validate dates
    IF @CheckInDate >= @CheckOutDate
    BEGIN
        SELECT 0 AS Success, 'Invalid date range' AS Message;
        RETURN;
    END

    -- Prevent past booking
    -- check in date can not be in past
    DECLARE @Today DATE = CAST(GETDATE() AS DATE);    

    IF @CheckInDate < @Today
    BEGIN
        SELECT 0 AS Success, 'Check-in date cannot be in the past' AS Message;
        RETURN;
    END

    -- Validate room
    IF NOT EXISTS (SELECT 1 FROM tbl_Rooms WHERE RoomId=@RoomId AND IsDeleted=0)
    BEGIN
        SELECT 0 AS Success, 'Room not found' AS Message;
        RETURN;
    END

    DECLARE @Price DECIMAL(10,2);

    SELECT @Price = PricePerNight
    FROM tbl_Rooms
    WHERE RoomId = @RoomId;

    DECLARE @Nights INT = DATEDIFF(DAY, @CheckInDate, @CheckOutDate);

    DECLARE @Total DECIMAL(12,2) = @Price * @Nights * @RoomsBooked;

    DECLARE @BookedRooms INT;
    
    SELECT @BookedRooms = ISNULL(SUM(b.RoomsBooked),0)
    FROM tbl_Bookings b
    WHERE b.RoomId = @RoomId
    AND b.BookingStatus != 'Cancelled'
    AND (
        @CheckInDate < b.CheckOutDate
        AND @CheckOutDate > b.CheckInDate
    );
    
    DECLARE @TotalRooms INT;
    
    SELECT @TotalRooms = TotalRooms
    FROM tbl_Rooms
    WHERE RoomId = @RoomId;
    
    IF (@BookedRooms + @RoomsBooked) > @TotalRooms
    BEGIN
        SELECT 0 AS Success, 'Not enough rooms available' AS Message;
        RETURN;
    END

    INSERT INTO tbl_Bookings
    (
        UserId, GuestName, GuestEmail, GuestPhone,
        HotelId, RoomId,
        CheckInDate, CheckOutDate,
        Adults, Children,
        RoomsBooked,
        PricePerNight, TotalNights, TotalAmount,
        SpecialRequest
    )
    VALUES
    (
        @UserId, @GuestName, @GuestEmail, @GuestPhone,
        @HotelId, @RoomId,
        @CheckInDate, @CheckOutDate,
        @Adults, @Children,
        @RoomsBooked,
        @Price, @Nights, @Total,
        @SpecialRequest
    );

    SELECT 1 AS Success, 'Booking created successfully' AS Message;
END;


/* =========================
   SP: Get user booking
   ========================= */
GO
CREATE OR ALTER PROCEDURE sp_Booking_GetByUser
(
    @UserId INT
)
AS
BEGIN
    SELECT *
    FROM tbl_Bookings
    WHERE UserId = @UserId
    ORDER BY CreatedAt DESC;
END; 


/* =========================
   SP: Get guest booking
   ========================= */
GO
CREATE OR ALTER PROCEDURE sp_Booking_GetByGuest
(
    @GuestEmail NVARCHAR(150),
    @GuestPhone NVARCHAR(20)
)
AS
BEGIN
    SELECT *
    FROM tbl_Bookings
    WHERE GuestEmail = @GuestEmail
      AND GuestPhone = @GuestPhone
    ORDER BY CreatedAt DESC;
END;


/* =========================
   SP: get all booking 
   ========================= */
GO
CREATE OR ALTER PROCEDURE sp_Booking_GetAll
AS
BEGIN
    SELECT *
    FROM tbl_Bookings
    ORDER BY CreatedAt DESC;
END;


/* =========================
   SP: Update booking status
   ========================= */
GO
CREATE OR ALTER PROCEDURE sp_Booking_UpdateStatus
(
    @BookingId INT,
    @Status NVARCHAR(20)
)
AS
BEGIN
    IF @Status NOT IN ('Pending', 'Confirmed', 'Cancelled', 'Completed')
    BEGIN
        SELECT 0 AS RowsAffected;
        RETURN;
    END

    UPDATE tbl_Bookings
    SET BookingStatus = @Status
    WHERE BookingId = @BookingId
    AND BookingStatus != 'Cancelled';

    SELECT @@ROWCOUNT AS RowsAffected;
END;


/* =========================
   SP: Update payment status
   ========================= */
GO
CREATE OR ALTER PROCEDURE sp_Booking_UpdatePayment
(
    @BookingId INT,
    @Status NVARCHAR(20)
)
AS
BEGIN
    IF @Status NOT IN ('Paid', 'Refunded')
    BEGIN
        SELECT 0 AS RowsAffected;
        RETURN;
    END

    UPDATE tbl_Bookings
    SET PaymentStatus = @Status
    WHERE BookingId = @BookingId
    AND BookingStatus != 'Cancelled';

    SELECT @@ROWCOUNT AS RowsAffected;
END;


/* =========================
   SP: cancle booking
   ========================= */
GO
CREATE OR ALTER PROCEDURE sp_Booking_Cancel
(
    @BookingId INT,
    @UserId INT = NULL,
    @IsAdmin BIT = 0
)
AS
BEGIN
    SET NOCOUNT ON;

    --admin can cancle any booking
    --user can cancle own booking
    --cancle only if not Cancelled and not Paid 
    UPDATE tbl_Bookings
    SET BookingStatus = 'Cancelled'
    WHERE BookingId = @BookingId
    AND BookingStatus != 'Cancelled'
    AND PaymentStatus != 'Paid'
    AND (
        @IsAdmin = 1 OR UserId = @UserId
    );

    SELECT @@ROWCOUNT AS RowsAffected;
END;
