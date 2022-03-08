﻿using System;

namespace HelloStéphane.BO
{
    public class User
    {
        public enum ProfileOptions
        {
            Admin,
            Visitor
        }

        public DateTime? CreatedAt { get; set; }

        public int Id { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Fullname { get; set; }

        public ProfileOptions? Profile { get; set; }

        public bool? Status { get; set; }

        public byte[] Picture { get; set; }

        public User()
        {
        }

        public User(int id, string username, string password,
            string fullname, ProfileOptions? profile, bool? status, byte[] picture, DateTime? created)
        {
            Id = id;
            Username = username;
            Password = password;
            Fullname = fullname;
            Profile = profile;
            Status = status;
            Picture = picture;
            CreatedAt = created;
        }

        public override bool Equals(object obj)
        {
            return obj is User user &&
                   Id == user.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
