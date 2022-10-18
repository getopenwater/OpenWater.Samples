using Newtonsoft.Json;
using OpenWater.ApiClient;
using OpenWater.ApiClient.Definitions;
using System.Net;
using System.Text;

namespace SampleConsoleApplication;

public class ReportSample
{
    private readonly OpenWaterApiClient _apiClient;
    public ReportSample(OpenWaterApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<ReportResult> GenerateApplicationsAndEvaluationsReportAsync()
    {
        return await _apiClient.RetrieveReportResult<ReportResult>(Constants.ApplicationsAndEvaluationsReportId);
    }
}

public static class OpenWaterReportExtensions
{
    public static async Task<string> GetReportUrl(OpenWaterApiClient apiClient, int reportId)
    {
        var runRequestAsJSON = new OpenWater.ApiClient.ReportRunner.RunRequest("json");
        var reportJob = await apiClient.RunReportAsync(reportId, runRequestAsJSON);
        int reportJobId = (int)reportJob.JobId;
        var reportJobState = apiClient.GetJobById(reportJobId).JobState;
        while (reportJobState != StateType.Succeeded)
        {
            Thread.Sleep(5000);
            reportJobState = apiClient.GetJobById(reportJobId).JobState;
        }
        string reportURL = apiClient.GetJobById(reportJobId).ResultUrl;
        return reportURL;
    }

    public static async Task<T> RetrieveReportResult<T>(this OpenWaterApiClient apiClient, int reportId)
    {
        string reportURL = await GetReportUrl(apiClient, reportId);

        using (WebClient webClient = new WebClient())
        {
            webClient.Encoding = Encoding.UTF8;
            var jsonData = webClient.DownloadString(reportURL);
            var reportData = JsonConvert.DeserializeObject<T>(jsonData);
            return reportData;
        }
    }
}

#region Application and Judge Report Models
public class ReportItem
{
    [JsonProperty("(Nested Fields (Demo)) Drop Down Example")]
    public string? NestedFieldsDemoDropDownExample { get; set; }

    [JsonProperty("(Nested Fields (Demo)) Nested Field Example")]
    public string? NestedFieldsDemoNestedFieldExample { get; set; }
    public string? JudgeFirstName { get; set; }

    [JsonProperty("(Nested Fields (Demo)) Nesting within Nesting is Allowed")]
    public string? NestedFieldsDemoNestingWithinNestingIsAllowed { get; set; }

    [JsonProperty("(Business Information) Name of Business")]
    public string? BusinessInformationNameOfBusiness { get; set; }

    [JsonProperty("(More Fields (Demo)) Enter an Address.CountryName")]
    public string? MoreFieldsDemoEnterAnAddressCountryName { get; set; }

    [JsonProperty("(Nested Fields (Demo)) Checkbox List Example")]
    public string? NestedFieldsDemoCheckboxListExample { get; set; }

    [JsonProperty("(File Uploads (Demo)) Upload a Video (mp4).MediaPreviewUrl")]
    public string? FileUploadsDemoUploadAVideoMp4MediaPreviewUrl { get; set; }

    [JsonProperty("(Table Field (Demo)) Table Field Example #0 - Person #0")]
    public string? TableFieldDemoTableFieldExample0Person0 { get; set; }

    [JsonProperty("(More Fields (Demo)) Enter an Address.Line2")]
    public string? MoreFieldsDemoEnterAnAddressLine2 { get; set; }

    [JsonProperty("(Nested Fields (Demo)) Radio Field Example")]
    public string? NestedFieldsDemoRadioFieldExample { get; set; }

    [JsonProperty("(More Fields (Demo)) Text Fields Can Have Character Counters")]
    public string? MoreFieldsDemoTextFieldsCanHaveCharacterCounters { get; set; }

    [JsonProperty("(File Uploads (Demo)) Upload a Video (mp4).Caption")]
    public string? FileUploadsDemoUploadAVideoMp4Caption { get; set; }

    [JsonProperty("(File Uploads (Demo)) Upload a PDF.MediaFileName")]
    public string? FileUploadsDemoUploadAPDFMediaFileName { get; set; }

    [JsonProperty("(More Fields (Demo)) Single Line, Multi-Line and Rich Text all Supported")]
    public string? MoreFieldsDemoSingleLineMultiLineAndRichTextAllSupported { get; set; }
    public string? CommentsScore { get; set; }

    [JsonProperty("(Business Information) In your opinion, what is this business’s top qualities?")]
    public string? BusinessInformationInYourOpinionWhatIsThisBusinessSTopQualities { get; set; }

    [JsonProperty("(File Uploads (Demo)) Upload an Office Document or any other file.MediaFileName")]
    public string? FileUploadsDemoUploadAnOfficeDocumentOrAnyOtherFileMediaFileName { get; set; }
    public string? HowWellDidThisApplicantPerformScore { get; set; }

    [JsonProperty("(Nested Fields (Demo)) More fields can be nested!")]
    public string? NestedFieldsDemoMoreFieldsCanBeNested { get; set; }

    [JsonProperty("(File Uploads (Demo)) Upload an Office Document or any other file.MediaPreviewUrl")]
    public string? FileUploadsDemoUploadAnOfficeDocumentOrAnyOtherFileMediaPreviewUrl { get; set; }
    public string? JudgingQuestionsScore { get; set; }

    [JsonProperty("(Nested Fields (Demo)) Nested Field")]
    public string? NestedFieldsDemoNestedField { get; set; }

