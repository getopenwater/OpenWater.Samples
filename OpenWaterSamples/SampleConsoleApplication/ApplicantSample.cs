using OpenWater.ApiClient;
using OpenWater.ApiClient.User;

namespace SampleConsoleApplication;

public class ApplicantSample
{
    private readonly OpenWaterApiClient _apiClient;
    public ApplicantSample(OpenWaterApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<DetailsResponse> CreateApplicantAsync()
    {
        var createUserRequest = Common.GenerateSampleUser(isApplicant: true);
        return await _apiClient.CreateUserAsync(createUserRequest);
    }
}
