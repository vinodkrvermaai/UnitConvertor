using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using UnitConversion.Application.DTOs;

namespace UnitConversion.IntegrationTests;

[TestFixture]
public sealed class ConversionApiTests
{
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;

    [SetUp]
    public void SetUp()
    {
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();
    }

    [TearDown]
    public void TearDown()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    [Test]
    public async Task GetConversion_MeterToFoot_Returns200()
    {
        var response = await _client.GetAsync("/api/v1/conversion?value=1&from=meter&to=foot");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var body = await response.Content.ReadFromJsonAsync<ConversionResponse>();
        Assert.That(body, Is.Not.Null);
        Assert.That(body!.Result, Is.EqualTo(3.280840m));
        Assert.That(body.Category, Is.EqualTo("Length"));
    }

    [Test]
    public async Task GetConversion_UnknownUnit_Returns400()
    {
        var response = await _client.GetAsync("/api/v1/conversion?value=1&from=invalid&to=meter");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task GetConversion_CrossCategory_Returns400()
    {
        var response = await _client.GetAsync("/api/v1/conversion?value=1&from=meter&to=kilogram");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task Health_Returns200()
    {
        var response = await _client.GetAsync("/health");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task GetUnits_ReturnsMeterAndMore()
    {
        var response = await _client.GetAsync("/api/v1/units");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var symbols = doc.RootElement.EnumerateArray().Select(e => e.GetProperty("symbol").GetString()).ToList();

        Assert.That(symbols, Does.Contain("meter"));
        Assert.That(symbols, Does.Contain("celsius"));
        Assert.That(symbols, Does.Contain("kilogram"));
    }
}