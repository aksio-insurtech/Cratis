// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import 'reflect-metadata';
import ReactDOM from 'react-dom';

import '@aksio/cratis-workbench.styles/theme';
import './index.scss';

import { App } from './App';

ReactDOM.render(
    <App />,
    document.getElementById('root')
);
