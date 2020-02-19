using Junto.Api.Controllers;
using Junto.Application.Entity;
using Junto.Application.Models;
using Junto.Application.Repository;
using Junto.Application.Services;
using Junto.Infra.Repository;
using Junto.Test.Helper;
using JWT.Algorithms;
using JWT.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Junto.Test.Controllers
{
    public class AuthControllerTest
    {
        [Fact]
        public async Task UserNotAuthorized()
        {
            var mockConfiguration = new Mock<IConfiguration>();

            var repository = new UserRepository(DatabaseHelper.GetContext(), new Mock<ILogger<UserRepository>>().Object);
            var service = new UserService(repository);
            var controller = new AuthController(service, mockConfiguration.Object);
            var result = await controller.CreateToken(new UserAuthentication()
            {
                Login = "teste",
                Password = "teste"
            });
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task UserAuthorized()
        {
            var userRequest = new UserAuthentication()
            {
                Login = "TésTeLogin",
                Password = "testePassword"
            };

            var login = Application.Helpers.StringHelper.Normalize(userRequest.Login);
            var password = Application.Helpers.Sha256Helper.Convert(userRequest.Password);

            var user = new User()
            {
                Id = 3,
                Login = login,
                Name = "testeName",
                Password = password
            };

            var secret = "aaaa29ecc7dfc2ea4dd4e1a114d17066bb66dfc6bbf587af51a1a55568fcbd7b";

            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(x => x["JwtToken:SecretKey"]).Returns(secret);
            mockConfiguration.Setup(x => x["JwtToken:Issuer"]).Returns("http://localhost:5000");

            var context = DatabaseHelper.GetContext();
            context.User.Add(user);
            context.SaveChanges();

            var repository = new UserRepository(context, new Mock<ILogger<UserRepository>>().Object);
            var service = new UserService(repository);
            var controller = new AuthController(service, mockConfiguration.Object);
            var result = await controller.CreateToken(userRequest);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var data = okResult.ToDictionary();

            Assert.True(data != null && data.ContainsKey("Token"));

            var payload = new JwtBuilder()
                .WithSecret(secret)
                .MustVerifySignature()
                .WithAlgorithm(new HMACSHA256Algorithm())
                .Decode<IDictionary<string, object>>(data["Token"]);
            
            Assert.True(payload != null && payload.Any());
            Assert.True(payload.ContainsKey(System.Security.Claims.ClaimTypes.NameIdentifier));
            Assert.Equal(payload[System.Security.Claims.ClaimTypes.NameIdentifier], user.Id.ToString());
        }
    }
}
