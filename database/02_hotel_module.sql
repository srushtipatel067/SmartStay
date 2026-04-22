/* =========================================================
   02_hotel_module.sql
   SmartStay - hotel Module
   ========================================================= */

/* =========================
   TABLE: tbl_Hotels
   ========================= */

CREATE TABLE tbl_Hotels
(
    HotelId INT IDENTITY(1,1) PRIMARY KEY,
    HotelName NVARCHAR(150) NOT NULL,
    HotelCode NVARCHAR(50) NULL,
    Description NVARCHAR(1000) NULL,

    AddressLine1 NVARCHAR(200) NOT NULL,
    AddressLine2 NVARCHAR(200) NULL,
    City NVARCHAR(100) NOT NULL,
    State NVARCHAR(100) NULL,
    Country NVARCHAR(100) NOT NULL,
    PostalCode NVARCHAR(20) NULL,

    Latitude DECIMAL(9,6) NULL,
    Longitude DECIMAL(9,6) NULL,

    StarRating DECIMAL(2,1) NULL,
    AverageRating DECIMAL(3,2) NULL,
    BasePricePerNight DECIMAL(10,2) NULL,

    ThumbnailImageUrl NVARCHAR(500) NULL,
    ContactEmail NVARCHAR(150) NULL,
    ContactPhone NVARCHAR(20) NULL,

    CheckInTime TIME NULL,
    CheckOutTime TIME NULL,

    IsActive BIT NOT NULL DEFAULT 1,
    IsDeleted BIT NOT NULL DEFAULT 0,

    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedBy INT NULL,
    UpdatedAt DATETIME NULL,
    UpdatedBy INT NULL
);


/* =========================
   SP: Create Hotel
   ========================= */
   
GO
CREATE OR ALTER PROCEDURE sp_Hotel_Create
(
    @HotelName NVARCHAR(150),
    @HotelCode NVARCHAR(50) = NULL,
    @Description NVARCHAR(1000) = NULL,

    @AddressLine1 NVARCHAR(200),
    @AddressLine2 NVARCHAR(200) = NULL,
    @City NVARCHAR(100),
    @State NVARCHAR(100) = NULL,
    @Country NVARCHAR(100),
    @PostalCode NVARCHAR(20) = NULL,

    @Latitude DECIMAL(9,6) = NULL,
    @Longitude DECIMAL(9,6) = NULL,

    @StarRating DECIMAL(2,1) = NULL,
    @BasePricePerNight DECIMAL(10,2) = NULL,

    @ThumbnailImageUrl NVARCHAR(500) = NULL,
    @ContactEmail NVARCHAR(150) = NULL,
    @ContactPhone NVARCHAR(20) = NULL,

    @CheckInTime TIME = NULL,
    @CheckOutTime TIME = NULL,

    @CreatedBy INT = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO tbl_Hotels
    (
        HotelName, HotelCode, Description,
        AddressLine1, AddressLine2, City, State, Country, PostalCode,
        Latitude, Longitude,
        StarRating, BasePricePerNight,
        ThumbnailImageUrl, ContactEmail, ContactPhone,
        CheckInTime, CheckOutTime,
        CreatedAt, CreatedBy
    )
    VALUES
    (
        @HotelName, @HotelCode, @Description,
        @AddressLine1, @AddressLine2, @City, @State, @Country, @PostalCode,
        @Latitude, @Longitude,
        @StarRating, @BasePricePerNight,
        @ThumbnailImageUrl, @ContactEmail, @ContactPhone,
        @CheckInTime, @CheckOutTime,
        GETDATE(), @CreatedBy
    );

    SELECT SCOPE_IDENTITY() AS HotelId;
END;

/* =========================
   SP: Get Hotels
   ========================= */

GO
CREATE OR ALTER PROCEDURE sp_Hotel_GetAll
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM tbl_Hotels
    WHERE IsDeleted = 0
    ORDER BY CreatedAt DESC;
END;

/* =========================
   SP: Get Hotels By HotelId
   ========================= */

GO
CREATE OR ALTER PROCEDURE sp_Hotel_GetById
(
    @HotelId INT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM tbl_Hotels
    WHERE HotelId = @HotelId
      AND IsDeleted = 0;
END;

/* =========================
   SP: Update Hotel 
   ========================= */

GO
CREATE OR ALTER PROCEDURE sp_Hotel_Update
(
    @HotelId INT,
    @HotelName NVARCHAR(150),
    @HotelCode NVARCHAR(50) = NULL,
    @Description NVARCHAR(1000) = NULL,

    @AddressLine1 NVARCHAR(200),
    @AddressLine2 NVARCHAR(200) = NULL,
    @City NVARCHAR(100),
    @State NVARCHAR(100) = NULL,
    @Country NVARCHAR(100),
    @PostalCode NVARCHAR(20) = NULL,

    @Latitude DECIMAL(9,6) = NULL,
    @Longitude DECIMAL(9,6) = NULL,

    @StarRating DECIMAL(2,1) = NULL,
    @BasePricePerNight DECIMAL(10,2) = NULL,

    @ThumbnailImageUrl NVARCHAR(500) = NULL,
    @ContactEmail NVARCHAR(150) = NULL,
    @ContactPhone NVARCHAR(20) = NULL,

    @CheckInTime TIME = NULL,
    @CheckOutTime TIME = NULL,

    @IsActive BIT,
    @UpdatedBy INT = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE tbl_Hotels
    SET
        HotelName = @HotelName,
        HotelCode = @HotelCode,
        Description = @Description,

        AddressLine1 = @AddressLine1,
        AddressLine2 = @AddressLine2,
        City = @City,
        State = @State,
        Country = @Country,
        PostalCode = @PostalCode,

        Latitude = @Latitude,
        Longitude = @Longitude,

        StarRating = @StarRating,
        BasePricePerNight = @BasePricePerNight,

        ThumbnailImageUrl = @ThumbnailImageUrl,
        ContactEmail = @ContactEmail,
        ContactPhone = @ContactPhone,

        CheckInTime = @CheckInTime,
        CheckOutTime = @CheckOutTime,

        IsActive = @IsActive,
        UpdatedAt = GETDATE(),
        UpdatedBy = @UpdatedBy
    WHERE HotelId = @HotelId
      AND IsDeleted = 0;

    SELECT @@ROWCOUNT AS RowsAffected;
END;

/* =========================
   SP:Delete Hotel
   ========================= */

GO
CREATE OR ALTER PROCEDURE sp_Hotel_Delete
(
    @HotelId INT,
    @UpdatedBy INT = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE tbl_Hotels
    SET
        IsDeleted = 1,
        UpdatedAt = GETDATE(),
        UpdatedBy = @UpdatedBy
    WHERE HotelId = @HotelId;

    SELECT @@ROWCOUNT AS RowsAffected;
END;
