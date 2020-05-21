namespace WinRTFs.Core


module Power =
    open System
    open Windows.System.Power

    type PowerInformation =
        { BatteryStatus: BatteryStatus
          PowerSupplyStatus: PowerSupplyStatus
          EnergySaverStatus: EnergySaverStatus
          RemainingCharge: int
          RemainingRuntime: TimeSpan }

    let getPowerInfo(): PowerInformation =
        { BatteryStatus = PowerManager.BatteryStatus
          PowerSupplyStatus = PowerManager.PowerSupplyStatus
          EnergySaverStatus = PowerManager.EnergySaverStatus
          RemainingCharge = PowerManager.RemainingChargePercent
          RemainingRuntime = PowerManager.RemainingDischargeTime }

    let stringifyRemainingCharge charge = sprintf "Remaining charge: %i%%" charge

    let stringifyRemainingTime (timeLeft: TimeSpan) =
        match timeLeft > TimeSpan.FromDays(1.0) with
        | true -> sprintf "Time Left: 1d+"
        | false -> sprintf "Time Left: %s" (timeLeft.ToString())

    let stringifyBatteryStatus (status: BatteryStatus) =
        let battery =
            match status with
            | BatteryStatus.Charging -> "Charging"
            | BatteryStatus.Discharging -> "Discharging"
            | BatteryStatus.Idle -> "Idle"
            | BatteryStatus.NotPresent -> "Not Present"
            | _ -> "Unkwnown"

        sprintf "Battery Status: %s" battery

    let stringifyPowerSupplyStatus (status: PowerSupplyStatus) =
        let powerSupply =
            match status with
            | PowerSupplyStatus.Adequate -> "Adequate"
            | PowerSupplyStatus.Inadequate -> "Inadequate"
            | PowerSupplyStatus.NotPresent -> "Not Present"
            | _ -> "Unkwnown"

        sprintf "Power Supply Status: %s" powerSupply


    let stringifyEnergySaverStatus (status: EnergySaverStatus) =
        let energySaver =
            match status with
            | EnergySaverStatus.On -> "On"
            | EnergySaverStatus.Off -> "Off"
            | EnergySaverStatus.Disabled -> "Disabled"
            | _ -> "Unkwnown"

        sprintf "Energy Saver Status: %s" energySaver
