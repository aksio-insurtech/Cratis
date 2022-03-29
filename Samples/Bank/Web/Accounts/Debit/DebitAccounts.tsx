// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { useMemo, useState, ReactElement, useEffect } from 'react';
import { useDialog, DialogResult } from '@aksio/cratis-applications-frontend/dialogs';
import { CreateAccountDialog, CreateAccountDialogResult } from './CreateAccountDialog';
import { Guid } from '@aksio/cratis-fundamentals';

import {
    CommandBar,
    IColumn,
    ICommandBarItemProps,
    DetailsList,
    Selection,
    SelectionMode,
    Stack,
    SearchBox,
    TextField
} from '@fluentui/react';
import { AmountDialog, AmountDialogInput, AmountDialogResult } from './AmountDialog';
import { OpenDebitAccount } from 'API/accounts/debit/OpenDebitAccount';
import { DepositToAccount } from 'API/accounts/debit/DepositToAccount';
import { WithdrawFromAccount } from 'API/accounts/debit/WithdrawFromAccount';
import { AllAccounts } from 'API/accounts/debit/AllAccounts';
import { StartingWith } from 'API/accounts/debit/StartingWith';
import { LatestTransactions } from 'API/accounts/debit/LatestTransactions';
import { DebitAccount } from 'API/accounts/debit/DebitAccount';
import { CommandTracker, CommandTrackerContext, useCommandTracker } from '@aksio/cratis-applications-frontend/commands';
import { SetDebitAccountName } from 'API/accounts/debit/SetDebitAccountName';

export const DebitAccounts = () => {
    const [accounts] = AllAccounts.use();
    const [openDebitAccount, setOpenDebitAccountValues] = OpenDebitAccount.use({ owner: 'edd60145-a6df-493f-b48d-35ffdaaefc4c' });


    const columns: IColumn[] = [
        {
            key: 'name',
            name: 'Name',
            fieldName: 'name',
            minWidth: 200,
            onRender: (item: DebitAccount, index?: number) => {
                return (
                    <TextField defaultValue={item.name} onKeyUp={(event) => {
                        if (event.code == 'Enter' && item.name != event.currentTarget.value) {
                            const command = new SetDebitAccountName();
                            command.accountId = item.id;
                            command.name = event.currentTarget.value;
                            command.execute();
                        }
                    }} />
                );
            }
        },
        {
            key: 'balance',
            name: 'Balance',
            fieldName: 'balance',
            minWidth: 200
        }
    ];


    const [latestTransactionsForAccount, queryLatestTransactionsForAccount] = LatestTransactions.use();
    const [accountsStartingWith, queryAccountsStartingWith] = StartingWith.use({ filter: '' });
    const [searching, setSearching] = useState<boolean>(false);
    const [selectedItem, setSelectedItem] = useState<any>(undefined);
    const [showCreateAccount, createAccountDialogProps] = useDialog<any, CreateAccountDialogResult>(async (result, output?) => {
        if (result === DialogResult.Success && output) {
            setOpenDebitAccountValues({
                accountId: Guid.create().toString(),
                name: output.name
            });
            await openDebitAccount.execute();
        }
    });


    const [showDepositAmountDialog, depositAmountDialogProps] = useDialog<AmountDialogInput, AmountDialogResult>(async (result, output?) => {
        if (result === DialogResult.Success && output && selectedItem) {
            const command = new DepositToAccount();
            command.accountId = selectedItem.id;
            command.amount = output.amount;
            await command.execute();
        }
    });

    const [showWithdrawAmountDialog, withdrawAmountDialogProps] = useDialog<AmountDialogInput, AmountDialogResult>(async (result, output?) => {
        if (result === DialogResult.Success && output && selectedItem) {
            const command = new WithdrawFromAccount();
            command.accountId = selectedItem.id;
            command.amount = output.amount;
            await command.execute();
        }
    });

    const searchFor = (filter: string) => {
        if (filter && filter !== '') {
            setSearching(true);
        } else {
            setSearching(false);
        }
        queryAccountsStartingWith({ filter });
    };

    const commandBarItems: ICommandBarItemProps[] = [
        {
            key: 'add',
            name: 'Add Debit Account',
            iconProps: { iconName: 'Add' },
            onClick: showCreateAccount
        },
        {
            key: 'search',
            onRender: (props, defaultRenderer) => {
                return (
                    <div style={{ position: 'relative', top: '6px', width: '400px' }}>
                        <SearchBox
                            placeholder="Accounts starting with"
                            onClear={() => searchFor('')}
                            onChange={(ev, newValue) => searchFor(newValue || '')} />
                    </div>
                );
            }
        }
    ];


    if (selectedItem) {
        commandBarItems.push(
            {
                key: 'deposit',
                name: 'Deposit',
                iconProps: { iconName: 'Money' },
                onClick: () => showDepositAmountDialog({ okTitle: 'Deposit' })
            }
        );

        commandBarItems.push(
            {
                key: 'withdraw',
                name: 'Withdraw',
                iconProps: { iconName: 'Money' },
                onClick: () => showWithdrawAmountDialog({ okTitle: 'Withdraw' })
            }
        );
    }

    const selection = useMemo(
        () => new Selection({
            selectionMode: SelectionMode.single,
            onSelectionChanged: () => {
                const selected = selection.getSelection();
                if (selected.length === 1) {
                    const account = selected[0] as DebitAccount;
                    setSelectedItem(account);
                    queryLatestTransactionsForAccount({ accountId: account.id });
                }
            },
            items: accounts.data as any
        }), [accounts.data]);

    const items = searching ? accountsStartingWith.data : accounts.data;

    const [setDebitAccountCommands, setSetDebitAccountCommands] = useState<SetDebitAccountName[]>([]);

    useEffect(() => {
        setSetDebitAccountCommands(accounts.data.map(_ => {
            const command = new SetDebitAccountName();
            command.setInitialValues({
                accountId: _.id,
                name: _.name
            });
            return command;
        }));
    }, [accounts.data]);

    return (
        <>
            <CommandTracker>
                <Stack>
                    <Stack.Item disableShrink>
                        <CommandTrackerContext.Consumer>
                            {value => {
                                const actualItems: ICommandBarItemProps[] = [{
                                    key: 'save',
                                    name: 'Save',
                                    iconProps: { iconName: 'Save' },
                                    disabled: !value.hasChanges,
                                    onClick: (e) => {
                                        value.execute();
                                    }
                                }, ...commandBarItems];

                                return (
                                    <CommandBar items={actualItems} />
                                );
                            }}
                        </CommandTrackerContext.Consumer>
                    </Stack.Item>
                    <Stack.Item>
                        <DetailsList columns={columns} items={items} selection={selection} />
                    </Stack.Item>
                </Stack>
            </CommandTracker>

            <CreateAccountDialog {...createAccountDialogProps} />
            <AmountDialog {...depositAmountDialogProps} />
            <AmountDialog {...withdrawAmountDialogProps} />
        </>
    );
};
