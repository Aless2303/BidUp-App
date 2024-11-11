CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),       -- Auto-incrementing primary key
    FullName NVARCHAR(100) NOT NULL,            -- User's full name
    Email NVARCHAR(255) NOT NULL UNIQUE,        -- User's email, unique
    PasswordHash NVARCHAR(255) NOT NULL,        -- Password hash for secure storage
     Role NVARCHAR(20) CHECK (Role IN ('Bidder', 'Admin', 'Seller')),                 -- Role of the user (e.g., Bidder, Admin, Seller)
    BirthDate DATE NOT NULL,                    -- User's date of birth
    ProfilePicturePath NVARCHAR(500) NULL,      -- Path to profile picture, nullable in case not provided
    CreatedAt DATETIME DEFAULT GETDATE()        -- Date and time the user was created, with default value of current date
);

DROP TABLE IF EXISTS Cards;

CREATE TABLE Cards (
    CardID INT PRIMARY KEY IDENTITY(1,1),         -- Unique identifier for each card
    CardNumber NVARCHAR(20) NOT NULL,             -- Card number (consider masking)
    CardHolderName NVARCHAR(100) NOT NULL,        -- Name of the cardholder
    ExpiryDate DATE NOT NULL,                     -- Expiry date of the card
    CVV NVARCHAR(5) NOT NULL,                     -- CVV code (for demonstration purposes)
    OwnerUserID INT,                              -- Foreign key linking to Users table
    Balance DECIMAL,
    FOREIGN KEY (OwnerUserID) REFERENCES Users(UserID) ON DELETE CASCADE
);