    [JsonProperty("(File Uploads (Demo)) Upload a Video (mp4).MediaUrl")]
    public string? FileUploadsDemoUploadAVideoMp4MediaUrl { get; set; }

    [JsonProperty("(More Fields (Demo)) Enter a Date")]
    public string? MoreFieldsDemoEnterADate { get; set; }
    public string? HowWellDidThisApplicantPerformText { get; set; }

    [JsonProperty("(More Fields (Demo)) Enter a Number")]
    public string? MoreFieldsDemoEnterANumber { get; set; }

    [JsonProperty("(Table Field (Demo)) Table Field Example #0 - First Name")]
    public string? TableFieldDemoTableFieldExample0FirstName { get; set; }

    [JsonProperty("(File Uploads (Demo)) Upload a PDF.MediaPreviewUrl")]
    public string? FileUploadsDemoUploadAPDFMediaPreviewUrl { get; set; }
    public string? ApplicantLastName { get; set; }

    [JsonProperty("(More Fields (Demo)) Text Fields can have Word Counters")]
    public string? MoreFieldsDemoTextFieldsCanHaveWordCounters { get; set; }

    [JsonProperty("(Business Information) Logo.Caption")]
    public string? BusinessInformationLogoCaption { get; set; }

    [JsonProperty("(More Fields (Demo)) Enter a Web Address / URL")]
    public string? MoreFieldsDemoEnterAWebAddressURL { get; set; }

    [JsonProperty("(More Fields (Demo)) Enter an Address.State")]
    public string? MoreFieldsDemoEnterAnAddressState { get; set; }

    [JsonProperty("(More Fields (Demo)) Enter an Address.CountryCode")]
    public string? MoreFieldsDemoEnterAnAddressCountryCode { get; set; }

    [JsonProperty("(Business Information) Why else?")]
    public string? BusinessInformationWhyElse { get; set; }

    [JsonProperty("(Business Information) Logo.MediaUrl")]
    public string? BusinessInformationLogoMediaUrl { get; set; }

    [JsonProperty("(More Fields (Demo)) Enter a Valid Email")]
    public string? MoreFieldsDemoEnterAValidEmail { get; set; }

    [JsonProperty("(Business Information) Expand the Demo Fields?")]
    public string? BusinessInformationExpandTheDemoFields { get; set; }

    [JsonProperty("(File Uploads (Demo)) Upload a PDF.Caption")]
    public string? FileUploadsDemoUploadAPDFCaption { get; set; }

    [JsonProperty("(More Fields (Demo)) Enter an Address.Zip")]
    public string? MoreFieldsDemoEnterAnAddressZip { get; set; }
    public string? ApplicantFirstName { get; set; }
    public string? ApplicantEmail { get; set; }

    [JsonProperty("(Business Information) Why are you nominating this business?")]
    public string? BusinessInformationWhyAreYouNominatingThisBusiness { get; set; }

    [JsonProperty("(More Fields (Demo)) Enter a Phone Number")]
    public string? MoreFieldsDemoEnterAPhoneNumber { get; set; }

    [JsonProperty("(Nested Fields (Demo)) Nested Field within a Nested Field")]
    public string? NestedFieldsDemoNestedFieldWithinANestedField { get; set; }

    [JsonProperty("(More Fields (Demo)) Enter an Address.Street")]
    public string? MoreFieldsDemoEnterAnAddressStreet { get; set; }

    [JsonProperty("(File Uploads (Demo)) Upload a Video (mp4).MediaFileName")]
    public string? FileUploadsDemoUploadAVideoMp4MediaFileName { get; set; }

    [JsonProperty("(Table Field (Demo)) Table Field Example #0 - Last Name")]
    public string? TableFieldDemoTableFieldExample0LastName { get; set; }

    [JsonProperty("(File Uploads (Demo)) Upload an Office Document or any other file.Caption")]
    public string? FileUploadsDemoUploadAnOfficeDocumentOrAnyOtherFileCaption { get; set; }
    public string? JudgeEmail { get; set; }

    [JsonProperty("(More Fields (Demo)) Fields can be made to be Required")]
    public string? MoreFieldsDemoFieldsCanBeMadeToBeRequired { get; set; }
    public string? JudgeLastName { get; set; }

    [JsonProperty("(More Fields (Demo)) Enter an Address.City")]
    public string? MoreFieldsDemoEnterAnAddressCity { get; set; }

    [JsonProperty("(Business Information) Logo.MediaPreviewUrl")]
    public string? BusinessInformationLogoMediaPreviewUrl { get; set; }

    [JsonProperty("(More Fields (Demo)) Enter an Address.Line3")]
    public string? MoreFieldsDemoEnterAnAddressLine3 { get; set; }

    [JsonProperty("(File Uploads (Demo)) Upload a PDF.MediaUrl")]
    public string? FileUploadsDemoUploadAPDFMediaUrl { get; set; }

    [JsonProperty("(Business Information) Logo.MediaFileName")]
    public string? BusinessInformationLogoMediaFileName { get; set; }

    [JsonProperty("(File Uploads (Demo)) Upload an Office Document or any other file.MediaUrl")]
    public string? FileUploadsDemoUploadAnOfficeDocumentOrAnyOtherFileMediaUrl { get; set; }
}

public class ReportResult
{
    public List<ReportItem>? Items { get; set; }
}

#endregion

