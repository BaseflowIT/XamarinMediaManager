﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using MediaManager.Abstractions.Enums;
using MediaManager.Abstractions.EventArguments;
using MediaManager.Media;

namespace MediaManager.Playback
{
    public delegate void StatusChangedEventHandler(object sender, StatusChangedEventArgs e);
    public delegate void PlayingChangedEventHandler(object sender, PlayingChangedEventArgs e);
    public delegate void BufferingChangedEventHandler(object sender, BufferingChangedEventArgs e);
    public delegate void MediaFinishedEventHandler(object sender, MediaFinishedEventArgs e);
    public delegate void MediaFailedEventHandler(object sender, MediaFailedEventArgs e);
    public delegate void MediaItemChangedEventHandler(object sender, MediaFileChangedEventArgs e);
    public delegate void MediaItemFailedEventHandler(object sender, MediaItemFailedEventArgs e);

    public interface IPlaybackManager : INotifyPropertyChanged
    {
        /// <summary>
        /// Reading the current status of the player
        /// </summary>
        MediaPlayerState State { get; }

        /// <summary>
        /// Gets the players position
        /// </summary>
        TimeSpan Position { get; }

        /// <summary>
        /// Gets the source duration
        /// If the response is TimeSpan.Zero, the duration is unknown or the player is still buffering.
        /// </summary>
        TimeSpan Duration { get; }

        /// <summary>
        /// Gets the buffered time
        /// </summary>
        TimeSpan Buffered { get; }

        /// <summary>
        /// Plays the current MediaFile
        /// </summary>
        Task Play();

        // <summary>
        /// Adds MediaFile to the Queue and starts playing
        /// </summary>
        Task Play(IMediaItem mediaItem);

        Task<IMediaItem> Play(string uri);

        Task Play(IEnumerable<IMediaItem> items);

        Task<IEnumerable<IMediaItem>> Play(IEnumerable<string> items);

        //TODO: Move to extension
        //Task<IMediaItem> Play(FileInfo file);

        //TODO: Make check inside normal api?
        //Task PlayFromQueue(IMediaItem mediaItem);

        /// <summary>
        /// Pauses the current MediaFile
        /// </summary>
        Task Pause();

        /// <summary>
        /// Stops playing
        /// </summary>
        Task Stop();

        /// <summary>
        /// Plays the previous MediaFile
        /// </summary>
        Task PlayPrevious();

        /// <summary>
        /// Plays the next MediaFile
        /// </summary>
        /// <returns></returns>
        Task PlayNext();

        /// <summary>
        /// Seeks to the start of the current MediaFile
        /// </summary>
        Task SeekToStart();

        /// <summary>
        /// Seeks forward a fixed amount of seconds of the current MediaFile
        /// </summary>
        Task StepForward();

        /// <summary>
        /// Seeks backward a fixed amount of seconds of the current MediaFile
        /// </summary>
        Task StepBackward();

        /// <summary>
        /// Seeks to the specified amount of seconds
        /// </summary>
        /// <param name="position"></param>
        Task SeekTo(TimeSpan position);

        /// <summary>
        /// Toggles between the different repeat: modes None, RepeatOne and RepeatAll
        /// </summary>
        void ToggleRepeat();

        /// <summary>
        /// Enables or disables shuffling
        /// </summary>
        void ToggleShuffle();
    }
}