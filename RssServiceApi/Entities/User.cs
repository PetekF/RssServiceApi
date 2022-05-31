﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace RssServiceApi.Entities
{
    [Index(nameof(Email), IsUnique = true)]
    [Index(nameof(Token), IsUnique = true)]
    public class User
    {
        public int Id { get; set; }

        public string Email { get; set; }
        public string Password { get; set; }
        public string? Token { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public bool? IsConfirmed { get; set; }

        public ICollection<UserFeed> Subscriptions { get; set; }

        public ICollection<Item> FavoriteItems { get; set; }

        public ICollection<Item> UnreadItems { get; set; }
    }
}
