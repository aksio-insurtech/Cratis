// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aksio.Applications.Queries;

namespace Aksio.Cratis.Kernel.Read;

/// <summary>
/// Extension methods for working with <see cref="ClientObservable{T}"/>.
/// </summary>
public static class ClientObservableExtensions
{
    /// <summary>
    /// Convert a regular observable to a <see cref="ClientObservable{T}"/>.
    /// </summary>
    /// <param name="observable"><see cref="IObservable{T}"/> to convert.</param>
    /// <typeparam name="TType">Type being observed.</typeparam>
    /// <returns><see cref="ClientObservable{T}"/>. </returns>
    public static ClientObservable<TType> ToClientObservable<TType>(this IObservable<TType> observable)
    {
        var clientObservable = new ClientObservable<TType>();
        var subscription = observable.Subscribe(clientObservable.OnNext);
        clientObservable.ClientDisconnected = subscription.Dispose;
        return clientObservable;
    }
}
