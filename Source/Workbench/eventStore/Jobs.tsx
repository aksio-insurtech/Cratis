// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { useEffect, useState } from 'react';
import { Divider, FormControl, Grid, InputLabel, MenuItem, Select, Stack, Toolbar, Typography } from '@mui/material';
import { DataGrid, GridColDef, GridRowSelectionModel, GridValueGetterParams } from '@mui/x-data-grid';
import { AllTenants } from 'API/configuration/tenants/AllTenants';
import { TenantInfo } from 'API/configuration/tenants/TenantInfo';
import { AllJobs } from 'API/jobs/AllJobs';
import { JobState } from 'API/jobs/JobState';
import { useRouteParams } from './RouteParams';
import { AllJobSteps } from 'API/jobs/AllJobSteps';
import { JobStatus } from 'API/jobs/JobStatus';
import { JobStepState } from 'API/jobs/JobStepState';
import { JobStepStatus } from 'API/jobs/JobStepStatus';

const getJobStatusText = (status: JobStatus) => {
    switch (status) {
        case JobStatus.none: return 'None';
        case JobStatus.running: return 'Running';
        case JobStatus.completedSuccessfully: return 'Completed successfully';
        case JobStatus.completedWithFailures: return 'Completed with failures';
        case JobStatus.paused: return 'Paused';
        case JobStatus.stopped: return 'Stopped';
    }
}

const getJobStepStatusText = (status: JobStepStatus) => {
    switch (status) {
        case JobStepStatus.unknown: return 'None';
        case JobStepStatus.scheduled: return 'Scheduled';
        case JobStepStatus.running: return 'Running';
        case JobStepStatus.succeeded: return 'Succeeded';
        case JobStepStatus.failed: return 'Failed';
    }
}

const jobColumns: GridColDef[] = [
    {
        headerName: 'Name',
        field: 'name',
        width: 300,
    },
    {
        headerName: 'Status',
        field: 'status',
        width: 100,
        valueGetter: (params: GridValueGetterParams<JobState>) => {
            return getJobStatusText(params.row.status);
        }
    },
    {
        headerName: 'Total steps',
        field: 'progress.totalSteps',
        width: 130,
        valueGetter: (params: GridValueGetterParams<JobState>) => {
            return params.row.progress.totalSteps;
        }
    },
    {
        headerName: 'Successful steps',
        field: 'progress.successFulSteps',
        width: 130,
        valueGetter: (params: GridValueGetterParams<JobState>) => {
            return params.row.progress.successfulSteps;
        }
    },
    {
        headerName: 'Failed steps',
        field: 'progress.failedSteps',
        width: 130,
        valueGetter: (params: GridValueGetterParams<JobState>) => {
            return params.row.progress.failedSteps;
        }
    }
];

const jobStepColumns: GridColDef[] = [
    {
        headerName: 'Name',
        field: 'name',
        width: 300,
    },
    {
        headerName: 'Status',
        field: 'status',
        width: 200,
        valueGetter: (params: GridValueGetterParams<JobStepState>) => {
            return getJobStepStatusText(params.row.status);
        }
    },
]


export const Jobs = () => {
    const { microserviceId } = useRouteParams();
    const [tenants] = AllTenants.use();
    const [selectedTenant, setSelectedTenant] = useState<TenantInfo>();

    const [jobs] = AllJobs.use({
        microserviceId,
        tenantId: selectedTenant?.id || undefined!,
    });
    const [selectedJob, setSelectedJob] = useState<JobState | undefined>(undefined);
    const [jobSteps] = AllJobSteps.use({
        microserviceId,
        tenantId: selectedTenant?.id || undefined!,
        jobId: selectedJob?.id!
    });

    useEffect(() => {
        if (tenants.data.length > 0) {
            setSelectedTenant(tenants.data[0]);
        }
    }, [tenants.data]);


    const jobSelected = (selection: GridRowSelectionModel) => {
        const selectedJobs = selection.map(id => jobs.data.find(job => job.id === id));
        if (selectedJobs.length == 1) {
            setSelectedJob(selectedJobs[0]);
        }
    }

    return (
        <Stack direction="column" style={{ height: '100%' }}>
            <Typography variant='h4'>Jobs</Typography>
            <Divider sx={{ mt: 1, mb: 3 }} />

            <Toolbar>
                <FormControl size="small" sx={{ m: 1, minWidth: 120 }}>
                    <InputLabel>Tenant</InputLabel>
                    <Select
                        label="Tenant"
                        autoWidth
                        value={selectedTenant?.id || ''}
                        onChange={e => setSelectedTenant(tenants.data.find(_ => _.id == e.target.value))}>

                        {tenants.data.map(tenant => {
                            return (
                                <MenuItem key={tenant.id} value={tenant.id}>{tenant.name}</MenuItem>
                            );
                        })}
                    </Select>
                </FormControl>
            </Toolbar>

            <Grid container spacing={2} sx={{ height: '100%' }}>
                <Grid item xs={8}>
                    <DataGrid
                        columns={jobColumns}
                        filterMode="client"
                        sortingMode="client"
                        getRowId={(row: JobState) => row.id}
                        onRowSelectionModelChange={jobSelected}
                        rows={jobs.data}
                    />
                </Grid>

                <Grid item xs={4} >
                    <DataGrid
                        columns={jobStepColumns}
                        filterMode="client"
                        sortingMode="client"
                        getRowId={(row: JobStepState) => row.grainId.toString()}
                        onRowSelectionModelChange={() => { }}
                        rows={jobSteps.data}
                    />
                </Grid>
            </Grid>
        </Stack>
    );
};