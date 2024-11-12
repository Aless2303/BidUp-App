CREATE TABLE Logs (
    LogID INT IDENTITY(1,1) PRIMARY KEY,
    Timestamp DATETIME DEFAULT GETDATE(),
    EventType NVARCHAR(100),    -- Tipul evenimentului (ex: 'Bid', 'AuctionStart')
    Message NVARCHAR(MAX),      -- Mesaj descriptiv
    DynamicData NVARCHAR(MAX)   -- Câmp JSON pentru date adiționale
);