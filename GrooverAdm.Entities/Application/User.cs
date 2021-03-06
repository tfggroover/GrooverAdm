﻿using GrooverAdm.Entities.Spotify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrooverAdm.Entities.Application
{
    public class User :IApplicationEntity
    {
        public User() {
        }
        public User(UserInfo userInfo, string currentToken, int expiresIn, DateTime dateTime)
        {
            DisplayName = userInfo.Display_name;
            Id = userInfo.Id;
            Email = userInfo.Email;
            CurrentToken = currentToken;
            ExpiresIn = expiresIn;
            TokenEmissionTime = dateTime;
        }
        public int Born { get; set; }
        public string DisplayName { get; set; }
        public string Id { get; set; }
        public string Email { get; set; }
        public bool Admin { get; set; }
        public string CurrentToken { get; set; }
        public string RefreshToken { get; set; }
        /// <summary>
        /// Expiration time in seconds
        /// </summary>
        public int ExpiresIn { get; set; }
        public DateTime TokenEmissionTime { get; set; }
    }

    public class ListableUser: IApplicationEntity
    {
        public ListableUser() { }
        public ListableUser(User user)
        {
            this.Born = user.Born;
            this.DisplayName = user.DisplayName;
            this.Id = user.Id;
            this.Email = user.Email;
            this.Admin = user.Admin;
        }
        public int Born { get; set; }
        public string DisplayName { get; set; }
        public string Id { get; set; }
        public string Email { get; set; }
        public bool Admin { get; set; }
    }
}
