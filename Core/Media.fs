namespace WinRTFs.Core

module Media =
    open System
    open System.IO
    open System.Threading.Tasks
    open Windows.Media.MediaProperties
    open Windows.Media.Capture
    open Windows.Storage

    let private getTempPathAsync() = StorageFolder.GetFolderFromPathAsync(Path.GetTempPath()).AsTask()

    /// <summary>Starts recording via webcam the amount of time supplied and returns the path of the created file</summary>
    /// <param name="time">Amount of time to record</param>
    /// <returns>The path of the file that was produced</returns>
    let recordWebcamAsync (time: TimeSpan) =
        async {
            use capture = new MediaCapture()
            do! capture.InitializeAsync().AsTask() |> Async.AwaitTask

            let format = MediaEncodingProfile.CreateMp4(VideoEncodingQuality.Auto)

            let! tmp = getTempPathAsync() |> Async.AwaitTask
            let! file = tmp.CreateFileAsync("TestVideo.mp4", CreationCollisionOption.GenerateUniqueName).AsTask()
                        |> Async.AwaitTask

            do! capture.StartRecordToStorageFileAsync(format, file).AsTask() |> Async.AwaitTask

            do! Task.Delay(time) |> Async.AwaitTask
            do! capture.StopRecordAsync().AsTask() |> Async.AwaitTask
            return file.Path
        }
        |> Async.StartAsTask

    /// <summary>Takes a picture with the webcam</summary>
    /// <returns>The path of the file that was produced</returns>
    let takePictureAsync() =
        async {
            use capture = new MediaCapture()

            do! capture.InitializeAsync().AsTask() |> Async.AwaitTask

            let format = ImageEncodingProperties.CreatePng()

            let! tmp = getTempPathAsync() |> Async.AwaitTask

            let! file = tmp.CreateFileAsync("picture.png", CreationCollisionOption.GenerateUniqueName).AsTask()
                        |> Async.AwaitTask

            do! capture.CapturePhotoToStorageFileAsync(format, file).AsTask() |> Async.AwaitTask

            return file.Path
        }
        |> Async.StartAsTask

    /// <summary>Starts recording via microphone the amount of time supplied and returns the path of the created file</summary>
    /// <param name="time">Amount of time to record</param>
    /// <returns>The path of the file that was produced</returns>
    let recordAudioAsync (time: TimeSpan) =
        async {
            let settings = MediaCaptureInitializationSettings()
            settings.StreamingCaptureMode <- StreamingCaptureMode.Audio

            use capture = new MediaCapture()

            do! capture.InitializeAsync(settings).AsTask() |> Async.AwaitTask

            let format = MediaEncodingProfile.CreateMp3(AudioEncodingQuality.Auto)

            let! tmp = getTempPathAsync() |> Async.AwaitTask

            let! file = tmp.CreateFileAsync("audio.mp3", CreationCollisionOption.GenerateUniqueName).AsTask()
                        |> Async.AwaitTask

            do! capture.StartRecordToStorageFileAsync(format, file).AsTask() |> Async.AwaitTask

            do! Task.Delay(time) |> Async.AwaitTask
            do! capture.StopRecordAsync().AsTask() |> Async.AwaitTask
            return file.Path
        }
        |> Async.StartAsTask
