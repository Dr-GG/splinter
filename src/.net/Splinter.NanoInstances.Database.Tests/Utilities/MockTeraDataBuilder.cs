using System;
using AutoFixture;
using Splinter.NanoInstances.Database.DbContext;
using Splinter.NanoInstances.Database.DbContext.Models;
using Splinter.NanoTypes.Domain.Enums;
using Tenjin.Extensions;
using Tenjin.Tests.AutoFixture.EntityFramework.Data.Builders;
using OperatingSystem = Splinter.NanoTypes.Domain.Enums.OperatingSystem;

namespace Splinter.NanoInstances.Database.Tests.Utilities;

public class MockTeraDataBuilder : EntityFrameworkAutoFixtureDataBuilder<TeraDbContext>
{
    public MockTeraDataBuilder(TeraDbContext teraDbContext) : base(teraDbContext)
    { }

    public OperatingSystemModel AddOperatingSystem(
        long id = 0,
        string? description = null,
        ProcessorArchitecture? architecture = null,
        OperatingSystem? type = null)
    {
        var query = Fixture.Build<OperatingSystemModel>()
            .With(m => m.Id, id);

        if (description.IsNotNullOrEmpty())
        {
            query = query.With(m => m.Description, description);
        }

        if (architecture.HasValue)
        {
            query = query.With(m => m.ProcessorArchitecture, architecture);
        }

        if (type.HasValue)
        {
            query = query.With(m => m.Type, type);
        }

        var operatingSystem = query.Create();

        DbContext.OperatingSystems.Add(operatingSystem);
        DbContext.SaveChanges();

        return operatingSystem;
    }

    public TeraPlatformModel AddTeraPlatform(
        long operatingSystemId,
        long id,
        Guid? teraId = null,
        TeraPlatformStatus status = TeraPlatformStatus.Running,
        string? frameworkDescription = null)
    {
        var query = Fixture.Build<TeraPlatformModel>()
            .With(m => m.Id, id)
            .With(m => m.OperatingSystemId, operatingSystemId)
            .With(m => m.TeraId, teraId ?? Guid.NewGuid())
            .With(m => m.Status, status);

        if (frameworkDescription.IsNotNullOrEmpty())
        {
            query = query.With(m => m.FrameworkDescription, frameworkDescription);
        }

        var platform = query.Create();

        DbContext.TeraPlatforms.Add(platform);
        DbContext.SaveChanges();

        return platform;
    }

    public TeraPlatformModel AddTeraPlatform(long id = 0)
    {
        var operatingSystemId = AddOperatingSystem().Id;

        return AddTeraPlatform(operatingSystemId, id);
    }

    public NanoTypeModel AddNanoType(
        long id = 0,
        Guid? guid = null,
        string? name = null,
        string? version = null)
    {
        var query = Fixture.Build<NanoTypeModel>()
            .With(m => m.Id, id)
            .With(m => m.Guid, guid ?? Guid.NewGuid());

        if (name.IsNotNullOrEmpty())
        {
            query = query.With(m => m.Name, name);
        }

        if (version.IsNotNullOrEmpty())
        {
            query = query.With(m => m.Version, version);
        }

        var nanoType = query.Create();

        DbContext.NanoTypes.Add(nanoType);
        DbContext.SaveChanges();

        return nanoType;
    }

    public NanoInstanceModel AddNanoInstance(
        long nanoTypeId,
        long id,
        Guid? guid = null,
        string? name = null,
        string? version = null)
    {
        var query = Fixture.Build<NanoInstanceModel>()
            .With(m => m.Id, id)
            .With(m => m.NanoTypeId, nanoTypeId)
            .With(m => m.Guid, guid ?? Guid.NewGuid());

        if (name.IsNotNullOrEmpty())
        {
            query = query.With(m => m.Name, name);
        }

        if (version.IsNotNullOrEmpty())
        {
            query = query.With(m => m.Version, version);
        }

        var nanoInstance = query.Create();

        DbContext.NanoInstances.Add(nanoInstance);
        DbContext.SaveChanges();

        return nanoInstance;
    }

    public NanoInstanceModel AddNanoInstance(long id = 0)
    {
        var nanoTypeId = AddNanoType().Id;

        return AddNanoInstance(nanoTypeId, id);
    }

    public TeraAgentModel AddTeraAgent(
        long platformId,
        long nanoInstanceId,
        long id = 0,
        Guid? teraId = null,
        TeraAgentStatus status = TeraAgentStatus.Running)
    {
        var teraAgent = Fixture.Build<TeraAgentModel>()
            .With(m => m.Id, id)
            .With(m => m.TeraPlatformId, platformId)
            .With(m => m.NanoInstanceId, nanoInstanceId)
            .With(m => m.TeraId, teraId ?? Guid.NewGuid())
            .With(m => m.Status, status)
            .Create();

        DbContext.TeraAgents.Add(teraAgent);
        DbContext.SaveChanges();

        return teraAgent;
    }

    public TeraAgentModel AddTeraAgent(long id)
    {
        var teraId = Guid.NewGuid();

        return AddTeraAgent(teraId, id);
    }

