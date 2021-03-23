namespace UserAvatar.Bll.TaskManager.Infrastructure
{
    public static class ResultCode
    {
        public const int Success = 200;
        public const int Forbidden = 403;
        public const int NotFound = 404;

        //Authorization
        public const int InvalidEmail = 11;
        public const int InvalidPassword = 12;
        public const int LoginAlreadyExist = 13;
        public const int EmailAlreadyExist = 14;

        //Limitation
        public const int MaxBoardCount = 21;
        public const int MaxColumnCount = 22;
        public const int MaxTaskCount = 23;

    }
}
