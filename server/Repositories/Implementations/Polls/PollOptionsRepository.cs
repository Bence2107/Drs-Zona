using Context;
using Entities.Models.Polls;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces.Polls;

namespace Repositories.Implementations.Polls;

public class PollOptionsRepository(EfContext context) : IPollOptionsRepository
{
    private readonly DbSet<PollOption> _pollOptions = context.PollOptions;
    
    public PollOption? GetPollOptionById(Guid id) => _pollOptions.FirstOrDefault(x => x.Id == id);
    
    public List<PollOption> GetAllPollOptions() =>_pollOptions.ToList();
    
    public void Create(PollOption pollOption)
    {
        _pollOptions.Add(pollOption);
        context.SaveChanges();
    }

    public void Update(PollOption pollOption)
    {
        _pollOptions.Update(pollOption);
        context.SaveChanges();
    }

    public void Delete(Guid id)
    {
        var pollOption =  GetPollOptionById(id);
        if(pollOption == null) return;
        
        _pollOptions.Remove(pollOption);
        context.SaveChanges();
    }

    public PollOption? GetByIdWithPoll(Guid id) =>_pollOptions
        .Include(po => po.Poll)
        .FirstOrDefault(po => po.Id == id);

    public List<PollOption> GetByPollId(Guid pollId) => _pollOptions
        .Where(poll => poll.PollId == pollId)
        .ToList();
    
    public bool CheckIfIdExists(Guid id) => _pollOptions.Any(po => po.Id == id);
}