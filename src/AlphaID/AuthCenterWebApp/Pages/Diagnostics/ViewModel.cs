// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.


using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using System.Text;
using System.Text.Json;

namespace AuthCenterWebApp.Pages.Diagnostics;

public class ViewModel
{
    public ViewModel(AuthenticateResult result)
    {
        this.AuthenticateResult = result;

        if (result.Properties.Items.ContainsKey("client_list"))
        {
            var encoded = result.Properties.Items["client_list"];
            var bytes = Base64Url.Decode(encoded);
            var value = Encoding.UTF8.GetString(bytes);

            this.Clients = JsonSerializer.Deserialize<string[]>(value);
        }
    }

    public AuthenticateResult AuthenticateResult { get; }
    public IEnumerable<string> Clients { get; } = new List<string>();
}