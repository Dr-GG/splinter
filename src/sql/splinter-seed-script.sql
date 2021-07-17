IF (NOT EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = 'masterdata'))
BEGIN
	EXEC('CREATE SCHEMA masterdata');
END

IF (NOT EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = 'platform'))
BEGIN
	EXEC('CREATE SCHEMA platform');
END

IF (NOT EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = 'tera'))
BEGIN
	EXEC('CREATE SCHEMA tera');
END

IF (NOT EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = 'nano'))
BEGIN
	EXEC('CREATE SCHEMA nano');
END

IF (NOT EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = 'superposition'))
BEGIN
	EXEC('CREATE SCHEMA superposition');
END

GO

IF (OBJECT_ID('masterdata.TeraAgentStatuses') IS NULL)
BEGIN

	CREATE TABLE masterdata.TeraAgentStatuses
	(
		Id INT NOT NULL
			CONSTRAINT PK_masterdataTeraAgentStatuses
			PRIMARY KEY (Id),
		Name NVARCHAR(100) NOT NULL
			CONSTRAINT UQ_masterdataTeraAgentStatuses_Name
			UNIQUE (Name)
	);

END

IF (OBJECT_ID('masterdata.TeraPlatformStatuses') IS NULL)
BEGIN

	CREATE TABLE masterdata.TeraPlatformStatuses
	(
		Id INT NOT NULL
			CONSTRAINT PK_masterdataTeraPlatformStatuses
			PRIMARY KEY (Id),
		Name NVARCHAR(100) NOT NULL
			CONSTRAINT UQ_masterdataTeraPlatformStatuses_Name
			UNIQUE (Name)
	);

END

IF (OBJECT_ID('masterdata.OperatingSystems') IS NULL)
BEGIN

	CREATE TABLE masterdata.OperatingSystems
	(
		Id INT NOT NULL
			CONSTRAINT PK_masterdataOperatingSystems
			PRIMARY KEY (Id),
		Name NVARCHAR(100) NOT NULL
			CONSTRAINT UQ_masterdataOperatingSystems_Name
			UNIQUE (Name)
	);

END

IF (OBJECT_ID('masterdata.ProcessorArchitectures') IS NULL)
BEGIN

	CREATE TABLE masterdata.ProcessorArchitectures
	(
		Id INT NOT NULL
			CONSTRAINT PK_masterdataProcessorArchitectures
			PRIMARY KEY (Id),
		Name NVARCHAR(100) NOT NULL
			CONSTRAINT UQ_masterdataProcessorArchitectures_Name
			UNIQUE (Name)
	);

END

IF (OBJECT_ID('masterdata.TeraMessageStatuses') IS NULL)
BEGIN

	CREATE TABLE masterdata.TeraMessageStatuses
	(
		Id INT NOT NULL
			CONSTRAINT PK_masterdataTeraMessageStatuses
			PRIMARY KEY (Id),
		Name NVARCHAR(100) NOT NULL
			CONSTRAINT UQ_masterdataTeraMessageStatuses_Name
			UNIQUE (Name)
	);

END

IF (OBJECT_ID('masterdata.TeraMessageErrorCodes') IS NULL)
BEGIN

	CREATE TABLE masterdata.TeraMessageErrorCodes
	(
		Id INT NOT NULL
			CONSTRAINT PK_masterdataTeraMessageErrorCodes
			PRIMARY KEY (Id),
		Name NVARCHAR(100) NOT NULL
			CONSTRAINT UQ_masterdataTeraMessageErrorCodes_Name
			UNIQUE (Name)
	);

END

