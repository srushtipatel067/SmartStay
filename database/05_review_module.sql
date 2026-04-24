/* =========================================================
   05_review_module.sql
   SmartStay - review Module
   ========================================================= */

/* =========================
   TABLE: tbl_Reviews
   ========================= */
GO
CREATE TABLE tbl_Reviews
(
    ReviewId INT IDENTITY(1,1) PRIMARY KEY,

    BookingId INT NOT NULL,
    UserId INT NOT NULL,
    HotelId INT NOT NULL,

    Rating INT NOT NULL, -- 1 to 5
    Comment NVARCHAR(1000) NULL,

    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),

    FOREIGN KEY (BookingId) REFERENCES tbl_Bookings(BookingId),
    FOREIGN KEY (UserId) REFERENCES tbl_Users(UserId),
    FOREIGN KEY (HotelId) REFERENCES tbl_Hotels(HotelId)
);


/* =========================
   SP: create review
   ========================= */
GO
CREATE OR ALTER PROCEDURE sp_Review_Create
(
    @BookingId INT,
    @UserId INT,
    @Rating INT,
    @Comment NVARCHAR(1000) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    -- rating validation
    IF @Rating < 1 OR @Rating > 5
    BEGIN
        SELECT 0 AS Success, 'Invalid rating' AS Message;
        RETURN;
    END

    -- booking validation (must be completed)
    IF NOT EXISTS (
        SELECT 1 FROM tbl_Bookings
        WHERE BookingId = @BookingId
        AND UserId = @UserId
        AND BookingStatus = 'Completed'
    )
    BEGIN
        SELECT 0 AS Success, 'You can only review completed bookings' AS Message;
        RETURN;
    END

    -- prevent duplicate review
    IF EXISTS (
        SELECT 1 FROM tbl_Reviews WHERE BookingId = @BookingId
    )
    BEGIN
        SELECT 0 AS Success, 'Review already exists for this booking' AS Message;
        RETURN;
    END

    DECLARE @HotelId INT;

    SELECT @HotelId = HotelId
    FROM tbl_Bookings
    WHERE BookingId = @BookingId;

    INSERT INTO tbl_Reviews (BookingId, UserId, HotelId, Rating, Comment)
    VALUES (@BookingId, @UserId, @HotelId, @Rating, @Comment);

    -- update average rating
    UPDATE tbl_Hotels
    SET AverageRating = (
        SELECT AVG(CAST(Rating AS DECIMAL(3,2)))
        FROM tbl_Reviews
        WHERE HotelId = @HotelId
    )
    WHERE HotelId = @HotelId;

    SELECT 1 AS Success, 'Review added successfully' AS Message;
END;


/* =========================
   SP: get review by hotel
   ========================= */
GO
CREATE OR ALTER PROCEDURE sp_Review_GetByHotel
(
    @HotelId INT
)
AS
BEGIN
    SELECT 
        r.ReviewId,
        r.Rating,
        r.Comment,
        r.CreatedAt,
        u.FullName AS UserName
    FROM tbl_Reviews r
    JOIN tbl_Users u ON r.UserId = u.UserId
    WHERE r.HotelId = @HotelId
    ORDER BY r.CreatedAt DESC;
END;