    public TeraAgentModel AddTeraAgent(Guid teraId, long id = 0)
    {
        var platformId = AddTeraPlatform().Id;
        var nanoInstanceId = AddNanoInstance().Id;

        return AddTeraAgent(platformId, nanoInstanceId, teraId: teraId, id: id);
    }

    public TeraMessageModel AddTeraMessage(
        long sourceTeraAgentId,
        long recipientTeraAgentId,
        long id = 0,
        int? priority = null,
        int code = 0,
        string? message = null,
        TeraMessageStatus? status = TeraMessageStatus.Pending,
        DateTime? loggedTimestamp = null,
        DateTime? completedTimestamp = null,
        DateTime? absoluteExpiryTimestamp = null,
        int dequeueCount = 0,
        TeraMessageErrorCode? errorCode = null,
        string? errorMessage = null,
        string? errorStackTrace = null,
        bool addPending = true)
    {
        var query = Fixture.Build<TeraMessageModel>()
            .With(m => m.Id, id)
            .With(m => m.SourceTeraAgentId, sourceTeraAgentId)
            .With(m => m.RecipientTeraAgentId, recipientTeraAgentId)
            .With(m => m.Message, message ?? GetHashCode().ToString())
            .With(m => m.DequeueCount, dequeueCount)
            .With(m => m.Code, code)
            .Without(m => m.ErrorCode)
            .Without(m => m.ErrorMessage)
            .Without(m => m.ErrorStackTrace);

        if (priority.HasValue)
        {
            query = query.With(m => m.Priority, priority);
        }

        if (status.HasValue)
        {
            query = query.With(m => m.Status, status);
        }

        if (loggedTimestamp.HasValue)
        {
            query = query.With(m => m.LoggedTimestamp, loggedTimestamp);
        }

        if (completedTimestamp.HasValue)
        {
            query = query.With(m => m.CompletedTimestamp, completedTimestamp);
        }

        if (absoluteExpiryTimestamp.HasValue)
        {
            query = query.With(m => m.AbsoluteExpiryTimestamp, absoluteExpiryTimestamp);
        }

        if (errorCode.HasValue)
        {
            query = query.With(m => m.ErrorCode, errorCode);
        }

        if (errorMessage.IsNotNullOrEmpty())
        {
            query = query.With(m => m.ErrorMessage, errorMessage);
        }

        if (errorStackTrace.IsNotNullOrEmpty())
        {
            query = query.With(m => m.ErrorStackTrace, errorStackTrace);
        }

        var dbMessage = query.Create();

        DbContext.TeraMessages.Add(dbMessage);
        DbContext.SaveChanges();

        if (addPending)
        {
            dbMessage.Pending = AddPendingTeraMessage(
                dbMessage.RecipientTeraAgentId,
                dbMessage.Id);
        }

        return dbMessage;
    }

    public TeraAgentNanoTypeDependencyModel AddTeraAgentNanoTypeDependency(
        long teraAgentId,
        long nanoTypeId,
        long? id = 0,
        int? numberOfDependencies = null)
    {
        var query = Fixture.Build<TeraAgentNanoTypeDependencyModel>()
            .With(m => m.Id, id)
            .With(m => m.TeraAgentId, teraAgentId)
            .With(m => m.NanoTypeId, nanoTypeId);

        if (numberOfDependencies.HasValue)
        {
            query = query.With(m => m.NumberOfDependencies, numberOfDependencies.Value);
        }

        var model = query.Create();

        DbContext.TeraAgentNanoTypeDependencies.Add(model);
        DbContext.SaveChanges();

        return model;
    }

    public PendingTeraMessageModel AddPendingTeraMessage(
        long teraAgentId,
        long messageId,
        long id = 0)
    {
        var pending = new PendingTeraMessageModel
        {
            TeraAgentId = teraAgentId,
            TeraMessageId = messageId,
            Id = id
        };

        DbContext.PendingTeraMessages.Add(pending);
        DbContext.SaveChanges();

        return pending;
    }

    public NanoTypeRecollapseOperationModel AddNanoTypeRecollapseOperation(
        long nanoTypeId,
        long id = 0,
        Guid? guid = null,
        long? numberOfExpectedRecollapses = null,
        long? numberOfSuccessfulRecollapses = null,
        long? numberOfFailedRecollapses = null)
    {
        var query = Fixture.Build<NanoTypeRecollapseOperationModel>()
            .With(m => m.Id, id)
            .With(m => m.NanoTypeId, nanoTypeId);

        if (guid.HasValue)
        {
            query = query.With(m => m.Guid, guid.Value);
        }

        if (numberOfExpectedRecollapses.HasValue)
        {
            query = query.With(m => m.NumberOfExpectedRecollapses, numberOfExpectedRecollapses.Value);
        }

        if (numberOfSuccessfulRecollapses.HasValue)
        {
            query = query.With(m => m.NumberOfSuccessfulRecollapses, numberOfSuccessfulRecollapses.Value);
        }

        if (numberOfFailedRecollapses.HasValue)
        {
            query = query.With(m => m.NumberOfFailedRecollapses, numberOfFailedRecollapses.Value);
        }

        var operation = query.Create();

        DbContext.NanoTypeRecollapseOperations.Add(operation);
        DbContext.SaveChanges();

        return operation;
    }
}