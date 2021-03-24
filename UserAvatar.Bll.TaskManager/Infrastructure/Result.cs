namespace UserAvatar.Bll.TaskManager.Infrastructure
{
    public readonly struct Result<T>
    {
        public T Value { get; }

        public int Code { get; }

        public Result(T value)
        {
            Code = ResultCode.Success;
            Value = value;
        }

        public Result(int code)
        {
            Code = code;
            Value = default(T);
        }
    }
}
