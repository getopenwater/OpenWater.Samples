using OpenWater.ApiClient;
using OpenWater.ApiClient.Evaluation;
using OpenWater.ApiClient.JudgeAssignment;
using Faker;

namespace SampleConsoleApplication
{
    public class JudgeSample
    {
        private readonly OpenWaterApiClient _apiClient;
        public JudgeSample(OpenWaterApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<OpenWater.ApiClient.User.DetailsResponse> CreateJudgeAsync(int roundId)
        {
            var createdUser = await CreateUserAsync();
            await MarkUserAsJudgeAsync(createdUser.Id, roundId);
            return createdUser;
        }

        public async Task AssignJudgeToApplication(int applicationId, int judgeId, int roundId)
        {
            var assignJudgeToApplicationRequest = new AssignJudgeToApplicationRequest
            {
                ApplicationId = applicationId,
                JudgeUserId = judgeId,
                RoundId = roundId
            };
            await _apiClient.AssignJudgeToApplicationAsync(assignJudgeToApplicationRequest);
        }

        public async Task EvaluateApplicationAsync(int evaluationId)
        {
            var scoringAnswers = new List<GeneralScoringAnswerModel>
            {
                new GeneralScoringAnswerModel
                {
                    Alias = "comments",
                    Score = Convert.ToDouble(NumberFaker.Number(1,5)),
                    Text = "This applicant performed well",
                },
                new GeneralScoringAnswerModel
                {
                    Alias = "howWellDidThisApplicantPerform",
                    Text = "Did a Great Job",
                    Score = 2
                }
            };
            var updateEvaluationFormRequest = new UpdateEvaluationFormRequest
            {
                GeneralScoringAnswers = scoringAnswers,
                FinalizeScore = true
            };
            await _apiClient.UpdateEvaluationFormAsync(evaluationId, updateEvaluationFormRequest);
        }

        #region Judge creation and assignment helper methods
        private async Task<OpenWater.ApiClient.User.DetailsResponse> CreateUserAsync()
        {
            var createUserRequest = Common.GenerateSampleUser(isApplicant: false);
            return await _apiClient.CreateUserAsync(createUserRequest);
        }
        private async Task MarkUserAsJudgeAsync(int userId, int roundId)
        {
            var createJudgeRequest = new AssignJudgeToRoundRequest
            {
                JudgeUserId = userId,
                RoundId = roundId
            };
            await _apiClient.AssignJudgeToRoundAsync(createJudgeRequest);
        }
        #endregion
    }
}
