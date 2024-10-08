﻿namespace UserAvatar.Bll.Infrastructure;

public readonly struct Result<T>
{
    public Result(T value)
    {
        Code = ResultCode.Success;
        Value = value;
        EventType = null;
    }

    public Result(T value, string eventType)
    {
        Code = ResultCode.Success;
        Value = value;
        EventType = eventType;
    }

    public Result(int code)
    {
        Code = code;
        Value = default;
        EventType = null;
    }

    public T Value { get; }

    public int Code { get; }

    public string EventType { get; }
}
