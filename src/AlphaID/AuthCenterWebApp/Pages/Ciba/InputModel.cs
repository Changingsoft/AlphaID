// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

namespace AuthCenterWebApp.Pages.Ciba;

public class InputModel
{
    public string Button { get; set; } = default!;
    public IEnumerable<string> ScopesConsented { get; set; } = default!;
    public string Id { get; set; } = default!;
    public string Description { get; set; } = default!;
}