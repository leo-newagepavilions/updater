CREATE TABLE [dbo].[Table]
(
	[PatchScriptId] INT NOT NULL PRIMARY KEY, 
    [MarketId] INT NOT NULL, 
    [MarketSN] NVARCHAR(50) NOT NULL, 
    [PatchSeverity] NVARCHAR(50) NULL, 
    [DownloadDate] DATETIME NULL, 
    [PatchState] NVARCHAR(50) NULL
)
