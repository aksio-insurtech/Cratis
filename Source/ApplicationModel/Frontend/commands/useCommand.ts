// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { Constructor } from '@aksio/cratis-fundamentals';
import { useState, useEffect } from 'react';
import { Command } from '@aksio/cratis-applications-frontend/commands';
import React from 'react';
import { CommandTrackerContext } from './CommandTracker';

export type SetCommandValues<TCommandContent> = (command: TCommandContent) => void;

export function useCommand<TCommand extends Command, TCommandContent>(commandType: Constructor<TCommand>, initialValues?: TCommandContent): [TCommand, SetCommandValues<TCommandContent>] {
    const instance = new commandType();
    const [command, setCommand] = useState<TCommand>(instance);

    useEffect(() => {
        if (initialValues) {
            instance.setInitialValues(initialValues);
        }
    }, []);

    const context = React.useContext(CommandTrackerContext);
    context.addCommand?.(command!);

    const setCommandValues = (values: TCommandContent) => {
        command!.properties.forEach(property => {
            if (values[property]) {
                command![property] = values[property];
            }
        });
    };

    return [command!, setCommandValues];
}
