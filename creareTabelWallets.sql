CREATE TABLE Wallets (
    WalletID INT IDENTITY(1,1) PRIMARY KEY, -- Cheie primară
    UserID INT NOT NULL,                    -- Referință către utilizator
    Balance DECIMAL(18, 2) NOT NULL DEFAULT 0.00, -- Sold inițial (implicit 0)
    LastUpdated DATETIME NOT NULL DEFAULT GETDATE(), -- Data ultimei actualizări
    CONSTRAINT FK_Wallet_User FOREIGN KEY (UserID) REFERENCES Users(UserID)
);
