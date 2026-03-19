using Context;
using Entities.Models.News;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces.News;

namespace Repositories.Implementations.News;

public class CommentVotesRepository(EfContext context) : ICommentVotesRepository 
{
    private readonly DbSet<CommentVote> _votes = context.CommentVotes;

    public async Task<CommentVote?> GetVoteForACommment(Guid? userId, Guid commentId) => await _votes
            .FirstOrDefaultAsync(v => v.UserId == userId && v.CommentId == commentId);
    
    public async Task Create(CommentVote vote)
    {
        _votes.Add(vote); 
        await context.SaveChangesAsync();
    }

    public async Task Update(CommentVote vote)
    {
        _votes.Update(vote); 
        await context.SaveChangesAsync();
    }

    public async Task Delete(CommentVote vote)
    {
        _votes.Remove(vote); 
        await context.SaveChangesAsync();
    }
}