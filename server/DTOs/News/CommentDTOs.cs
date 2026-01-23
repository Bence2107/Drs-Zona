using System.ComponentModel.DataAnnotations;

namespace DTOs.News;

public record CommentCreateDto(
    [Required(ErrorMessage = "A cikk azonosítója kötelező")] 
    [Range(1, int.MaxValue, ErrorMessage = "Érvénytelen cikk azonosító")] 
    int ArticleId,
    int? ReplyToCommentId,
    [Required(ErrorMessage = "A komment tartalma nem lehet üres")]
    [StringLength(1000, MinimumLength = 1, ErrorMessage = "A komment hossza 1 és 1000 karakter között lehet")]
    string Content
) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(Content))
            yield return new ValidationResult("A komment nem tartalmazhat csak szóközt!", [nameof(Content)]);
    }
}

public record CommentContentUpdateDto(
    [Required(ErrorMessage = "Az azonosító kötelező")]
    int Id,
    [Required(ErrorMessage = "A cikk azonosítója kötelező")]
    int ArticleId,
    [Required(ErrorMessage = "A tartalom nem lehet üres")]
    [StringLength(1000, MinimumLength = 1, ErrorMessage = "A tartalom hossza 1 és 1000 karakter között lehet")]
    string Content
);

public record CommentUpdateVoteDto(
    int Id,
    int ArticleId,
    int UpVotes,
    int DownVotes
);

public record CommentDetailDto(
    int Id,
    int UserId,
    int? ReplyToCommentId,
    string Username,
    string Content,
    int UpVotes,
    int DownVotes,
    DateTime DateCreated,
    DateTime DateUpdated
);