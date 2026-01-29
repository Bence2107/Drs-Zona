using Entities.Models;
using Entities.Models.News;
using Moq;
using Repositories.Interfaces;
using Repositories.Interfaces.News;
using Services.Implementations;
using Xunit;
using FluentAssertions;

namespace Tests;

public class CommentTests
{
    [Fact]
    public void US_04_AC_01_GetCommentReplies_ShouldReturnAllRepliesForComment()
    {
        var mockRepo = new Mock<ICommentsRepository>();
        var mockUserRepo = new Mock<IUsersRepository>();
        var service = new CommentService(mockRepo.Object, mockUserRepo.Object);
    
        var targetCommentId = Guid.NewGuid();
        var fakeReplies = new List<Comment>
        {
            new()
            { Id = Guid.NewGuid(), Content = "First Reply", ReplyToCommentId = targetCommentId, User = new User
            {
                Id = Guid.NewGuid(),
                Username = "User1",
                Email = "User1@gmail.com",
                Password = "Password1!",
                Role =  "user",
                HasAvatar =  true,
                Created =  DateTime.Now
            } },
            new()
            { Id = Guid.NewGuid(), Content = "Second reply", ReplyToCommentId = targetCommentId, User = new User
            {
                Id = Guid.NewGuid(),
                Username = "User2",
                Email = "User2@gmail.com",
                Password = "Password2!$$",
                Role =  "user",
                HasAvatar =  false,
                Created =  DateTime.Now
            } }
        };

        mockRepo.Setup(r => r.GetRepliesToAComment(targetCommentId)).Returns(fakeReplies);

        var result = service.GetCommentReplies(targetCommentId);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2); 
        result.Value.Should().AllSatisfy(c => c.ReplyToCommentId.Should().Be(targetCommentId));
    }
}