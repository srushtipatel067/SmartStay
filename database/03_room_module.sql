/* =========================================================
   03_room_module.sql
   SmartStay - Room Module
   ========================================================= */

/* =========================
   TABLE: tbl_Rooms
   ========================= */

CREATE TABLE tbl_Rooms
(
    RoomId INT IDENTITY(1,1) PRIMARY KEY,
    HotelId INT NOT NULL,

    RoomType NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500) NULL,

    PricePerNight DECIMAL(10,2) NOT NULL,
    MaxAdults INT NOT NULL,
    MaxChildren INT NOT NULL,

    TotalRooms INT NOT NULL,
    AvailableRooms INT NOT NULL,

    ThumbnailImageUrl NVARCHAR(500) NULL,

    IsActive BIT NOT NULL DEFAULT 1,
    IsDeleted BIT NOT NULL DEFAULT 0,

    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedBy INT NULL,
    UpdatedAt DATETIME NULL,
    UpdatedBy INT NULL,

    CONSTRAINT FK_tbl_Rooms_tbl_Hotels
        FOREIGN KEY (HotelId) REFERENCES tbl_Hotels(HotelId)
);
GO

/* =========================
   SP: Create Room
   ========================= */

CREATE OR ALTER PROCEDURE sp_Room_Create
(
    @HotelId INT,
    @RoomType NVARCHAR(100),
    @Description NVARCHAR(500) = NULL,

    @PricePerNight DECIMAL(10,2),
    @MaxAdults INT,
    @MaxChildren INT,

    @TotalRooms INT,
    @AvailableRooms INT,

    @ThumbnailImageUrl NVARCHAR(500) = NULL,
    @CreatedBy INT = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    -- Hotel must exist and not be deleted
    IF NOT EXISTS
    (
        SELECT 1
        FROM tbl_Hotels
        WHERE HotelId = @HotelId
          AND IsDeleted = 0
    )
    BEGIN
        SELECT 0 AS RoomId, 0 AS Success, 'Hotel not found.' AS Message;
        RETURN;
    END

    -- Prevent duplicate active room type for same hotel
    IF EXISTS
    (
        SELECT 1
        FROM tbl_Rooms
        WHERE HotelId = @HotelId
          AND RoomType = @RoomType
          AND IsDeleted = 0
    )
    BEGIN
        SELECT 0 AS RoomId, 0 AS Success, 'Room type already exists for this hotel.' AS Message;
        RETURN;
    END

    INSERT INTO tbl_Rooms
    (
        HotelId,
        RoomType,
        Description,
        PricePerNight,
        MaxAdults,
        MaxChildren,
        TotalRooms,
        AvailableRooms,
        ThumbnailImageUrl,
        CreatedAt,
        CreatedBy
    )
    VALUES
    (
        @HotelId,
        @RoomType,
        @Description,
        @PricePerNight,
        @MaxAdults,
        @MaxChildren,
        @TotalRooms,
        @AvailableRooms,
        @ThumbnailImageUrl,
        GETDATE(),
        @CreatedBy
    );

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS RoomId, 1 AS Success, 'Room created successfully.' AS Message;
END;
GO

/* =========================
   SP: Get All Rooms
   ========================= */

CREATE OR ALTER PROCEDURE sp_Room_GetAll
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        r.RoomId,
        r.HotelId,
        h.HotelName,
        r.RoomType,
        r.Description,
        r.PricePerNight,
        r.MaxAdults,
        r.MaxChildren,
        r.TotalRooms,
        r.AvailableRooms,
        r.ThumbnailImageUrl,
        r.IsActive,
        r.CreatedAt,
        r.CreatedBy,
        r.UpdatedAt,
        r.UpdatedBy
    FROM tbl_Rooms r
    INNER JOIN tbl_Hotels h ON r.HotelId = h.HotelId
    WHERE r.IsDeleted = 0
      AND h.IsDeleted = 0
    ORDER BY r.CreatedAt DESC;
END;
GO

/* =========================
   SP: Get Room By Id
   ========================= */

CREATE OR ALTER PROCEDURE sp_Room_GetById
(
    @RoomId INT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        r.RoomId,
        r.HotelId,
        h.HotelName,
        r.RoomType,
        r.Description,
        r.PricePerNight,
        r.MaxAdults,
        r.MaxChildren,
        r.TotalRooms,
        r.AvailableRooms,
        r.ThumbnailImageUrl,
        r.IsActive,
        r.CreatedAt,
        r.CreatedBy,
        r.UpdatedAt,
        r.UpdatedBy
    FROM tbl_Rooms r
    INNER JOIN tbl_Hotels h ON r.HotelId = h.HotelId
    WHERE r.RoomId = @RoomId
      AND r.IsDeleted = 0
      AND h.IsDeleted = 0;
END;
GO

/* =========================
   SP: Get Rooms By HotelId
   ========================= */

CREATE OR ALTER PROCEDURE sp_Room_GetByHotelId
(
    @HotelId INT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        r.RoomId,
        r.HotelId,
        r.RoomType,
        r.Description,
        r.PricePerNight,
        r.MaxAdults,
        r.MaxChildren,
        r.TotalRooms,
        r.AvailableRooms,
        r.ThumbnailImageUrl,
        r.IsActive,
        r.CreatedAt,
        r.CreatedBy,
        r.UpdatedAt,
        r.UpdatedBy
    FROM tbl_Rooms r
    WHERE r.HotelId = @HotelId
      AND r.IsDeleted = 0
    ORDER BY r.CreatedAt DESC;
END;
GO

/* =========================
   SP: Update Room
   ========================= */

CREATE OR ALTER PROCEDURE sp_Room_Update
(
    @RoomId INT,
    @HotelId INT,
    @RoomType NVARCHAR(100),
    @Description NVARCHAR(500) = NULL,

    @PricePerNight DECIMAL(10,2),
    @MaxAdults INT,
    @MaxChildren INT,

    @TotalRooms INT,
    @AvailableRooms INT,

    @ThumbnailImageUrl NVARCHAR(500) = NULL,
    @IsActive BIT,
    @UpdatedBy INT = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    -- Room must exist
    IF NOT EXISTS
    (
        SELECT 1
        FROM tbl_Rooms
        WHERE RoomId = @RoomId
          AND IsDeleted = 0
    )
    BEGIN
        SELECT 0 AS RowsAffected, 0 AS Success, 'Room not found.' AS Message;
        RETURN;
    END

    -- Hotel must exist
    IF NOT EXISTS
    (
        SELECT 1
        FROM tbl_Hotels
        WHERE HotelId = @HotelId
          AND IsDeleted = 0
    )
    BEGIN
        SELECT 0 AS RowsAffected, 0 AS Success, 'Hotel not found.' AS Message;
        RETURN;
    END

    -- Prevent duplicate room type for same hotel except current room
    IF EXISTS
    (
        SELECT 1
        FROM tbl_Rooms
        WHERE HotelId = @HotelId
          AND RoomType = @RoomType
          AND RoomId <> @RoomId
          AND IsDeleted = 0
    )
    BEGIN
        SELECT 0 AS RowsAffected, 0 AS Success, 'Room type already exists for this hotel.' AS Message;
        RETURN;
    END

    UPDATE tbl_Rooms
    SET
        HotelId = @HotelId,
        RoomType = @RoomType,
        Description = @Description,
        PricePerNight = @PricePerNight,
        MaxAdults = @MaxAdults,
        MaxChildren = @MaxChildren,
        TotalRooms = @TotalRooms,
        AvailableRooms = @AvailableRooms,
        ThumbnailImageUrl = @ThumbnailImageUrl,
        IsActive = @IsActive,
        UpdatedAt = GETDATE(),
        UpdatedBy = @UpdatedBy
    WHERE RoomId = @RoomId
      AND IsDeleted = 0;

    SELECT @@ROWCOUNT AS RowsAffected, 1 AS Success, 'Room updated successfully.' AS Message;
END;
GO

/* =========================
   SP: Delete Room (Soft Delete)
   ========================= */

CREATE OR ALTER PROCEDURE sp_Room_Delete
(
    @RoomId INT,
    @UpdatedBy INT = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS
    (
        SELECT 1
        FROM tbl_Rooms
        WHERE RoomId = @RoomId
          AND IsDeleted = 0
    )
    BEGIN
        SELECT 0 AS RowsAffected, 0 AS Success, 'Room not found.' AS Message;
        RETURN;
    END

    UPDATE tbl_Rooms
    SET
        IsDeleted = 1,
        UpdatedAt = GETDATE(),
        UpdatedBy = @UpdatedBy
    WHERE RoomId = @RoomId
      AND IsDeleted = 0;

    SELECT @@ROWCOUNT AS RowsAffected, 1 AS Success, 'Room deleted successfully.' AS Message;
END;
GO

/* =========================
   SP: Get Active Rooms By HotelId
   ========================= */

CREATE OR ALTER PROCEDURE sp_Room_GetActiveByHotelId
(
    @HotelId INT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        RoomId,
        HotelId,
        RoomType,
        Description,
        PricePerNight,
        MaxAdults,
        MaxChildren,
        TotalRooms,
        AvailableRooms,
        ThumbnailImageUrl,
        IsActive
    FROM tbl_Rooms
    WHERE HotelId = @HotelId
      AND IsDeleted = 0
      AND IsActive = 1
    ORDER BY CreatedAt DESC;
END;
GO