IF (OBJECT_ID('platform.OperatingSystems') IS NULL)
BEGIN

	CREATE TABLE [platform].OperatingSystems
	(
		Id BIGINT IDENTITY (1, 1)
			CONSTRAINT PK_platformOperatingSystems
			PRIMARY KEY (Id),
		Type INT NOT NULL
			CONSTRAINT FK_platformOperatingSystems_masterdataOperatingSystems
			FOREIGN KEY (Type) REFERENCES masterdata.OperatingSystems (Id),
		ProcessorArchitecture INT NOT NULL
			CONSTRAINT FK_platformOperatingSystems_masterdataProcessorArchitectures
			FOREIGN KEY (ProcessorArchitecture) REFERENCES masterdata.ProcessorArchitectures (Id),
		Description NVARCHAR(500) NOT NULL
			CONSTRAINT UQ_platformOperatingSystems_Type_ProcessorArchitecture_Description
			UNIQUE (Type, ProcessorArchitecture, Description)
	);

END

IF (OBJECT_ID('platform.TeraPlatforms') IS NULL)
BEGIN

	CREATE TABLE platform.TeraPlatforms
	(
		Id BIGINT IDENTITY (1, 1) NOT NULL
			CONSTRAINT PK_platformTeraPlatforms
			PRIMARY KEY (Id),
		TeraId UNIQUEIDENTIFIER NOT NULL
			CONSTRAINT UQ_platformTeraPlatforms_TeraId
			UNIQUE (TeraId),
		Status INT NOT NULL
			CONSTRAINT FK_platformTeraPlatforms_masterdataTeraPlatformStatuses
			FOREIGN KEY (Status) REFERENCES masterdata.TeraPlatformStatuses (Id),
		OperatingSystemId BIGINT NOT NULL
			CONSTRAINT FK_platformTeraPlatforms_platformOperatingSystems
			FOREIGN KEY (OperatingSystemId) REFERENCES platform.OperatingSystems (Id),
		FrameworkDescription VARCHAR(200) NOT NULL,
	);

END

IF (OBJECT_ID('nano.NanoTypes') IS NULL)
BEGIN

	CREATE TABLE nano.NanoTypes
	(
		Id BIGINT IDENTITY (1, 1) NOT NULL
			CONSTRAINT PK_nanoNanoTypes
			PRIMARY KEY (Id),
		Name VARCHAR(200) NOT NULL,
		Guid UNIQUEIDENTIFIER NOT NULL,
		Version VARCHAR(100) NOT NULL
			CONSTRAINT UQ_nanoNanoTypes_Guid_Version
			UNIQUE (Guid, Version),
	);

END

IF (OBJECT_ID('nano.NanoInstances') IS NULL)
BEGIN

	CREATE TABLE nano.NanoInstances
	(
		Id BIGINT IDENTITY (1, 1) NOT NULL
			CONSTRAINT PK_nanoNanoInstances
			PRIMARY KEY (Id),
		NanoTypeId BIGINT NOT NULL
			CONSTRAINT FK_nanoNanoInstances_nanoNanoTypes
			FOREIGN KEY (NanoTypeId) REFERENCES nano.NanoTypes (Id),
		Name VARCHAR(200) NOT NULL,
		Guid UNIQUEIDENTIFIER NOT NULL,
		Version VARCHAR(100) NOT NULL
			CONSTRAINT UQ_nanoNanoInstances_Guid_Version
			UNIQUE (Guid, Version),
	)

END

IF (OBJECT_ID('tera.TeraAgents') IS NULL)
BEGIN

	CREATE TABLE tera.TeraAgents
	(
		Id BIGINT IDENTITY (1, 1)
			CONSTRAINT PK_teraTeraAgents
			PRIMARY KEY (Id),
		TeraId UNIQUEIDENTIFIER NOT NULL
			CONSTRAINT UQ_teraTeraAgents_TeraId
			UNIQUE (TeraId),
		NanoInstanceId BIGINT NOT NULL
			CONSTRAINT FK_teraTeraAgents_nanoNanoInstances
			FOREIGN KEY (NanoInstanceId) REFERENCES nano.NanoInstances (Id),
		Status INT NOT NULL
			CONSTRAINT FK_teraTeraAgents_masterdataTeraAgentStatuses
			FOREIGN KEY (Status) REFERENCES masterdata.TeraAgentStatuses (Id),
		TeraPlatformId BIGINT NOT NULL
			CONSTRAINT FK_teraTeraAgents_platformTeraPlatforms
			FOREIGN KEY (TeraPlatformId) REFERENCES platform.TeraPlatforms (Id)
	);

