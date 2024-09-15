namespace UserAvatar.Bll.Infrastructure;

public static class EventType
{
    public const string Registration = "Registration";

    public const string Login = "Login";

    public const string Logout = "Logout";

    public const string CreateBoard = "Create board";

    public const string SendInvite = "Send invite";

    public const string CreateCardOnOwnBoard = "Create card on own board";

    public const string CreateCardOnAlienBoard = "Create card on alien board";

    public const string ChangeCardStatusOnOwnBoard = "Change card status on own board";

    public const string ChangeCardStatusOnAlienBoard = "Change card status on alien board";

    public const string ChangeUserBalanceByAdmin = "Change user balance by admin";
}
