using System;
using System.Collections;
using System.Collections.Generic;

namespace Domain
{
    public class AppUser
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Role {  get; set; }
        public ICollection<Tickets> Tickets { get; set; }
    }
    public static class Role
    {
        public const string Accountant = "Accountant";
        public const string User = "User";
    }
}