END

IF (OBJECT_ID('tera.TeraMessages') IS NULL)
BEGIN

	CREATE TABLE tera.TeraMessages
	(
		Id BIGINT IDENTITY (1, 1) NOT NULL
			CONSTRAINT PK_teraTeraMessages
			PRIMARY KEY (Id),
		ETag ROWVERSION NOT NULL,
		SourceTeraAgentId BIGINT NOT NULL
			CONSTRAINT FK_teraMessages_teraAgents_SourceTeraAgentId
			FOREIGN KEY (SourceTeraAgentId) REFERENCES tera.TeraAgents (Id),
		RecipientTeraAgentId BIGINT NOT NULL
			CONSTRAINT FK_teraMessages_teraAgents_RecipientTeraAgentId
			FOREIGN KEY (RecipientTeraAgentId) REFERENCES tera.TeraAgents (Id),
		BatchId UNIQUEIDENTIFIER NOT NULL
			CONSTRAINT UQ_teraTeraMessages_RecipientTeraAgentId_BatchId
			UNIQUE (RecipientTeraAgentId, BatchId),
		Priority INT NOT NULL,
		DequeueCount INT NOT NULL,
		Code INT NOT NULL,
		Status INT NOT NULL,
			CONSTRAINT FK_teraMessages_masterdataTeraMessageStatuses
			FOREIGN KEY (Status) REFERENCES masterdata.TeraMessageStatuses (Id),
		ErrorCode INT NULL
			CONSTRAINT FK_teraMessages_masterdataTeraErrorCodes
			FOREIGN KEY (ErrorCode) REFERENCES masterdata.TeraMessageErrorCodes (Id),
		Message NVARCHAR(MAX) NULL,
		AbsoluteExpiryTimestamp DATETIME2 NOT NULL,
		LoggedTimestamp DATETIME2 NOT NULL,
		DequeuedTimestamp DATETIME2 NULL,
		CompletedTimestamp DATETIME2 NULL,
		ErrorMessage NVARCHAR(MAX) NULL,
		ErrorStackTrace NVARCHAR(MAX) NULL
	);

END

IF (OBJECT_ID('tera.PendingTeraMessages') IS NULL)
BEGIN

	CREATE TABLE tera.PendingTeraMessages
	(
		Id BIGINT IDENTITY (1, 1) NOT NULL
			CONSTRAINT PK_teraPendingTeraMessages
			PRIMARY KEY (Id),
		TeraAgentId BIGINT NOT NULL
			CONSTRAINT FK_teraPendingTeraMessages_teraAgents
			FOREIGN KEY (TeraAgentId) REFERENCES tera.TeraAgents(Id),
		TeraMessageId BIGINT NOT NULL
			CONSTRAINT FK_teraPendingTeraMessages_teraTeraMessages
			FOREIGN KEY (TeraMessageId) REFERENCES tera.TeraMessages (Id),

		CONSTRAINT UQ_teraPendingTeraMessages_TeraMessageId
		UNIQUE (TeraMessageId)
	);

END

IF (OBJECT_ID('superposition.TeraAgentNanoTypeDependencies') IS NULL)
BEGIN

	CREATE TABLE superposition.TeraAgentNanoTypeDependencies
	(
		Id BIGINT IDENTITY (1, 1) NOT NULL
			CONSTRAINT PK_superpositionTeraAgentNanoTypeDependencies
			PRIMARY KEY (Id),
		TeraAgentId BIGINT NOT NULL
			CONSTRAINT FK_superpositionTeraAgentNanoTypeDependencies_teraTeraAgents
			FOREIGN KEY (TeraAgentId) REFERENCES tera.TeraAgents (Id),
		NanoTypeId BIGINT NOT NULL
			CONSTRAINT FK_superpositionTeraAgentNanoTypeDependencies_nanoNanoTypes
			FOREIGN KEY (NanoTypeId) REFERENCES nano.NanoTypes (Id),
		NumberOfDependencies INT NOT NULL,
	
		CONSTRAINT UQ_superpositionTeraAgentNanoTypeDependencies_TeraAgentId_NanoTypeId
		UNIQUE (TeraAgentId, NanoTypeId)
	);

