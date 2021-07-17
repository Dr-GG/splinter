using System;
using AutoFixture;
using System.Linq;
using Splinter.NanoInstances.Database.DbContext;
using Splinter.NanoInstances.Database.DbContext.Models;
using Splinter.NanoTypes.Domain.Enums;
using OperatingSystem = Splinter.NanoTypes.Domain.Enums.OperatingSystem;

namespace Splinter.NanoInstances.Database.Tests.Utilities
{
    public class MockTeraDbContextBuilder
    {
        private readonly Fixture _fixture;
        private readonly TeraDbContext _dbContext;

        public MockTeraDbContextBuilder(TeraDbContext teraDbContext)
        {
            _dbContext = teraDbContext;
            _fixture = new Fixture();

            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _fixture.Behaviors
                .OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(x => _fixture.Behaviors.Remove(x));
        }

        public OperatingSystemModel AddOperatingSystem(
            long id = 0,
            string? description = null,
            ProcessorArchitecture? architecture = null,
            OperatingSystem? type = null)
        {
            var query = _fixture.Build<OperatingSystemModel>()
                .With(m => m.Id, id);

            if (!string.IsNullOrEmpty(description))
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

            _dbContext.OperatingSystems.Add(operatingSystem);
            _dbContext.SaveChanges();

            return operatingSystem;
        }

        public TeraPlatformModel AddTeraPlatform(
            long operatingSystemId,
            long id,
            Guid? teraId = null,
            TeraPlatformStatus status = TeraPlatformStatus.Running,
            string? frameworkDescription = null)
        {
            var query = _fixture.Build<TeraPlatformModel>()
                .With(m => m.Id, id)
                .With(m => m.OperatingSystemId, operatingSystemId)
                .With(m => m.TeraId, teraId ?? Guid.NewGuid())
                .With(m => m.Status, status)
                .Without(m => m.OperatingSystem);

            if (!string.IsNullOrEmpty(frameworkDescription))
            {
                query = query.With(m => m.FrameworkDescription, frameworkDescription);
            }

            var platform = query.Create();

            _dbContext.TeraPlatforms.Add(platform);
            _dbContext.SaveChanges();

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
            var query = _fixture.Build<NanoTypeModel>()
                .With(m => m.Id, id)
                .With(m => m.Guid, guid ?? Guid.NewGuid());

            if (!string.IsNullOrEmpty(name))
            {
                query = query.With(m => m.Name, name);
            }

            if (!string.IsNullOrEmpty(version))
            {
                query = query.With(m => m.Version, version);
            }

            var nanoType = query.Create();

            _dbContext.NanoTypes.Add(nanoType);
            _dbContext.SaveChanges();

            return nanoType;
        }

        public NanoInstanceModel AddNanoInstance(
            long nanoTypeId,
            long id,
            Guid? guid = null,
            string? name = null,
            string? version = null)
        {
            var query = _fixture.Build<NanoInstanceModel>()
                .With(m => m.Id, id)
                .With(m => m.NanoTypeId, nanoTypeId)
                .With(m => m.Guid, guid ?? Guid.NewGuid())
                .Without(m => m.NanoType);

            if (!string.IsNullOrEmpty(name))
            {
                query = query.With(m => m.Name, name);
            }

            if (!string.IsNullOrEmpty(version))
            {
                query = query.With(m => m.Version, version);
            }

            var nanoInstance = query.Create();

            _dbContext.NanoInstances.Add(nanoInstance);
            _dbContext.SaveChanges();

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
            var teraAgent = _fixture.Build<TeraAgentModel>()
                .With(m => m.Id, id)
                .With(m => m.TeraPlatformId, platformId)
                .With(m => m.NanoInstanceId, nanoInstanceId)
                .With(m => m.TeraId, teraId ?? Guid.NewGuid())
                .With(m => m.Status, status)
                .Without(m => m.NanoInstance)
                .Without(m => m.TeraPlatform)
                .Create();

            _dbContext.TeraAgents.Add(teraAgent);
            _dbContext.SaveChanges();

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
            var query = _fixture.Build<TeraMessageModel>()
                .With(m => m.Id, id)
                .With(m => m.SourceTeraAgentId, sourceTeraAgentId)
                .With(m => m.RecipientTeraAgentId, recipientTeraAgentId)
                .With(m => m.Message, message ?? GetHashCode().ToString())
                .With(m => m.DequeueCount, dequeueCount)
                .With(m => m.Code, code)
                .Without(m => m.ErrorCode)
                .Without(m => m.ErrorMessage)
                .Without(m => m.ErrorStackTrace)
                .Without(m => m.Pending)
                .Without(m => m.SourceTeraAgent)
                .Without(m => m.RecipientTeraAgent);

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

            if (!string.IsNullOrEmpty(errorMessage))
            {
                query = query.With(m => m.ErrorMessage, errorMessage);
            }

            if (!string.IsNullOrEmpty(errorStackTrace))
            {
                query = query.With(m => m.ErrorStackTrace, errorStackTrace);
            }

            var dbMessage = query.Create();

            _dbContext.TeraMessages.Add(dbMessage);
            _dbContext.SaveChanges();

            if (addPending)
            {
                AddPendingTeraMessage(
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
            var query = _fixture.Build<TeraAgentNanoTypeDependencyModel>()
                .With(m => m.Id, id)
                .With(m => m.TeraAgentId, teraAgentId)
                .With(m => m.NanoTypeId, nanoTypeId)
                .Without(m => m.NanoType)
                .Without(m => m.TeraAgent);

            if (numberOfDependencies.HasValue)
            {
                query = query.With(m => m.NumberOfDependencies, numberOfDependencies.Value);
            }

            var model = query.Create();

            _dbContext.TeraAgentNanoTypeDependencies.Add(model);
            _dbContext.SaveChanges();

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

            _dbContext.PendingTeraMessages.Add(pending);
            _dbContext.SaveChanges();

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
            var query = _fixture.Build<NanoTypeRecollapseOperationModel>()
                .With(m => m.Id, id)
                .With(m => m.NanoTypeId, nanoTypeId)
                .Without(m => m.NanoType);

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

            _dbContext.NanoTypeRecollapseOperations.Add(operation);
            _dbContext.SaveChanges();

            return operation;
        }

        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }
    }
}
