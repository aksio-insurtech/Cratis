// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aksio.Cratis.Execution;

namespace Aksio.Cratis.Compliance.Domain.Microservices;

public record AddMicroservice(MicroserviceId MicroserviceId, string Name);
