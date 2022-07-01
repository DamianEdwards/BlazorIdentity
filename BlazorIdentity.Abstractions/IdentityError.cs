﻿namespace BlazorIdentity.Abstractions;

public class IdentityError
{
    public string Code { get; set; } = default!;

    public string Description { get; set; } = default!;
}