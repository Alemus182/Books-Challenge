using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Test
{
    public class AuthIntegrationTest
    {

        [Fact]
        public async Task PostOk()
        {
            // Arrange
            var app = new ApiApplication();

            // Act
            var client = app.CreateClient();
            var body = new
            {
                username = "test@nexttech.com",
                password = "3454365464"
            };

            var dataToPost = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/api/auth/login",dataToPost);
            var responseBody = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task PostFailValidation()
        {
            // Arrange
            var app = new ApiApplication();

            // Act
            var client = app.CreateClient();
            var body = new
            {
                username = "testnexttech.com",
                password = "3242"
            };

            var dataToPost = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/api/auth/login", dataToPost);
            var responseBody = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
