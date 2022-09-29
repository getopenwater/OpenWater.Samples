using Newtonsoft.Json;
using OpenWater.ApiClient;

namespace SampleConsoleApplication
{
    public class Program
    {
        public static async Task Main()
        {
            try
            {
                OpenWaterApiClient apiClient = new OpenWaterApiClient(Constants.OpenWaterDomain, Constants.OpenWaterApiKey);

                #region 1.Creating sample applicant user
                var applicantSample = new ApplicantSample(apiClient);
                var createdApplicantResponse = await applicantSample.CreateApplicantAsync();
                Console.WriteLine($"------- A Sample applicant is created with id '{createdApplicantResponse.Id}' -------\n");
                #endregion

                #region 2.Creating sample application with the applicant created in step 1
                var applicationSample = new ApplicationSample(apiClient);
                var createdApplicationResponse = await applicationSample.CreateApplicationAsync(createdApplicantResponse.Id, Constants.ProgramId);
                Console.WriteLine($"------- A Sample application is created with id '{createdApplicationResponse.Id}' -------\n");
                #endregion

                int roundId = createdApplicationResponse.RoundSubmissions.FirstOrDefault()!.RoundId;
                int applicationId = createdApplicationResponse.Id;

                #region 3. Creating a judge user
                var judgeSample = new JudgeSample(apiClient);
                var createdJudgeResponse = await judgeSample.CreateJudgeAsync(roundId);
                Console.WriteLine($"------- A Sample judge is created with id '{createdJudgeResponse.Id}' -------\n");
                #endregion

                int judgeId = createdJudgeResponse.Id;

                #region 4.Assigning the created judge in step 3 to the application created in step 2
                await judgeSample.AssignJudgeToApplication(applicationId, judgeId, roundId);
                Console.WriteLine("------- The new judge is assigned to the new application -------\n");
                #endregion

                var evaluations = await apiClient.GetEvaluationsAsync(Constants.ProgramId, roundId, applicationId);
                var applicationEvaluation = evaluations.Items.FirstOrDefault(e => e.ApplicationId == applicationId);

                #region 5.Judge the application with the assigned judge
                await judgeSample.EvaluateApplicationAsync(applicationEvaluation.Id);
                Console.WriteLine("------- The new application is evaluated -------\n");
                #endregion


                #region 6.Pulling applications and evaluations data with a custom report
                ReportSample reportSample = new ReportSample(apiClient);
                var reportData = await reportSample.GenerateApplicationsAndEvaluationsReportAsync();
                var formattedReportData = JsonConvert.SerializeObject(reportData, Formatting.Indented);
                Console.WriteLine(formattedReportData);
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Something went wrong: {ex.Message}");
            }
        }
    }
}