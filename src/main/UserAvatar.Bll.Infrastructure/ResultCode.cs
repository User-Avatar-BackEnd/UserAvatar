﻿namespace UserAvatar.Bll.Infrastructure;

public static class ResultCode
{
    public const int Success = 200;
    public const int Forbidden = 403;
    public const int BadRequest = 400;
    public const int NotFound = 404;

    // Authorization
    public const int InvalidEmail = 11;
    public const int InvalidPassword = 12;
    public const int LoginAlreadyExist = 13;
    public const int EmailAlreadyExist = 14;


    // Limitation
    public const int MaxBoardCount = 21;
    public const int MaxColumnCount = 22;
    public const int MaxCardCount = 23;

    // Internal
    public const int UserNotFound = -1;

    // Personal account
    public const int SamePasswordAsOld = 33;
    public const int SameLoginAsCurrent = 34;
}
