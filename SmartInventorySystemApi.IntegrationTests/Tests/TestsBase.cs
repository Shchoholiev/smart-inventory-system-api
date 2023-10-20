namespace SmartInventorySystemApi.IntegrationTests.Tests;

public class TestsBase : IClassFixture<TestingFactory<Program>>
{
    private protected HttpClient HttpClient;

    private protected string ResourceUrl;

    public TestsBase(TestingFactory<Program> factory, string resourceUrl)
    {
        HttpClient = factory.CreateClient();
        ResourceUrl = resourceUrl;
        factory.InitialaizeDatabase();
    }
}
