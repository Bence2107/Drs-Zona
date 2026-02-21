using Context;
using Entities.Models.News;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces.News;

namespace Repositories.Implementations.News;

public class CommentVotesRepository(EfContext context) : ICommentVotesRepository
{
    private readonly DbSet<CommentVote> _votes = context.CommentVotes;

    public CommentVote? GetVote(Guid userId, Guid commentId) => 
        _votes.FirstOrDefault(v => v.UserId == userId && v.CommentId == commentId);

    public void Add(CommentVote vote) { _votes.Add(vote); context.SaveChanges(); }
    public void Update(CommentVote vote) { _votes.Update(vote); context.SaveChanges(); }
    public void Delete(CommentVote vote) { _votes.Remove(vote); context.SaveChanges(); }
}