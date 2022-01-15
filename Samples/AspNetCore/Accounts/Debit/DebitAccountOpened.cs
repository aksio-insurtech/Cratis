// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Compliance;
using Cratis.Events;

namespace Sample.Accounts.Debit
{
    [EventType("8a109d92-aa6a-4aee-8852-f8f2528da1fc")]
    public record DebitAccountOpened(string Name, [PII] Person Owner);
}