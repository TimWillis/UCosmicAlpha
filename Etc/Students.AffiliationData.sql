CREATE TABLE [dbo].[Table]
(
	[id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [studentId] INT NOT NULL, 
    [localEstablishmentId] INT NOT NULL
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Student Term Data',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Table',
    @level2type = NULL,
    @level2name = NULL