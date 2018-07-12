﻿using DCode.Data.ReportingRepository;
using DCode.Data.TaskRepository;
using DCode.Data.UserRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using DCode.Models.Common;
using DCode.Models.Management;
using System.Configuration;
using DCode.Services.Common;

namespace DCode.Services.Reporting
{
    public class ReportingService : IReportingService
    {
        private readonly ITaskRepository _taskRepository;

        private readonly IUserRepository _userRepository;

        private readonly IDailyUsageStatisticsRepository _dailyUsageStatisticsRepository;

        private readonly IDataManagement _dbQueryManager;

        private readonly ICommonService _commonService;

        public ReportingService(ITaskRepository taskRepository,
            IUserRepository userRepository,
            IDailyUsageStatisticsRepository dailyUsageStatisticsRepository,
            IDataManagement dbQueryManager,
            ICommonService commonService)
        {
            _taskRepository = taskRepository;

            _userRepository = userRepository;

            _dailyUsageStatisticsRepository = dailyUsageStatisticsRepository;

            _dbQueryManager = dbQueryManager;

            _commonService = commonService;
        }

        public IEnumerable<string> GetSubscribedUserForTask(string task)
        {
            return _userRepository
                 .GetSubscribedUserForTask(task);
        }

        public IEnumerable<string> GetSkillsForNewTasksAddeddYesterday()
        {
            return _taskRepository
                .GetTaskCountBySkillForDate(DateTime.Now.AddDays(-1))
                ?.SkipWhile(x => x.Item2 == 0)
                ?.Select(x => x.Item1)
                .Distinct();
        }

        public IEnumerable<Tuple<string, string>> GetProjectDetailsForNewTasksAddedYesterday(string skill)
        {
            var projectDetails = _taskRepository
                  .GetProjectDetailsForNewTasksFromDateForSkill(DateTime.Now.AddDays(-1), skill);

            var result = new List<Tuple<string, string>>();

            foreach (var project in projectDetails)
            {
                result.Add(
                    Tuple.Create<string, string>(project.PROJECT_NAME, project.TASK_NAME));
            }

            return result;
        }

        public List<Tuple<string, long>> GetUserVisitsCount()
        {
            var recordsCount = new List<Tuple<string, long>>();

            var results = _dailyUsageStatisticsRepository.GetDailyStatisticsFor();

            foreach (var result in results)
            {
                recordsCount.Add(
                    Tuple.Create(
                        result.date.ToShortDateString(), result.visits));
            }

            return recordsCount;
        }

        public void UpdateDailySiteVisitCount()
        {
            _dailyUsageStatisticsRepository.UpsertDailyStatistics();
        }

        public DatabaseTable ExecuteDbQuery(string query)
        {
            return _dbQueryManager.RunQuery(query);
        }

        public IEnumerable<string> GetConsultingUsersForServiceLine(int serviceLineId)
        {
            var recipientEmailsFromConfig = 
                _commonService.GetFINotificationRecipientsForOffering(serviceLineId);

            return recipientEmailsFromConfig;
        }

        public IEnumerable<Tuple<string, string, string, int>> GetFirmInitiativeTasksCreatedYesterday()
        {
            var result =
                _taskRepository.GetFirmInitiativesForDate(DateTime.Now.AddDays(-1));

            if (result == null || result.Count() == 0)
            {
                return null;
            }

            var mappedResult = new List<Tuple<string, string, string, int>>();

            foreach (var resultItem in result)
            {
                mappedResult.Add(
                    Tuple.Create<string, string, string, int>(
                        resultItem.TASK_NAME,
                        resultItem.DETAILS,
                        resultItem.taskskills.FirstOrDefault().skill.VALUE,
                        resultItem.SERVICE_LINE_ID));
            }

            return mappedResult;
        }

        public IEnumerable<string> GetDummyConsultingUsers()
        {
            return new List<string> {
               "risen@deloitte.com",
               "shirastogi@deloitte.com"
           };
        }
    }
}
