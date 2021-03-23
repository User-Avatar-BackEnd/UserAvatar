namespace UserAvatar.Api.Contracts.Requests
{
    public class UpdateCommentRequest
    {
        public int CommentId { get; set; }
        
        //todo: validator!
        public string Text { get; set; }
    }
}