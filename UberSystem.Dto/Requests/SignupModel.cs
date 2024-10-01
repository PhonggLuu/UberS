﻿using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using UberSystem.Domain.Enums;

namespace UberSytem.Dto.Requests
{
    public class SignupModel
    {
        public string? UserName { get; set; }
        
        [DataType(DataType.EmailAddress)]
        public required string Email { get; set; }
        
        [DataType(DataType.Password)]
        public required string Password { get; set; }

		public UserRole Role { get; set; }
	}
}
