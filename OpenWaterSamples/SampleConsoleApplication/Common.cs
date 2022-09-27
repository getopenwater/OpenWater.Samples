using Faker;
using OpenWater.ApiClient.User;

namespace SampleConsoleApplication;

public static class Common
{
    public static CreateRequest GenerateSampleUser(bool isApplicant)
    {
        return new OpenWater.ApiClient.User.CreateRequest
        {
            FirstName = NameFaker.FirstName(),
            LastName = NameFaker.LastName(),
            Email = InternetFaker.Email(),
            Password = StringFaker.Alpha(8),
            IsApplicant = isApplicant
        };
    }
}
