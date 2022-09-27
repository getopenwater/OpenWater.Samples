using OpenWater.ApiClient;
using OpenWater.ApiClient.JudgeAssignment;
using OpenWater.ApiClient.User;

namespace SampleConsoleApplication
{
    public class JudgeSample
    {
        private readonly OpenWaterApiClient _apiClient;
        public JudgeSample(OpenWaterApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<DetailsResponse> CreateJudgeAsync(int roundId)
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



        #region Judge creation and assignment helper methods
        private async Task<DetailsResponse> CreateUserAsync()
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
