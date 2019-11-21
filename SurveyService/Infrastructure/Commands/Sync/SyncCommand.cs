using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Common.Infrastructure.Commands;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SurveyService.Models.Db;
using SurveyService.Models.Subordinates;
using SurveyService.Services;
using SurveyService.Services.Contracts;
using SurveyService.Services.Disciplines;
using SurveyService.Services.Tests;

namespace Infrastructure.Commands.Sync
{
    /// <summary>
    ///     Команда синхронизации с остальными микросервисами
    /// </summary>
    public class SyncCommand: Command
    {
        private readonly SyncCommandArguments _args;

        /// <summary>
        ///     Создание экземпляра класса <see cref="SyncCommand"/>
        /// </summary>
        /// <param name="args"></param>
        public SyncCommand(SyncCommandArguments args)
        {
            _args = args ?? throw new ArgumentNullException(nameof(args));
        }
        /// <inheritdoc/>
        public override string Title => "Force sync command";

        /// <inheritdoc/>
        public override void Execute(IServiceProvider services)
        {
            using (var scope = services.CreateScope())
            {
                Console.WriteLine("Services syncing starting");
                SyncDisciplines(scope.ServiceProvider.GetRequiredService<DisciplinesService>(),
                    scope.ServiceProvider.GetRequiredService<SurveysDbContext>());
                SyncTests(scope.ServiceProvider.GetRequiredService<TestsService>(),
                    scope.ServiceProvider.GetRequiredService<SurveysDbContext>());
                Console.WriteLine("Services syncing successful");
            }
        }

        #region tests

        private static void SyncTests(TestsService testsService, SurveysDbContext context)
        {
            Console.WriteLine("Tests syncing starting");
            var tests = testsService.GetTestsAsync().GetAwaiter().GetResult().ToList();
            var testsDict = tests.ToDictionary(x => x.Id, x => x);
            var localTests = context.Tests.ToList();

            var localTestsToDeletedIds =
                localTests.Select(x => x.ExternalId).Except(tests.Select(x => x.Id)).ToImmutableHashSet();
            var testsToAddIds = tests.Select(x => x.Id).Except(localTests.Select(x => x.ExternalId)).ToImmutableHashSet();

            UpdateTests(localTests, localTestsToDeletedIds, testsDict);

            AddTests(context, testsToAddIds, testsDict);

            context.SaveChanges();
            Console.WriteLine("Tests syncing ended");
        }


        private static void AddTests(SurveysDbContext context, ImmutableHashSet<string> testsToAddIds, IDictionary<string, ExternalTestDTO> testsDict)
        {
            foreach (var testToAddId in testsToAddIds)
            {
                var addedby = testsDict[testToAddId];
                var newTest = new Test { ExternalId = testToAddId };
                ReplaceTest(newTest, addedby);
                context.Tests.Add(newTest);
            }
        }

        private static void UpdateTests(IList<Test> localTests, ImmutableHashSet<string> localTestsToDeletedIds, IDictionary<string, ExternalTestDTO> testsDict)
        {
            foreach (var localTest in localTests)
            {
                if (localTestsToDeletedIds.Contains(localTest.ExternalId))
                {
                    localTest.IsDeleted = true;
                    continue;
                }

                var replacedBy = testsDict[localTest.ExternalId];
                ReplaceTest(localTest, replacedBy);
            }
        }

        /// <summary>
        ///     Обновление теста.
        /// </summary>
        /// <param name="localTest">тест, который необходимо обновить</param>
        /// <param name="replacedBy">тест, которым надо заменить</param>
        internal static void ReplaceTest(Test localTest, ExternalTestDTO replacedBy)
        {
            localTest.Name = replacedBy.Name;
            localTest.QuestionsCount = replacedBy.Questions.Count();
        }
        #endregion

        #region disciplines

        private static void SyncDisciplines(DisciplinesService disciplineService, SurveysDbContext context)
        {
            Console.WriteLine("Disciplines syncing starting");
            try
            {
                var disciplines = disciplineService.GetDisciplinesAsync().GetAwaiter().GetResult().ToList();
                var disciplinesDict = disciplines.ToDictionary(x => x.Id, x => x);
                var localDisciplines = context.Disciplines.ToList();

                var localDisciplinesToDeletedIds =
                    localDisciplines.Select(x => x.Id).Except(disciplines.Select(x => x.Id)).ToImmutableHashSet();
                var discplineToAddIds = disciplines.Select(x => x.Id).Except(localDisciplines.Select(x => x.Id)).ToImmutableHashSet();

                UpdateDisciplines(localDisciplines, localDisciplinesToDeletedIds, disciplinesDict);

                AddDisciplines(context, discplineToAddIds, disciplinesDict);

                context.SaveChanges();

            }
            catch (ExternalHttpServiceException e)
            {
                throw new CommandExecutionException("Ошибка с доступом к сервису дисциплин", e);
            }
            
            Console.WriteLine("Disciplines syncing ended");
        }

        /// <summary>
        ///     Обновление дисциплины.
        /// </summary>
        /// <param name="localDiscipline">дисциплина, которую необходимо обновить</param>
        /// <param name="replacedBy">дисциплина, значениями которой надо заменить</param>
        internal static void ReplaceDiscipline(Discipline localDiscipline, ExternalDisciplineDTO replacedBy)
        {
            localDiscipline.Name = replacedBy.Name;
        }

        private static void UpdateDisciplines(IList<Discipline> localDisciplines, ImmutableHashSet<long> localDisciplinesToDeletedIds,
            IDictionary<long, ExternalDisciplineDTO> disciplinesDict)
        {
            foreach (var localDiscipline in localDisciplines)
            {
                if (localDisciplinesToDeletedIds.Contains(localDiscipline.Id))
                {
                    localDiscipline.IsDeleted = true;
                    continue;
                }

                var replacedBy = disciplinesDict[localDiscipline.Id];
                ReplaceDiscipline(localDiscipline, replacedBy);
            }
        }

        private static void AddDisciplines(SurveysDbContext context, ImmutableHashSet<long> discplineToAddIds,
            IDictionary<long, ExternalDisciplineDTO> disciplinesDict)
        {
            foreach (var discplineToAddId in discplineToAddIds)
            {
                var addedby = disciplinesDict[discplineToAddId];
                var newDiscipline = new Discipline { Id = discplineToAddId };
                ReplaceDiscipline(newDiscipline, addedby);
                context.Disciplines.Add(newDiscipline);
            }
        }

        #endregion
    }
}
