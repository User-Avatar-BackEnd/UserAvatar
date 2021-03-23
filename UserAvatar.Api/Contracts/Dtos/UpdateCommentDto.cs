namespace UserAvatar.Api.Contracts.Dtos
{
    public class UpdateCommentDto
    {
        public int CommentId { get; set; }
        
        //todo: validator!
        public string Text { get; set; }
    }
}