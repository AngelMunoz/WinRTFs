namespace WinRTFs.Core

module Network =
    open Windows.Networking.Connectivity

    let stringifyConnectivity (conectivity: NetworkConnectivityLevel) =
        let status =
            match conectivity with
            | NetworkConnectivityLevel.None -> "No connection"
            | NetworkConnectivityLevel.LocalAccess -> "Local connection"
            | NetworkConnectivityLevel.ConstrainedInternetAccess -> "Limited connection"
            | NetworkConnectivityLevel.InternetAccess -> "Connected"
            | _ -> "Unkwnown"
        sprintf "Internet Access: %s" status

    let printProfiles() =
        printfn "Network Profiles:"
        for profile in NetworkInformation.GetConnectionProfiles() do
            printfn "%s\n\tSignal Bars: %A\n\tConectivity: %s" profile.ProfileName (profile.GetSignalBars())
                (stringifyConnectivity (profile.GetNetworkConnectivityLevel()))

    let getNetworkLevel (profile: ConnectionProfile) =
        match profile |> Option.ofObj with
        | Some profile -> profile.GetNetworkConnectivityLevel()
        | None -> NetworkConnectivityLevel.None