END

IF (OBJECT_ID('superposition.NanoTypeRecollapseOperations') IS NULL)
BEGIN

	CREATE TABLE superposition.NanoTypeRecollapseOperations
	(
		Id BIGINT IDENTITY (1, 1) NOT NULL
			CONSTRAINT PK_superpositionNanoTypeRecollapseOperations
			PRIMARY KEY (Id),
		Guid UNIQUEIDENTIFIER NOT NULL
			CONSTRAINT UQ_superpositionNanoTypeRecollapseOperations_Guid
			UNIQUE (Guid),
		NanoTypeId BIGINT NOT NULL
			CONSTRAINT FK_superpositionNanoTypeRecollapseOperations_nanoNanoTypes
			FOREIGN KEY (NanoTypeId) REFERENCES nano.NanoTypes (Id),
		CreatedTimestamp DATETIME2 NOT NULL,
		NumberOfExpectedRecollapses BIGINT NOT NULL,
		NumberOfSuccessfulRecollapses BIGINT NOT NULL,
		NumberOfFailedRecollapses BIGINT NOT NULL
	);

END

IF (NOT EXISTS(SELECT 1 FROM sys.indexes WHERE Name = 'IX_teraPendingTeramessages_TeraId'))
BEGIN
	CREATE NONCLUSTERED INDEX IX_teraPendingTeraMessages_TeraId ON tera.PendingTeraMessages (TeraAgentId) INCLUDE (TeraMessageId);
END

GO

IF ((SELECT COUNT(1) FROM masterdata.ProcessorArchitectures) = 0)
BEGIN

	INSERT INTO masterdata.ProcessorArchitectures (Id, Name) VALUES
	(1, 'Unknown'),
	(2, 'Arm'),
	(3, 'Arm64'),
	(4, 'x86'),
	(5, 'x64'),
	(6, 'Wasm');

END

IF ((SELECT COUNT(1) FROM masterdata.OperatingSystems) = 0)
BEGIN

	INSERT INTO masterdata.OperatingSystems (Id, Name) VALUES
	(1, 'Unknown'),
	(2, 'Free BSD'),
	(3, 'Linux'),
	(4, 'OSX'),
	(5, 'Windows');

END

IF ((SELECT COUNT(1) FROM masterdata.TeraPlatformStatuses) = 0)
BEGIN

	INSERT INTO masterdata.TeraPlatformStatuses (Id, Name) VALUES
	(1, 'Halted'),
	(2, 'Running');

END

IF ((SELECT COUNT(1) FROM masterdata.TeraAgentStatuses) = 0)
BEGIN

	INSERT INTO masterdata.TeraAgentStatuses (Id, Name) VALUES
	(1, 'Running'),
	(2, 'Migrating'),
	(3, 'Disposed');

END


IF ((SELECT COUNT(1) FROM masterdata.TeraMessageStatuses) = 0)
BEGIN

	INSERT INTO masterdata.TeraMessageStatuses (Id, Name) VALUES
	(1, 'Pending'),
	(2, 'Dequeued'),
	(3, 'Cancelled'),
	(4, 'Completed'),
	(5, 'Failed');

END

IF ((SELECT COUNT(1) FROM masterdata.TeraMessageErrorCodes) = 0)
BEGIN

	INSERT INTO masterdata.TeraMessageErrorCodes (Id, Name) VALUES
	(1, 'Unknown'),
	(2, 'Maximum Dequeue Count Reached'),
	(3, 'Disposed');

END

