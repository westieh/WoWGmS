using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using WowGMSBackend.Model;
using WowGMSBackend.Interfaces;
using WowGMSBackend.Service;
using WowGMSBackend.Repository;
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
           
            // ActD
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
            var member = new Member { Name = " ", Password = "x", Rank = Rank.Trialist };

            Assert.Throws<ArgumentException>(() => _memberService.AddMember(member));
        }
        [Fact]
        public void DeleteMember_ShouldThrow_WhenRankIsOfficer()
        {
            var officer = new Member { MemberId = 1, Name = "Officer", Rank = Rank.Officer };

            _mockMemberRepo.Setup(r => r.GetMember(1)).Returns(officer);

            Assert.Throws<InvalidOperationException>(() => _memberService.DeleteMember(1));
        }
        [Fact]
        public void ChangeMemberRank_ShouldUpdateRank_WhenActingMemberIsOfficer()
        {
            var officer = new Member { MemberId = 1, Rank = Rank.Officer };
            var target = new Member { MemberId = 2, Rank = Rank.Trialist };

            _mockMemberRepo.Setup(r => r.GetMember(1)).Returns(officer);
            _mockMemberRepo.Setup(r => r.GetMember(2)).Returns(target);
            _mockMemberRepo.Setup(r => r.UpdateMember(2, It.IsAny<Member>())).Returns((int id, Member m) => m);

            var updated = _memberService.ChangeMemberRank(1, 2, Rank.Officer);

            Assert.Equal(Rank.Officer, updated.Rank);
        }
        [Fact]
        public void ChangeMemberRank_ShouldThrow_WhenActingMemberIsNotOfficer()
        {
            var nonOfficer = new Member { MemberId = 1, Rank = Rank.Raider };
            var target = new Member { MemberId = 2, Rank = Rank.Trialist };

            _mockMemberRepo.Setup(r => r.GetMember(1)).Returns(nonOfficer);
            _mockMemberRepo.Setup(r => r.GetMember(2)).Returns(target);

            Assert.Throws<UnauthorizedAccessException>(() => _memberService.ChangeMemberRank(1, 2, Rank.Officer));
        }
        [Fact]
        public void AddMember_ShouldCallRepo_WhenMemberIsValid()
        {
            var member = new Member { Name = "NewUser", Password = "pass", Rank = Rank.Trialist };
            _mockMemberRepo.Setup(r => r.AddMember(member)).Returns(member);

            var result = _memberService.AddMember(member);

            Assert.Equal(member, result);
        }
        [Fact]
        public void GetMembers_ShouldReturnAllMembers()
        {
            var members = new List<Member> { new Member { Name = "A" }, new Member { Name = "B" } };
            _mockMemberRepo.Setup(r => r.GetMembers()).Returns(members);

            var result = _memberService.GetMembers();

            Assert.Equal(2, result.Count);
            Assert.Contains(result, m => m.Name == "A");
        }
        [Fact]
        public void GetMember_ShouldReturnMemberById()
        {
            var member = new Member { MemberId = 42, Name = "TestUser" };
            _mockMemberRepo.Setup(r => r.GetMember(42)).Returns(member);

            var result = _memberService.GetMember(42);

            Assert.Equal(member, result);
        }
        [Fact]
        public void UpdateMember_ShouldReturnUpdatedMember()
        {
            var updated = new Member { MemberId = 1, Name = "Updated", Rank = Rank.Trialist };

            _mockMemberRepo.Setup(r => r.UpdateMember(1, updated)).Returns(updated);

            var result = _memberService.UpdateMember(1, updated);

            Assert.Equal("Updated", result.Name);
        }
        [Fact]
        public void ChangeMemberRank_ShouldReturnNull_WhenTargetDoesNotExist()
        {
            var officer = new Member { MemberId = 1, Rank = Rank.Officer };

            _mockMemberRepo.Setup(r => r.GetMember(1)).Returns(officer); // acting member
            _mockMemberRepo.Setup(r => r.GetMember(2)).Returns((Member)null); // target not found

            var result = _memberService.ChangeMemberRank(1, 2, Rank.Officer);

            Assert.Null(result);
        }
        [Fact]
        public void ChangeMemberRank_ShouldReturnNull_WhenActingDoesNotExist()
        {
            _mockMemberRepo.Setup(r => r.GetMember(1)).Returns((Member)null);
            _mockMemberRepo.Setup(r => r.GetMember(2)).Returns(new Member { Rank = Rank.Trialist });

            var result = _memberService.ChangeMemberRank(1, 2, Rank.Raider);

            Assert.Null(result);
        }


    }
}
