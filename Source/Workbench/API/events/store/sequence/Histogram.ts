/*---------------------------------------------------------------------------------------------
 *  **DO NOT EDIT** - This file is an automatically generated file.
 *--------------------------------------------------------------------------------------------*/

import { QueryFor, QueryResult, useQuery, PerformQuery } from '@aksio/cratis-applications-frontend/queries';
import { EventHistogramEntry } from './EventHistogramEntry';
import Handlebars from 'handlebars';

const routeTemplate = Handlebars.compile('/api/events/store/sequence/histogram');

export interface HistogramArguments {
    eventSequenceId: string;
}
export class Histogram extends QueryFor<EventHistogramEntry[], HistogramArguments> {
    readonly route: string = '/api/events/store/sequence/histogram';
    readonly routeTemplate: Handlebars.TemplateDelegate = routeTemplate;
    readonly defaultValue: EventHistogramEntry[] = [];

    get requestArguments(): string[] {
        return [
            'eventSequenceId',
        ];
    }

    static use(args?: HistogramArguments): [QueryResult<EventHistogramEntry[]>, PerformQuery<HistogramArguments>] {
        return useQuery<EventHistogramEntry[], Histogram, HistogramArguments>(Histogram, args);
    }
}
