// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Immutable;

namespace Aksio.Cratis.Kernel.Orleans.StateMachines;

public class StateWithoutAllowedTransitionState : BaseState
{
    public override StateName Name => "State without allowed transition";

    protected override IImmutableList<Type> AllowedTransitions => ImmutableList<Type>.Empty;
}