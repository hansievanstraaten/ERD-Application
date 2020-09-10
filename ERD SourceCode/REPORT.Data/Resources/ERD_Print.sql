SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

IF NOT EXISTS (SELECT 1
                 FROM INFORMATION_SCHEMA.TABLES
                WHERE TABLE_NAME = 'ReportMaster')
BEGIN
    CREATE TABLE [ReportMaster]
         ( 
           [MasterReport_Id] [BigInt]  IDENTITY(1,1) NOT NULL,
           [ReportName] [VarChar] (255) NOT NULL,
           [Description] [VarBinary] (MAX) NULL,
           [ReportTypeEnum] [Int]  NOT NULL,
    CONSTRAINT [PK_ReportMaster] PRIMARY KEY CLUSTERED
    (
        [MasterReport_Id] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY]
END

IF NOT EXISTS (SELECT 1 
                 FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_NAME  = 'ReportMaster' 
                  AND COLUMN_NAME = 'MasterReport_Id')
BEGIN
    ALTER TABLE [ReportMaster]
    ADD [MasterReport_Id] [BigInt]  IDENTITY(1,1) NOT NULL
END


IF NOT EXISTS (SELECT 1 
                 FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_NAME  = 'ReportMaster' 
                  AND COLUMN_NAME = 'ReportName')
BEGIN
    ALTER TABLE [ReportMaster]
    ADD [ReportName] [VarChar] (255) NOT NULL
END


IF NOT EXISTS (SELECT 1 
                 FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_NAME  = 'ReportMaster' 
                  AND COLUMN_NAME = 'Description')
BEGIN
    ALTER TABLE [ReportMaster]
    ADD [Description] [VarBinary] (MAX) NULL
END


IF NOT EXISTS (SELECT 1 
                 FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_NAME  = 'ReportMaster' 
                  AND COLUMN_NAME = 'ReportTypeEnum')
BEGIN
    ALTER TABLE [ReportMaster]
    ADD [ReportTypeEnum] [Int]  NOT NULL
END



IF NOT EXISTS(SELECT 1
                FROM SYS.INDEXES
               WHERE IS_PRIMARY_KEY = 1
               AND[NAME] = 'PK_ReportMaster')
BEGIN
    ALTER TABLE[ReportMaster]
    ADD CONSTRAINT PK_ReportMaster PRIMARY KEY CLUSTERED([MasterReport_Id]);
END


SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

IF NOT EXISTS (SELECT 1
                 FROM INFORMATION_SCHEMA.TABLES
                WHERE TABLE_NAME = 'ReportXML')
BEGIN
    CREATE TABLE [ReportXML]
         ( 
           [ReportXMLVersion] [Int]  NOT NULL,
           [MasterReport_Id] [BigInt]  NOT NULL,
           [BinaryXML] [VarBinary] (MAX) NOT NULL,
           [PrintCount] [BigInt]  NOT NULL,
    CONSTRAINT [PK_ReportXML] PRIMARY KEY CLUSTERED
    (
        [ReportXMLVersion] ASC,
        [MasterReport_Id] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY]
END

IF NOT EXISTS (SELECT 1 
                 FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_NAME  = 'ReportXML' 
                  AND COLUMN_NAME = 'ReportXMLVersion')
BEGIN
    ALTER TABLE [ReportXML]
    ADD [ReportXMLVersion] [Int]  NOT NULL
END


IF NOT EXISTS (SELECT 1 
                 FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_NAME  = 'ReportXML' 
                  AND COLUMN_NAME = 'MasterReport_Id')
BEGIN
    ALTER TABLE [ReportXML]
    ADD [MasterReport_Id] [BigInt]  NOT NULL
END


IF NOT EXISTS (SELECT 1 
                 FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_NAME  = 'ReportXML' 
                  AND COLUMN_NAME = 'BinaryXML')
BEGIN
    ALTER TABLE [ReportXML]
    ADD [BinaryXML] [VarBinary] (MAX) NOT NULL
END


IF NOT EXISTS (SELECT 1 
                 FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_NAME  = 'ReportXML' 
                  AND COLUMN_NAME = 'PrintCount')
BEGIN
    ALTER TABLE [ReportXML]
    ADD [PrintCount] [BigInt]  NOT NULL
END



IF NOT EXISTS(SELECT 1
                FROM SYS.INDEXES
               WHERE IS_PRIMARY_KEY = 1
               AND[NAME] = 'PK_ReportXML')
BEGIN
    ALTER TABLE[ReportXML]
    ADD CONSTRAINT PK_ReportXML PRIMARY KEY CLUSTERED([ReportXMLVersion],[MasterReport_Id]);
END


SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

IF NOT EXISTS (SELECT 1
                 FROM INFORMATION_SCHEMA.TABLES
                WHERE TABLE_NAME = 'Lookup')
BEGIN
    CREATE TABLE [Lookup]
         ( 
           [LookupGroup] [VarChar] (255) NOT NULL,
           [GroupKey] [Int]  NOT NULL,
           [GroupDescription] [VarChar] (255) NOT NULL,
    CONSTRAINT [PK_Lookup] PRIMARY KEY CLUSTERED
    (
        [LookupGroup] ASC,
        [GroupKey] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY]
END

IF NOT EXISTS (SELECT 1 
                 FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_NAME  = 'Lookup' 
                  AND COLUMN_NAME = 'LookupGroup')
BEGIN
    ALTER TABLE [Lookup]
    ADD [LookupGroup] [VarChar] (255) NOT NULL
END


IF NOT EXISTS (SELECT 1 
                 FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_NAME  = 'Lookup' 
                  AND COLUMN_NAME = 'GroupKey')
BEGIN
    ALTER TABLE [Lookup]
    ADD [GroupKey] [Int]  NOT NULL
END


IF NOT EXISTS (SELECT 1 
                 FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_NAME  = 'Lookup' 
                  AND COLUMN_NAME = 'GroupDescription')
BEGIN
    ALTER TABLE [Lookup]
    ADD [GroupDescription] [VarChar] (255) NOT NULL
END



IF NOT EXISTS(SELECT 1
                FROM SYS.INDEXES
               WHERE IS_PRIMARY_KEY = 1
               AND[NAME] = 'PK_Lookup')
BEGIN
    ALTER TABLE[Lookup]
    ADD CONSTRAINT PK_Lookup PRIMARY KEY CLUSTERED([LookupGroup],[GroupKey]);
END


