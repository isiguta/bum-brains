﻿using Going.Plaid.Entity;

namespace BumBrains.Models.Configuration.Banking.Plaid.Link;

public class LinkResult
{
    public class t_metadata
    {
        public class t_institution
        {
            public string name { get; set; } = string.Empty;
            public string institution_id { get; set; } = string.Empty;
        }

        public string? link_session_id { get; set; }
        public string? status { get; set; }
        public string? request_id { get; set; }
        public t_institution? institution { get; set; }
    }


    public bool ok { get; set; }
    public string public_token { get; set; } = string.Empty;
    public PlaidError? error { get; set; }
    public t_metadata? metadata { get; set; }
};
