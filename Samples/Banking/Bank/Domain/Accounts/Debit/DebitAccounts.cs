// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Events.Accounts.Debit;

namespace Domain.Accounts.Debit;

[Route("/api/accounts/debit")]
public class DebitAccounts : ControllerBase
{
    readonly IEventLog _eventLog;

    public DebitAccounts(IEventLog eventLog) => _eventLog = eventLog;

    [HttpPost]
    public Task OpenDebitAccount([FromBody] OpenDebitAccount create) => _eventLog.Append(create.AccountId, new DebitAccountOpened(create.Details.Name, create.Details.Owner, create.Details.IncludeCard));
}
