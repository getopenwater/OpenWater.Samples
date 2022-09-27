using OpenWater.ApiClient;
using OpenWater.ApiClient.Application;
using OpenWater.ApiClient.FieldValues;
using OpenWater.ApiClient.Form;
using Faker;

namespace SampleConsoleApplication;

public class ApplicationSample
{
    private readonly OpenWaterApiClient _apiClient;

    public ApplicationSample(OpenWaterApiClient apiClient)
    {
        _apiClient = apiClient;
    }
    public async Task<DetailsResponse> CreateApplicationAsync(int userId, int programId)
    {
        List<FieldValueModelBase> fieldValues = PrefillApplicationFieldsValues();
        var applicationName = fieldValues.FirstOrDefault(w => w.Alias == "title") as ApplicationNameFieldValueModel;
        var applicationCategoryCodes = new[] { "00", "10" };

        var newApplication = new CreateRequest(applicationName?.FirstValue, applicationCategoryCodes[1], fieldValues, programId, userId);

        var createdApplication = await _apiClient.CreateApplicationAsync(newApplication);
        int roundId = createdApplication.RoundSubmissions.FirstOrDefault()!.RoundId;

        await _apiClient.FinalizeRoundSubmissionAsync(createdApplication.Id, roundId);
        await _apiClient.ChangeFinalizedRoundSubmissionStatusAsync(createdApplication.Id, roundId,
            new ChangeFinalizedRoundSubmissionStatusRequest { Status = OpenWater.ApiClient.Definitions.SubmissionStatus.Complete });
        //await _apiClient.SetForwardingStatusForApplicationInRoundAsync(createdApplication.Id, roundId,
        //new SetForwardingStatusForApplicationInRoundRequest { IsForwarded = true });

        return createdApplication;
    }

