﻿namespace GameWeb.Application.Common.Interfaces;

public interface IUser
{
    string? Id { get; }
    List<string>? Roles { get; }
    int? CharacterId { get; }
}
