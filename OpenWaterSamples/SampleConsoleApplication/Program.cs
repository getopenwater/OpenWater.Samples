using Faker;
using OpenWater.ApiClient;

namespace SampleConsoleApplication
{
    public class Program
    {
        private static OpenWaterApiClient _apiClient;
        public static async Task Main(
            )
        {
            try
            {
                var phone = PhoneFaker.Phone();
                var internationalPhone = PhoneFaker.InternationalPhone();

                _apiClient = new OpenWaterApiClient(Constants.OpenwaterDomain, Constants.OpenwaterApiKey);

                #region 1.Creating sample applicant user
                //var applicantSample = new ApplicantSample(_apiClient);
                //var createdApplicantResponse = await applicantSample.CreateApplicantAsync();
                #endregion

                #region 2.Creating sample application with the applicant created in step 1
                var applicationSample = new ApplicationSample(_apiClient);
                var createdApplicationResponse = await applicationSample.CreateApplicationAsync(57402, Constants.ProgramId);
                #endregion

                //int roundId = createdApplicationResponse.RoundSubmissions.FirstOrDefault()!.RoundId;
                //int applicationId = createdApplicationResponse.Id;

                //#region 3. Creating a judge user
                //var judgeSample = new JudgeSample(_apiClient);
                //var createdJudgeResponse = await judgeSample.CreateJudgeAsync(roundId);
                //#endregionPhoneFaker.Phone()

                //int judgeId = createdJudgeResponse.Id;

                //#region 4.Assigning the created judge in step 3 to the application created in step 2
                //await judgeSample.AssignJudgeToApplication(applicationId, judgeId, roundId);
                //#endregion

                // judge the application with the assigned judge
                // run a report to pull the newly created
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Something went wrong: {ex.Message}");
            }
        }
    }
}