﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMessenger.MApplication.DTO.AuthDTOs
{
    public class TokensDTO
    {
        public string? accessToken { get; set; }
        public string? refreshToken { get; set; }
    }
}
