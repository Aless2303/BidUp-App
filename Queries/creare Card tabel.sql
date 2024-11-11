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

