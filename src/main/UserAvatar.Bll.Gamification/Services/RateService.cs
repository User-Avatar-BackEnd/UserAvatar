﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using UserAvatar.Bll.Gamification.Models;
using UserAvatar.Bll.Gamification.Services.Interfaces;
using UserAvatar.Bll.Infrastructure;
using UserAvatar.Dal.Entities;
using UserAvatar.Dal.Storages.Interfaces;

namespace UserAvatar.Bll.Gamification.Services;

public sealed class RateService : IRateService
{
    private readonly IMapper _mapper;
    private readonly IRankService _rankService;
    private readonly IUserStorage _userStorage;

    public RateService(IUserStorage userStorage, IRankService rankService, IMapper mapper)
    {
        _userStorage = userStorage;
        _rankService = rankService;
        _mapper = mapper;
    }

    public async Task<Result<FullRateModel>> GetTopRateAsync(int userId)
    {
        var users = await _userStorage.GetUsersRateAsync();

        var firstTen = users.Take(10).ToList();
        var underTopUsers = new List<RateModel>();

        if (!firstTen.Any(x => x.Id == userId))
        {
            var currentUser = users.First(x => x.Id == userId);
            var currentUserIndex = users.IndexOf(currentUser);

            if (currentUserIndex == 10)
            {
                underTopUsers.Add(_mapper.Map<User, RateModel>(currentUser));
                underTopUsers.Add(_mapper.Map<User, RateModel>(users[currentUserIndex + 1]));
            }
            else if (currentUserIndex == users.Count() - 1)
            {
                underTopUsers.Add(_mapper.Map<User, RateModel>(users[currentUserIndex - 1]));
                underTopUsers.Add(_mapper.Map<User, RateModel>(currentUser));
            }
            else
            {
                underTopUsers.Add(_mapper.Map<User, RateModel>(users[currentUserIndex - 1]));
                underTopUsers.Add(_mapper.Map<User, RateModel>(currentUser));
                underTopUsers.Add(_mapper.Map<User, RateModel>(users[currentUserIndex + 1]));
            }
        }

        var topUsers = _mapper.Map<List<RateModel>>(firstTen);

        if (underTopUsers.Count >= 2)
        {
            if (underTopUsers.Count == 2)
            {
                var currentUser0 = underTopUsers.First(x => x.Id == userId);
                var currentUserIndex0 = underTopUsers.IndexOf(currentUser0);

                currentUser0.IsCurrentPlayer = true;

                underTopUsers[currentUserIndex0] = currentUser0;
            }
            else
            {
                underTopUsers[1].IsCurrentPlayer = true;
            }

            foreach (var user in underTopUsers)
            {
                var rank = await _rankService.GetAllRanksDataAsync(user.Score);
                user.Rank = rank.Name;
            }

            foreach (var user in underTopUsers)
            {
                var position = user.RatePosition = users
                    .IndexOf(users
                        .First(x => x.Id == user.Id));

                user.RatePosition = ++position;
            }
        }

        if (topUsers.Any(x => x.Id == userId))
        {
            var currentUser1 = topUsers.First(x => x.Id == userId);
            var currentUserIndex1 = topUsers.IndexOf(currentUser1);

            currentUser1.IsCurrentPlayer = true;

            topUsers[currentUserIndex1] = currentUser1;
        }

        foreach (var user in topUsers)
        {
            var rank = await _rankService.GetAllRanksDataAsync(user.Score);
            user.Rank = rank.Name;
        }

        foreach (var user in topUsers)
        {
            var position = user.RatePosition = users
                .IndexOf(users
                    .First(x => x.Id == user.Id));

            user.RatePosition = ++position;
        }

        var rate = new FullRateModel { TopUsers = topUsers, UnderTopUsers = underTopUsers };

        return new Result<FullRateModel>(rate);
    }
}
