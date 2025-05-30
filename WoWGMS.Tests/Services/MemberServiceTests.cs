using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using WowGMSBackend.Model;
using WowGMSBackend.Interfaces;
using WowGMSBackend.Service;
using WowGMSBackend.Repository;
using System.Security.Claims;

namespace WoWGMS.Tests.Services
{
    public class MembersServiceTests
    {
        private readonly Mock<IMemberRepo> _mockMemberRepo;
        private readonly MemberService _memberService;

        public MembersServiceTests()
        {
            _mockMemberRepo = new Mock<IMemberRepo>();
            _memberService = new MemberService(_mockMemberRepo.Object);
        }

        [Fact]
        public void ValidateLogin_ShouldReturnMember_WhenCredentialsAreCorrect()
        {
            // Arrange
            var member = new Member { Name = "TestUser", Password = "TestPassword" };
            var memberList = new List<Member> { member };
            _mockMemberRepo.Setup(repo => repo.GetMembers()).Returns(memberList);

            // Act
            var result = _memberService.ValidateLogin("TestUser", "TestPassword");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(member.Name, result.Name);
        }

        [Fact]
        public void ValidateLogin_ShouldReturnNull_WhenCredentialsAreIncorrect()
        {
            // Arrange
            var member = new Member { Name = "TestUser", Password = "TestPassword" };
            var memberList = new List<Member> { member };
            _mockMemberRepo.Setup(repo => repo.GetMembers()).Returns(memberList);

            // Act
            var result = _memberService.ValidateLogin("WrongUser", "WrongPassword");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void AddMember_ShouldThrow_WhenNameIsEmpty()
        {
            // Arrange
            var member = new Member { Name = " ", Password = "x", Rank = Rank.Trialist };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _memberService.AddMember(member));
        }

        [Fact]
        public void DeleteMember_ShouldThrow_WhenRankIsOfficer()
        {
            // Arrange
            var officer = new Member { MemberId = 1, Name = "Officer", Rank = Rank.Officer };
            _mockMemberRepo.Setup(r => r.GetMember(1)).Returns(officer);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _memberService.DeleteMember(1));
        }

        [Fact]
        public void ChangeMemberRank_ShouldUpdateRank_WhenActingMemberIsOfficer()
        {
            // Arrange
            var officer = new Member { MemberId = 1, Rank = Rank.Officer };
            var target = new Member { MemberId = 2, Rank = Rank.Trialist };
            _mockMemberRepo.Setup(r => r.GetMember(1)).Returns(officer);
            _mockMemberRepo.Setup(r => r.GetMember(2)).Returns(target);
            _mockMemberRepo.Setup(r => r.UpdateMember(2, It.IsAny<Member>())).Returns((int id, Member m) => m);

            // Act
            var updated = _memberService.ChangeMemberRank(1, 2, Rank.Officer);

            // Assert
            Assert.Equal(Rank.Officer, updated.Rank);
        }

        [Fact]
        public void ChangeMemberRank_ShouldThrow_WhenActingMemberIsNotOfficer()
        {
            // Arrange
            var nonOfficer = new Member { MemberId = 1, Rank = Rank.Raider };
            var target = new Member { MemberId = 2, Rank = Rank.Trialist };
            _mockMemberRepo.Setup(r => r.GetMember(1)).Returns(nonOfficer);
            _mockMemberRepo.Setup(r => r.GetMember(2)).Returns(target);

            // Act & Assert
            Assert.Throws<UnauthorizedAccessException>(() => _memberService.ChangeMemberRank(1, 2, Rank.Officer));
        }

        [Fact]
        public void AddMember_ShouldCallRepo_WhenMemberIsValid()
        {
            // Arrange
            var member = new Member { Name = "NewUser", Password = "pass", Rank = Rank.Trialist };
            _mockMemberRepo.Setup(r => r.AddMember(member)).Returns(member);

            // Act
            var result = _memberService.AddMember(member);

            // Assert
            Assert.Equal(member, result);
        }

        [Fact]
        public void GetMembers_ShouldReturnAllMembers()
        {
            // Arrange
            var members = new List<Member> { new Member { Name = "A" }, new Member { Name = "B" } };
            _mockMemberRepo.Setup(r => r.GetMembers()).Returns(members);

            // Act
            var result = _memberService.GetMembers();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, m => m.Name == "A");
        }

        [Fact]
        public void GetMember_ShouldReturnMemberById()
        {
            // Arrange
            var member = new Member { MemberId = 42, Name = "TestUser" };
            _mockMemberRepo.Setup(r => r.GetMember(42)).Returns(member);

            // Act
            var result = _memberService.GetMember(42);

            // Assert
            Assert.Equal(member, result);
        }

        [Fact]
        public void UpdateMember_ShouldReturnUpdatedMember()
        {
            // Arrange
            var updated = new Member { MemberId = 1, Name = "Updated", Rank = Rank.Trialist };
            _mockMemberRepo.Setup(r => r.UpdateMember(1, updated)).Returns(updated);

            // Act
            var result = _memberService.UpdateMember(1, updated);

            // Assert
            Assert.Equal("Updated", result.Name);
        }

        [Fact]
        public void ChangeMemberRank_ShouldReturnNull_WhenTargetDoesNotExist()
        {
            // Arrange
            var officer = new Member { MemberId = 1, Rank = Rank.Officer };
            _mockMemberRepo.Setup(r => r.GetMember(1)).Returns(officer);
            _mockMemberRepo.Setup(r => r.GetMember(2)).Returns((Member)null);

            // Act
            var result = _memberService.ChangeMemberRank(1, 2, Rank.Officer);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void ChangeMemberRank_ShouldReturnNull_WhenActingDoesNotExist()
        {
            // Arrange
            _mockMemberRepo.Setup(r => r.GetMember(1)).Returns((Member)null);
            _mockMemberRepo.Setup(r => r.GetMember(2)).Returns(new Member { Rank = Rank.Trialist });

            // Act
            var result = _memberService.ChangeMemberRank(1, 2, Rank.Raider);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetLoggedInMemberId_ShouldReturnId_WhenClaimExists()
        {
            // Arrange
            var claims = new List<Claim> { new Claim("MemberId", "42") };
            var identity = new ClaimsIdentity(claims);
            var user = new ClaimsPrincipal(identity);

            // Act
            var result = _memberService.GetLoggedInMemberId(user);

            // Assert
            Assert.Equal(42, result);
        }

        [Fact]
        public void GetLoggedInMemberId_ShouldReturnNull_WhenClaimMissingOrInvalid()
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity());

            // Act
            var result = _memberService.GetLoggedInMemberId(user);

            // Assert
            Assert.Null(result);
        }
    }
}
