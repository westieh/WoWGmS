namespace WoWGMS.Tests;
using WoWGMS.Controllers;
using WowGMSBackend.Model;
using WowGMSBackend.Service;
using WowGMSBackend.DBContext;
using Xunit;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WowGMSBackend.Interfaces;

public class ControllerUnitTest
{
    private readonly Mock<IDBService<Member>> _mockService;
    private readonly MemberController _controller;

    public ControllerUnitTest()
    {
        _mockService = new Mock<IDBService<Member>>();
        _controller = new MemberController(_mockService.Object);
    }

    [Fact]
    public async Task GetAllObjects_ReturnsView_WithMembers()
    {
        // Arrange
        var members = new List<Member> {
            new Member { MemberId = 1, Name = "Alice", Rank = Rank.Officer }
        };
        _mockService.Setup(s => s.GetAllObjectsAsync())
                    .ReturnsAsync(members);

        // Act
        var result = await _controller.GetAllObjects();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(members, viewResult.Model);
    }

    [Fact]
    public async Task CreateMember_RedirectsToIndex()
    {
        // Arrange
        var newMember = new Member { MemberId = 2, Name = "Bob", Rank = Rank.Raider };

        // Act
        var result = await _controller.CreateMember(newMember);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        _mockService.Verify(s => s.AddObjectAsync(newMember), Times.Once);
    }

    [Fact]
    public async Task Delete_ReturnsErrorView_WhenMemberNotFound()
    {
        // Arrange
        _mockService.Setup(s => s.GetObjectByIdAsync(99))
                    .ReturnsAsync((Member)null!);

        // Act
        var result = await _controller.Delete(99);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("Error", viewResult.ViewName);
    }

    [Fact]
    public async Task Delete_ReturnsView_WhenMemberExists()
    {
        // Arrange
        var member = new Member { MemberId = 1, Name = "Test" };
        _mockService.Setup(s => s.GetObjectByIdAsync(1))
                    .ReturnsAsync(member);

        // Act
        var result = await _controller.Delete(1);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(member, viewResult.Model);
    }

    [Fact]
    public async Task ConfirmDelete_DeletesAndRedirects_WhenMemberExists()
    {
        // Arrange
        var member = new Member { MemberId = 1, Name = "Test" };
        _mockService.Setup(s => s.GetObjectByIdAsync(1))
                    .ReturnsAsync(member);

        // Act
        var result = await _controller.ConfirmDelete(1);

        // Assert
        _mockService.Verify(s => s.DeleteObjectAsync(member), Times.Once);
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
    }

    [Fact]
    public async Task ConfirmDelete_ReturnsErrorView_WhenMemberMissing()
    {
        // Arrange
        _mockService.Setup(s => s.GetObjectByIdAsync(5))
                    .ReturnsAsync((Member)null!);

        // Act
        var result = await _controller.ConfirmDelete(5);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("Error", viewResult.ViewName);
    }
}
