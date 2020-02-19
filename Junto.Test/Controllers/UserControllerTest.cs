using FluentAssertions;
using FluentValidation.TestHelper;
using Junto.Api.Controllers;
using Junto.Application.Entity;
using Junto.Application.Exceptions;
using Junto.Application.Models;
using Junto.Application.Models.Validator;
using Junto.Application.Repository;
using Junto.Application.Services;
using Junto.Infra.Repository;
using Junto.Test.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Junto.Test.Controllers
{
    public class UserControllerTest
    {
        [Fact]
        public async Task GetAllUsersSuccess()
        {
            var context = DatabaseHelper.GetContext();

            var user1 = new User() { Id = 75, Login = "logintest1", Name = "nametest1", Password = "passwordtest1" };
            var user2 = new User() { Id = 44, Login = "logintest2", Name = "nametest2", Password = "passwordtest2" };

            context.User.AddRange(new List<User>() { user1, user2 });
            await context.SaveChangesAsync();

            var repository = new UserRepository(context, new Mock<ILogger<UserRepository>>().Object);
            var service = new UserService(repository);
            var controller = new UserController(service, new Mock<IHttpContextAccessor>().Object);
            var result = await controller.GetAll();
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var data = okResult.ToList<UserResponse>();
            data.Should().HaveCount(2).And.BeOfType<List<UserResponse>>().Which.Last().Id.Should().Be(user2.Id);
        }

        [Fact]
        public async Task GetAllUsersNotFound()
        {
            var context = DatabaseHelper.GetContext();
            var repository = new UserRepository(context, new Mock<ILogger<UserRepository>>().Object);
            var service = new UserService(repository);
            var controller = new UserController(service, new Mock<IHttpContextAccessor>().Object);
            var exception = await Assert.ThrowsAsync<NotFoundException>(() => controller.GetAll());
            exception.Should().BeOfType<NotFoundException>().Which.Message.Should().Be("users not found");
        }

        [Fact]
        public async Task GetOneUserSuccess()
        {
            var context = DatabaseHelper.GetContext();

            var user = new User() { Id = 55, Login = "logintest1", Name = "nametest1", Password = "passwordtest1" };

            context.User.Add(user);
            await context.SaveChangesAsync();

            var repository = new UserRepository(context, new Mock<ILogger<UserRepository>>().Object);
            var service = new UserService(repository);
            var controller = new UserController(service, new Mock<IHttpContextAccessor>().Object);
            var result = await controller.GetOne(55);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var data = okResult.Deserialize<UserResponse>();
            data.Should().NotBeNull().And.BeOfType<UserResponse>().Which.Id.Should().Be(user.Id);
        }

        [Fact]
        public async Task GetOneUserNotFound()
        {
            var context = DatabaseHelper.GetContext();
            var repository = new UserRepository(context, new Mock<ILogger<UserRepository>>().Object);
            var service = new UserService(repository);
            var controller = new UserController(service, new Mock<IHttpContextAccessor>().Object);
            var exception = await Assert.ThrowsAsync<NotFoundException>(() => controller.GetOne(15));
            exception.Should().BeOfType<NotFoundException>().Which.Message.Should().Be("user not found");
        }

        [Theory]
        [InlineData(null, "abcd123", "testname")]
        [InlineData("", "abcd123", "testname")]
        [InlineData("a", "abcd123", "testname")]
        [InlineData("login123", null, "testname")]
        [InlineData("login123", "", "testname")]
        [InlineData("login123", "a", "testname")]
        [InlineData("login123", "abcd123", null)]
        [InlineData("login123", "abcd123", "")]
        [InlineData("login123", "abcd123", "a")]
        public void CreateUserInvalidModel(string login, string name, string password)
        {
            var validator = new CreateUserValidator();
            var result = validator.Validate(new CreateUser() { Login = login, Name = name, Password = password });
            Assert.False(result.IsValid);
            result.Errors.Should().HaveCount(1);
        }

        [Fact]
        public async Task CreateUserLoginAlreadExists()
        {
            var createUser = new CreateUser() { Login = "testlogin", Name = "testname", Password = "abcd1234" };

            var context = DatabaseHelper.GetContext();

            context.User.Add(new User() { Id = 65, Login = createUser.Login, Name = createUser.Name, Password = createUser.Password });
            await context.SaveChangesAsync();

            var repository = new UserRepository(context, new Mock<ILogger<UserRepository>>().Object);
            var service = new UserService(repository);
            var controller = new UserController(service, new Mock<IHttpContextAccessor>().Object);
            var exception = await Assert.ThrowsAsync<InvalidUserException>(() => controller.Create(createUser));
            exception.Should().BeOfType<InvalidUserException>().Which.Message.Should().Be("login already exists");
        }

        [Fact]
        public async Task CreateUserFails()
        {
            var createUser = new CreateUser() { Login = "testlogin", Name = "testname", Password = "abcd1234" };

            var repository = new Mock<IUserRepository>();
            repository.Setup(x => x.Create(It.IsAny<User>())).ReturnsAsync(null as User);

            var service = new UserService(repository.Object);
            var controller = new UserController(service, new Mock<IHttpContextAccessor>().Object);
            var exception = await Assert.ThrowsAsync<UnexpectedUserException>(() => controller.Create(createUser));
            exception.Should().BeOfType<UnexpectedUserException>().Which.Message.Should().Be("cant create user");
        }

        [Fact]
        public async Task CreateUserSuccess()
        {
            var createUser = new CreateUser() { Login = "testlogin", Name = "testname", Password = "abcd1234" };

            var context = DatabaseHelper.GetContext();

            var repository = new UserRepository(context, new Mock<ILogger<UserRepository>>().Object);
            var service = new UserService(repository);
            var controller = new UserController(service, new Mock<IHttpContextAccessor>().Object);
            var result = await controller.Create(createUser);
            Assert.IsType<CreatedAtActionResult>(result.Result);

            context.User.Should().HaveCount(1);
        }

        [Theory]
        [InlineData(33, "a", "abcd123")]
        [InlineData(33, "login123", "a")]
        [InlineData(0, "login123", "abcd123")]
        public void UpdateUserInvalidModel(int id, string login, string name)
        {
            var validator = new UpdateUserValidator();
            var result = validator.Validate(new UpdateUser() { Id = id, Login = login, Name = name });
            Assert.False(result.IsValid);
            result.Errors.Should().HaveCount(1);
        }

        [Fact]
        public async Task UpdateUserNotFound()
        {
            var updateUser = new UpdateUser() { Id = 15, Name = "test1234", Login = "test1234" };

            var context = DatabaseHelper.GetContext();

            var repository = new UserRepository(context, new Mock<ILogger<UserRepository>>().Object);
            var service = new UserService(repository);
            var controller = new UserController(service, new Mock<IHttpContextAccessor>().Object);
            var exception = await Assert.ThrowsAsync<InvalidUserException>(() => controller.Update(updateUser));
            exception.Should().BeOfType<InvalidUserException>().Which.Message.Should().Be("user not found");
        }

        [Fact]
        public async Task UpdateUserLoginAlreadExists()
        {
            var updateUser = new UpdateUser() { Id = 15, Name = "test1234", Login = "test1234" };

            var context = DatabaseHelper.GetContext();
            context.User.Add(new User() { Id = 15, Login = "test987", Name = updateUser.Name, Password = "test123" });
            context.User.Add(new User() { Id = 10, Login = updateUser.Login, Name = updateUser.Name, Password = "test123" });
            await context.SaveChangesAsync();

            var repository = new UserRepository(context, new Mock<ILogger<UserRepository>>().Object);
            var service = new UserService(repository);
            var controller = new UserController(service, new Mock<IHttpContextAccessor>().Object);
            var exception = await Assert.ThrowsAsync<InvalidUserException>(() => controller.Update(updateUser));
            exception.Should().BeOfType<InvalidUserException>().Which.Message.Should().Be("login already exists");
        }

        [Fact]
        public async Task UpdateUserFail()
        {
            var updateUser = new UpdateUser() { Id = 15, Name = "test1234", Login = "test1234" };

            var repository = new Mock<IUserRepository>();
            repository.Setup(x => x.Update(It.IsAny<User>())).ReturnsAsync(false);
            repository.Setup(x => x.VerifyIfLoginAlreadyExists(15, updateUser.Login)).ReturnsAsync(false);
            repository.Setup(x => x.GetOne(updateUser.Id)).ReturnsAsync(new User() { Id = updateUser.Id, Login = updateUser.Login, Name = updateUser.Name, Password = "asdasda" });

            var service = new UserService(repository.Object);
            var controller = new UserController(service, new Mock<IHttpContextAccessor>().Object);
            var exception = await Assert.ThrowsAsync<UnexpectedUserException>(() => controller.Update(updateUser));
            exception.Should().BeOfType<UnexpectedUserException>().Which.Message.Should().Be("cant update user");
        }

        [Fact]
        public async Task UpdateUserSuccess()
        {
            var updateUser = new UpdateUser() { Id = 15, Name = "test1234", Login = "test1234" };

            var context = DatabaseHelper.GetContext();
            context.User.Add(new User() { Id = 15, Login = "test987", Name = "test654", Password = "test123" });
            await context.SaveChangesAsync();

            var repository = new UserRepository(context, new Mock<ILogger<UserRepository>>().Object);
            var service = new UserService(repository);
            var controller = new UserController(service, new Mock<IHttpContextAccessor>().Object);
            var result = await controller.Update(updateUser);
            Assert.IsType<OkResult>(result);
            context.User.First().Should().BeOfType<User>().Which.Name.Should().Be(updateUser.Name);
            context.User.First().Should().BeOfType<User>().Which.Login.Should().Be(updateUser.Login);

        }

        [Theory]
        [InlineData("")]
        [InlineData("p")]
        [InlineData(null)]
        public async Task UpdateUserPasswordInvalidModel(string password)
        {
            var validator = new UpdateUserPasswordValidator();
            var result = validator.Validate(new UpdateUserPassword() { Password = password });
            Assert.False(result.IsValid);
            result.Errors.Should().HaveCount(1);
        }

        [Fact]
        public async Task UpdateUserPasswordNotFound()
        {
            var updateUser = new UpdateUserPassword() { Password = "abcd1234" };

            var context = DatabaseHelper.GetContext();
            var httpContext = new Mock<IHttpContextAccessor>();
            httpContext.Setup(x => x.HttpContext.User.FindFirst(It.IsAny<Predicate<Claim>>())).Returns(new Claim(ClaimTypes.NameIdentifier, "15"));

            var repository = new UserRepository(context, new Mock<ILogger<UserRepository>>().Object);
            var service = new UserService(repository);
            var controller = new UserController(service, httpContext.Object);
            var exception = await Assert.ThrowsAsync<InvalidUserException>(() => controller.UpdatePassword(updateUser));
            exception.Should().BeOfType<InvalidUserException>().Which.Message.Should().Be("user not found");
        }

        [Fact]
        public async Task UpdateUserPasswordSuccess()
        {
            var password = "test1234";
            var passwordSha256 = 

            var updateUser = new UpdateUserPassword() { Password = password };

            var context = DatabaseHelper.GetContext();
            context.User.Add(new User() { Id = 15, Login = "test987", Name = "test654", Password = "test123" });
            await context.SaveChangesAsync();

            var repository = new UserRepository(context, new Mock<ILogger<UserRepository>>().Object);
            var service = new UserService(repository);
            var controller = new UserController(service, new Mock<IHttpContextAccessor>().Object);
            var result = await controller.Update(updateUser);
            Assert.IsType<OkResult>(result);
            context.User.First().Should().BeOfType<User>().Which.Name.Should().Be(updateUser.Name);
            context.User.First().Should().BeOfType<User>().Which.Login.Should().Be(updateUser.Login);
        }

        [Fact]
        public async Task UpdateUserPasswordFail()
        {
            var updateUser = new UpdateUserPassword() { Password = "abcd1234" };
            var userId = 15;

            var httpContext = new Mock<IHttpContextAccessor>();
            httpContext.Setup(x => x.HttpContext.User.FindFirst(It.IsAny<Predicate<Claim>>())).Returns(new Claim(ClaimTypes.NameIdentifier, "15"));

            var repository = new Mock<IUserRepository>();
            repository.Setup(x => x.Update(It.IsAny<User>())).ReturnsAsync(false);
            repository.Setup(x => x.GetOne(userId)).ReturnsAsync(new User() { Id = userId, Login = "abcd1234", Name = "abcd1234", Password = "asdasda" });

            var service = new UserService(repository.Object);
            var controller = new UserController(service, httpContext.Object);
            var exception = await Assert.ThrowsAsync<UnexpectedUserException>(() => controller.UpdatePassword(updateUser));
            exception.Should().BeOfType<UnexpectedUserException>().Which.Message.Should().Be("cant update user");
        }

        [Fact]
        public async Task DeleteUserNotFound()
        {

        }

        [Fact]
        public async Task DeleteUserSuccess()
        {

        }

        [Fact]
        public async Task DeleteUserFail()
        {

        }

    }
}