    #region Helper Methods
    private List<FieldValueModelBase> PrefillApplicationFieldsValues()
    {
        List<FieldValueModelBase> fieldValues = new List<FieldValueModelBase>();
        string applicationName = $"{NameFaker.FirstName()} {NameFaker.LastName()}";

        fieldValues.Add(new ApplicationNameFieldValueModel { Alias = "title", FirstValue = applicationName });
        var resumeMediaFile = _apiClient.CreateMedia(new OpenWater.ApiClient.Media.CreateRequest
        {
            FileName = $"{Guid.NewGuid()}.png",
            FileUrl = Constants.SampleLogoLink
        });
        fieldValues.Add(new FileUploadFieldValueModel($"logo", StringFaker.Alpha(9), resumeMediaFile.MediaId));
        fieldValues.Add(new CheckboxListFieldValueModel($"inYourOpinionWhatIsThisBusinesssTopQualities",
            GetOptionsInYourOpinionWhatIsThisBusinesssTopQualities()));
        fieldValues.Add(new TextFieldValueModel($"whyElse", StringFaker.Alpha(20)));
        fieldValues.Add(new TextFieldValueModel($"whyAreYouNominatingThisBusiness", TextFaker.Sentence()));
        fieldValues.Add(new CheckboxListFieldValueModel($"fieldsCanBeMadeToBeRequired", GetOptionsOfFieldsCanBeMadeToBeRequired()));
        fieldValues.Add(new TextFieldValueModel($"textFieldsCanHaveCharacterCounters", Faker.StringFaker.Alpha(5)));
        fieldValues.Add(new TextFieldValueModel($"textFieldsCanWordCounters", StringFaker.Alpha(80)));
        fieldValues.Add(new TextFieldValueModel($"singleLineMultiLineAndRichTextAllSupported", StringFaker.Alpha(99)));
        fieldValues.Add(new DateFieldValueModel($"enterADate", DateTime.UtcNow));
        fieldValues.Add(new NumberFieldValueModel($"enterANumber", NumberFaker.Number(1, 200)));
        fieldValues.Add(new TextFieldValueModel($"enterAValidEmail", InternetFaker.Email()));
        fieldValues.Add(new TextFieldValueModel($"enterAPhoneNumber", $"+10000000001"));
        fieldValues.Add(new TextFieldValueModel($"enterAWebAddressUrl", "https://www.google.com/"));
        fieldValues.Add(new AddressFieldValueModel($"enterAnAddress", "New York", "US",
            LocationFaker.Street(), "", "NY", LocationFaker.StreetName(), LocationFaker.ZipCode()));
        fieldValues.Add(new CheckboxListFieldValueModel($"checkboxListExample", GetOptionsCheckboxListExample()));
        fieldValues.Add(new TextFieldValueModel($"moreFieldsCanBeNested", StringFaker.Alpha(20)));

        var nestingWithinNestingIsAllowedField = new List<(string, string)>
        {
            ("cbf1c71e-7ccf-4e68-aa96-ecaee8d3f0e2", "Value 1"),
            ("709ba1ea-3c25-4182-8bd7-a12dd4df61d6", "Choose me to See Nesting")
        };
        fieldValues.Add(new ListFieldValueModel("nestingWithinNestingIsAllowed",
            new ListValue { Id = Guid.Parse(nestingWithinNestingIsAllowedField[1].Item1), Value = nestingWithinNestingIsAllowedField[1].Item2 }));
        fieldValues.Add(new TextFieldValueModel($"nestedFieldWithinANestedField", StringFaker.Alpha(12)));

        var dropDownExampleField = new List<(string, string)>
        {
            ("840232c3-ce92-4d65-9d8b-99c79079fc40", "Value 1"),
            ("e3699bc7-66e8-4983-83ee-8faf5facc915", "Choose Me to See Nesting")
        };
        fieldValues.Add(new ListFieldValueModel("dropDownExample",
            new ListValue { Id = Guid.Parse(dropDownExampleField[1].Item1), Value = dropDownExampleField[1].Item2 }));

        fieldValues.Add(new TextFieldValueModel($"nestedField", StringFaker.Alpha(20)));

        var radioFieldExampleField = new List<(string, string)>
        {
            ("46b10c1c-7a54-48d5-84d1-b88611ab79ce", "Value 1")
        };
        fieldValues.Add(new ListFieldValueModel("radioFieldExample",
            new ListValue { Id = Guid.Parse(radioFieldExampleField[0].Item1), Value = radioFieldExampleField[0].Item2 }));

        var uploadAPdfField = _apiClient.CreateMedia(new OpenWater.ApiClient.Media.CreateRequest
        {
            FileName = $"{Guid.NewGuid()}.pdf",
            FileUrl = Constants.SamplePdfLink
        });
        fieldValues.Add(new FileUploadFieldValueModel($"uploadAPdf", StringFaker.Alpha(9), uploadAPdfField.MediaId));

        var uploadAVideoMp4Field = _apiClient.CreateMedia(new OpenWater.ApiClient.Media.CreateRequest
        {
            FileName = $"{Guid.NewGuid()}.mp4",
            FileUrl = Constants.SampleVideoLink
        });
        fieldValues.Add(new FileUploadFieldValueModel($"uploadAVideoMp4", StringFaker.Alpha(9), uploadAVideoMp4Field.MediaId));

        var uploadAnOfficeDocumentOrAnyOtherField = _apiClient.CreateMedia(new OpenWater.ApiClient.Media.CreateRequest
        {
            FileName = $"{Guid.NewGuid()}.ppt",
            FileUrl = Constants.SamplePowerpointFileLink
        });
        fieldValues.Add(new FileUploadFieldValueModel($"uploadAnOfficeDocumentOrAnyOtherFile",
            StringFaker.Alpha(9), uploadAnOfficeDocumentOrAnyOtherField.MediaId));

        var tableRows = new List<RowModel>();
        for (int i = 0; i < 1; ++i)
        {
            var rowValues = new List<FieldValueModelBase>
            {
                new TextFieldValueModel("firstName", NameFaker.FirstName()),
                new TextFieldValueModel("lastName", NameFaker.LastName()),
            };

            var row = new RowModel(rowValues, Guid.NewGuid(), i);
            tableRows.Add(row);
        }
        fieldValues.Add(new TableFieldValueModel("tableFieldExample", tableRows));

        return fieldValues;
    }
    private List<ListValue> GetOptionsInYourOpinionWhatIsThisBusinesssTopQualities()
    {
        var checkBoxField = new List<(string, string)>
        {
            ("99588d5a-4919-4c98-81d3-de53babd974a", "Ambience"),
            ("4d5d50c8-3aa8-4ae8-9fe2-1c599b8370dd", "Bar service"),
            ("30aa2c42-4ed0-4a4f-a10f-b655d395bc84", "Drink offerings"),
            ("09376a90-5795-4fb6-90cd-9ab4cfaaba7b", "Events"),
            ("0a1c4843-03a8-4af8-a514-4b0c40aa69fc", "Food offerings"),
            ("9ba8d333-5762-4bed-a1c5-7e390f45dfb3", "Live music"),
            ("4e20138c-a6f4-44e6-8494-946ff3c38a14", "Menu pricing"),
            ("573c26a4-9d23-48f0-8a37-1f660247b1ed", "Specials"),
            ("76ed934c-9dd6-4b39-bd00-aa2e50ec1130", "Staff service"),
            ("5bdfb060-8c30-4d68-b97d-6e68b3460fb5", "Other"),
        };

        var selectedOptions = new List<ListValue>();
        Random generator = new Random();

        var selectedOption = generator.Next(0, checkBoxField.Count);
        selectedOptions.Add(new ListValue { Id = Guid.Parse(checkBoxField[selectedOption].Item1), Value = checkBoxField[selectedOption].Item2 });

        return selectedOptions;
    }
    private List<ListValue> GetOptionsOfFieldsCanBeMadeToBeRequired()
    {
        var checkBoxField = new List<(string, string)>
        {
            ("7d6d56d8-72c0-4a1d-8bdc-0f0040dfe1d6", "Cool"),
        };

        var selectedOptions = new List<ListValue>();
        selectedOptions.Add(new ListValue { Id = Guid.Parse(checkBoxField[0].Item1), Value = checkBoxField[0].Item2 });

        return selectedOptions;
    }
    private List<ListValue> GetOptionsCheckboxListExample()
    {
        var checkBoxField = new List<(string, string)>
        {
            ("eab3d7ad-89d6-4510-918a-a4e99e717682", "Value 1"),
            ("76d3ef54-45f4-4cf9-acbd-f98b6ceace0d", "Value 2"),
            ("7261e1ce-d31b-4f9d-8057-b5099d368727", "Value 3"),
            ("8ac7699e-1e36-483a-96da-3543ce114f01", "Choose Me to See Nesting"),
        };

        var selectedOptions = new List<ListValue>();
        Random generator = new Random();

        var selectedOption = 3;
        selectedOptions.Add(new ListValue { Id = Guid.Parse(checkBoxField[selectedOption].Item1), Value = checkBoxField[selectedOption].Item2 });

        return selectedOptions;
    }
    #endregion
}