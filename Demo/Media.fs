namespace WinRTFs.Demo

open Avalonia.Media.Imaging

module Media =
    open System

    open Elmish

    open Avalonia.Controls
    open Avalonia.FuncUI.DSL
    open Avalonia.Layout

    open Avalonia.FuncUI.Elmish
    open Avalonia.FuncUI.Components.Hosts

    open WinRTFs.Core.Media

    type State =
        { currentPath: Option<string>
          imagesrc: Option<string> }

    let init =
        { currentPath = None
          imagesrc = None }

    type ActionTaken =
        | Video
        | Audio
        | Picture

    type Msg =
        | StartVideoRecording
        | StartAudioRecording
        | TakePicture
        | RecordSuccess
        | AfterAction of string * ActionTaken
        | SetImageSrc of Option<string>

    let update (msg: Msg) (state: State) =
        match msg with
        | AfterAction(path, action) ->
            let cmd =
                match action with
                | Audio
                | Video -> Cmd.ofMsg (SetImageSrc None)
                | Picture -> Cmd.ofMsg (SetImageSrc(Some path))

            { state with currentPath = Some path }, cmd
        | SetImageSrc src -> { state with imagesrc = src }, Cmd.none
        | StartVideoRecording ->
            let action() =
                async {
                    let! path = recordWebcamAsync (TimeSpan.FromSeconds(10.)) |> Async.AwaitTask
                    return (path, Video) }
            state, Cmd.OfAsync.perform action () AfterAction
        | StartAudioRecording ->
            let action() =
                async {
                    let! path = recordAudioAsync (TimeSpan.FromSeconds(10.)) |> Async.AwaitTask
                    return (path, Audio) }
            state, Cmd.OfAsync.perform action () AfterAction
        | TakePicture ->
            let action() =
                async {
                    let! path = takePictureAsync() |> Async.AwaitTask
                    return (path, Picture) }
            state, Cmd.OfAsync.perform action () AfterAction


    let view (state: State) (dispatch) =
        let path =
            match state.currentPath with
            | Some path -> path
            | None -> ""

        let src =
            match state.imagesrc with
            | Some src -> src
            | None -> ""

        DockPanel.create
            [ DockPanel.children
                [ StackPanel.create
                    [ StackPanel.dock Dock.Bottom
                      StackPanel.margin 5.0
                      StackPanel.spacing 5.0
                      StackPanel.orientation Orientation.Horizontal
                      StackPanel.children
                          [ Button.create
                              [ Button.onClick (fun _ -> dispatch TakePicture)
                                Button.content "Take Picture"
                                Button.classes [ "plus" ] ]
                            Button.create
                                [ Button.onClick (fun _ -> dispatch StartVideoRecording)
                                  Button.content "Record Video"
                                  Button.classes [ "minus" ] ]
                            Button.create
                                [ Button.onClick (fun _ -> dispatch StartAudioRecording)
                                  Button.content "Record Audio" ] ] ]
                  StackPanel.create
                      [ StackPanel.spacing 12.
                        StackPanel.children
                            [ if String.IsNullOrEmpty src
                              then TextBlock.create [ TextBlock.text "No picture yet" ]
                              else Image.create [ Image.maxHeight 220.; Image.maxWidth 220.; Image.source (Bitmap.Create src) ]

                              TextBox.create
                                  [ TextBox.dock Dock.Top
                                    TextBox.fontSize 18.0
                                    TextBox.verticalAlignment VerticalAlignment.Center
                                    TextBox.horizontalAlignment HorizontalAlignment.Center
                                    TextBox.isReadOnly true
                                    TextBox.text (sprintf "Path to file: %s" path) ] ] ] ] ]

    type Host() as this =
        inherit HostControl()
        do

            Program.mkProgram (fun () -> init, Cmd.none) update view
            |> Program.withHost this
            |> Program.run
