namespace WinRTFs.Demo

/// This is the main module of your application
/// here you handle all of your child pages as well as their
/// messages and their updates, useful to update multiple parts
/// of your application, Please refer to the `view` function
/// to see how to handle different kinds of "*child*" controls
module Shell =
    open System
    open Elmish
    open Avalonia
    open Avalonia.Controls
    open Avalonia.Input
    open Avalonia.FuncUI.DSL
    open Avalonia.FuncUI
    open Avalonia.FuncUI.Builder
    open Avalonia.FuncUI.Components.Hosts
    open Avalonia.FuncUI.Elmish
    open Windows.Networking.Connectivity
    open Windows.System.Power
    open WinRTFs.Core.Network
    open WinRTFs.Core.Power

    type State =
        { /// store the child state in your main state
          AboutState: About.State
          CounterState: Counter.State
          NetworkConnectivityLevel: NetworkConnectivityLevel
          PowerInformation: PowerInformation }

    type Msg =
        | AboutMsg of About.Msg
        | CounterMsg of Counter.Msg
        | NetworkChanged of ConnectionProfile
        | BatteryStatusChanged of BatteryStatus
        | PowerSupplyStatusChanged of PowerSupplyStatus
        | EnergySaverStatusChanged of EnergySaverStatus
        | RemainingChargePercentChanged of int32
        | RemainingDischargeTimeChanged of TimeSpan

    let init networkLevel powerinfo =
        let aboutState, aboutCmd = About.init
        let counterState = Counter.init
        { AboutState = aboutState
          CounterState = counterState
          NetworkConnectivityLevel = networkLevel
          PowerInformation = powerinfo }, Cmd.batch [ aboutCmd ]

    /// If your children controls don't emit any commands
    /// in the init function, you can just return Cmd.none
    /// otherwise, you can use a batch operation on all of them
    /// you can add more init commands as you need
    let update (msg: Msg) (state: State): State * Cmd<_> =
        match msg with
        | AboutMsg bpmsg ->
            let aboutState, cmd = About.update bpmsg state.AboutState
            { state with AboutState = aboutState }, Cmd.map AboutMsg cmd
        | CounterMsg countermsg ->
            /// map the message to the kind of message
            /// your child control needs to handle
            let counterMsg = Counter.update countermsg state.CounterState

            { state with CounterState = counterMsg }, Cmd.none
        | NetworkChanged profile -> { state with NetworkConnectivityLevel = getNetworkLevel profile }, Cmd.none
        | BatteryStatusChanged status ->
            { state with PowerInformation = { state.PowerInformation with BatteryStatus = status } }, Cmd.none
        | PowerSupplyStatusChanged status ->
            { state with PowerInformation = { state.PowerInformation with PowerSupplyStatus = status } }, Cmd.none
        | EnergySaverStatusChanged status ->
            { state with PowerInformation = { state.PowerInformation with EnergySaverStatus = status } }, Cmd.none
        | RemainingChargePercentChanged charge ->
            { state with PowerInformation = { state.PowerInformation with RemainingCharge = charge } }, Cmd.none
        | RemainingDischargeTimeChanged timeLeft ->
            { state with PowerInformation = { state.PowerInformation with RemainingRuntime = timeLeft } }, Cmd.none

    /// map the message to the kind of message
    /// your child control needs to handle
    let private networkMenu state =
        Menu.create
            [ Menu.dock Dock.Bottom
              Menu.viewItems
                  [ MenuItem.create [ MenuItem.header (stringifyConnectivity state.NetworkConnectivityLevel) ] ] ]

    let private powerMenu state =
        Menu.create
            [ Menu.dock Dock.Bottom
              Menu.viewItems
                  [ MenuItem.create
                      [ MenuItem.header (stringifyEnergySaverStatus state.PowerInformation.EnergySaverStatus) ]
                    MenuItem.create
                        [ MenuItem.header (stringifyRemainingCharge state.PowerInformation.RemainingCharge) ]
                    MenuItem.create
                        [ MenuItem.header (stringifyRemainingTime state.PowerInformation.RemainingRuntime) ]
                    MenuItem.create [ MenuItem.header (stringifyBatteryStatus state.PowerInformation.BatteryStatus) ]
                    MenuItem.create
                        [ MenuItem.header (stringifyPowerSupplyStatus state.PowerInformation.PowerSupplyStatus) ] ] ]

    let view (state: State) (dispatch) =
        DockPanel.create
            [ DockPanel.children
                [ powerMenu state
                  networkMenu state
                  TabControl.create
                      [ TabControl.tabStripPlacement Dock.Top
                        TabControl.dock Dock.Top
                        TabControl.viewItems
                            [ TabItem.create
                                [ TabItem.header "Counter Sample"
                                  TabItem.content (Counter.view state.CounterState (CounterMsg >> dispatch)) ]
                              TabItem.create
                                  [ TabItem.header "Media APIs"
                                    TabItem.content (ViewBuilder.Create<Media.Host>([])) ]
                              TabItem.create
                                  [ TabItem.header "About"
                                    TabItem.content (About.view state.AboutState (AboutMsg >> dispatch)) ] ] ] ] ]

    /// This is the main window of your application
    /// you can do all sort of useful things here like setting heights and widths
    /// as well as attaching your dev tools that can be super useful when developing with
    /// Avalonia
    type MainWindow() as this =
        inherit HostWindow()

        do
            base.Title <- "Full App"
            base.Width <- 800.0
            base.Height <- 600.0
            base.MinWidth <- 800.0
            base.MinHeight <- 600.0

            let networkStatus state =
                let sub dispatch =
                    NetworkInformation.NetworkStatusChanged.AddHandler
                        (fun _ -> dispatch (NetworkChanged(NetworkInformation.GetInternetConnectionProfile())))
                    |> ignore

                Cmd.ofSub sub

            let batteryStatus state =
                let sub dispatch =
                    PowerManager.BatteryStatusChanged.Subscribe
                        (fun _ -> dispatch (BatteryStatusChanged PowerManager.BatteryStatus)) |> ignore

                Cmd.ofSub sub

            let powerSupplyStatus state =
                let sub dispatch =
                    PowerManager.PowerSupplyStatusChanged.Subscribe
                        (fun _ -> dispatch (PowerSupplyStatusChanged PowerManager.PowerSupplyStatus)) |> ignore

                Cmd.ofSub sub

            let energySaverStatus state =
                let sub dispatch =
                    PowerManager.EnergySaverStatusChanged.Subscribe
                        (fun _ -> dispatch (EnergySaverStatusChanged PowerManager.EnergySaverStatus)) |> ignore

                Cmd.ofSub sub

            let chargeRemaining state =
                let sub dispatch =
                    PowerManager.RemainingChargePercentChanged.Subscribe
                        (fun _ -> dispatch (RemainingChargePercentChanged PowerManager.RemainingChargePercent))
                    |> ignore

                Cmd.ofSub sub

            let runtimeRemaining state =
                let sub dispatch =
                    PowerManager.RemainingDischargeTimeChanged.Subscribe
                        (fun _ -> dispatch (RemainingDischargeTimeChanged PowerManager.RemainingDischargeTime))
                    |> ignore

                Cmd.ofSub sub

            let networklevel = getNetworkLevel (NetworkInformation.GetInternetConnectionProfile())
            let powerinfo = getPowerInfo()
            printProfiles()

            Program.mkProgram (fun () -> init networklevel powerinfo) update view
            |> Program.withHost this
            |> Program.withSubscription networkStatus
            |> Program.withSubscription batteryStatus
            |> Program.withSubscription powerSupplyStatus
            |> Program.withSubscription energySaverStatus
            |> Program.withSubscription chargeRemaining
            |> Program.withSubscription runtimeRemaining
            |> Program.run
