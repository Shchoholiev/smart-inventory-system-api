using System.Net.Http.Headers;
using System.Net.Http.Json;
using SmartInventorySystemApi.Application.Models.Identity;

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

    public async Task LoginAsync(string email, string password)
    {
        var login = new Login
        {
            Email = email,
            Password = password
        };

        var response = await HttpClient.PostAsJsonAsync($"users/login", login);
        var tokens = await response.Content.ReadFromJsonAsync<TokensModel>();

        HttpClient.DefaultRequestHeaders.Authorization 
            = new AuthenticationHeaderValue("Bearer", tokens?.AccessToken);
    }
}
