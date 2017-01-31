﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.Enums;
using Plugin.MediaManager.Abstractions.Implementations;

namespace Plugin.MediaManager.Tests.Unit
{
    [TestFixture]
    public class PlaybackControllerTests
    {
        private Mock<IMediaManager> _mediaManagerMock;
        private IMediaManager MediaManager => _mediaManagerMock.Object;

        [SetUp]
        public void Init()
        {
            _mediaManagerMock = new Mock<IMediaManager>(MockBehavior.Strict);
        }

        [Test]
        public async Task PlayPause_Playing_Pauses()
        {
            _mediaManagerMock
                .Setup(mediaManager => mediaManager.Pause())
                .Returns(Task.FromResult(0));

            MediaManagerStatus = MediaPlayerStatus.Playing;

            var playbackController = new PlaybackController(MediaManager);

            await playbackController.PlayPause();

            _mediaManagerMock
                .Verify(mediaManager => mediaManager.Pause(), Times.Once);
        }

        [Test, TestCaseSource(nameof(NotPlayingStatuses))]
        public async Task PlayPause_NotPlaying_Pauses(MediaPlayerStatus notPlayingStatus)
        {
            _mediaManagerMock
                .Setup(mediaManager => mediaManager.Play((IMediaFile) null))
                .Returns(Task.FromResult(0));

            MediaManagerStatus = notPlayingStatus;

            var playbackController = new PlaybackController(MediaManager);

            await playbackController.PlayPause();

            _mediaManagerMock
                .Verify(mediaManager => mediaManager.Play((IMediaFile) null), Times.Once);
        }

        [Test]
        public async Task PlayPrevious_QueueHasPrevious_PlaysPrevious()
        {
            _mediaManagerMock
                .Setup(mediaManager => mediaManager.PlayPrevious())
                .Returns(Task.FromResult(0));

            MediaQueue = GetMediaQueue(hasPrevious: true);

            var playbackController = new PlaybackController(MediaManager);

            await playbackController.PlayPrevious();

            _mediaManagerMock
                .Verify(mediaManager => mediaManager.PlayPrevious(), Times.Once);
        }

        [Test]
        public async Task PlayPrevious_QueueHasNoPrevious_SeeksToStart()
        {
            _mediaManagerMock
                .Setup(mediaManager => mediaManager.Seek(It.IsAny<TimeSpan>()))
                .Returns(Task.FromResult(0));

            Duration = TimeSpan.Zero;

            MediaQueue = GetMediaQueue(hasPrevious: false);

            var playbackController = new PlaybackController(MediaManager);

            await playbackController.PlayPrevious();

            _mediaManagerMock
                .Verify(mediaManager => mediaManager.Seek(TimeSpan.Zero), Times.Once);
        }

        [Test, Sequential]
        public async Task PlayPreviousOrSeekToStart(
            [Values(2,4)] int positionSeconds,
            [Values(false, true)] bool afterTreshold
        )
        {
            Duration = TimeSpan.FromSeconds(10);

            if (afterTreshold)
            {
                _mediaManagerMock
                    .Setup(mediaManager => mediaManager.Seek(It.IsAny<TimeSpan>()))
                    .Returns(Task.FromResult(0));
            }
            else
            {
                _mediaManagerMock
                    .Setup(mediaManager => mediaManager.PlayPrevious())
                    .Returns(Task.FromResult(0));
            }

            MediaQueue = GetMediaQueue(hasPrevious: true);

            Position = TimeSpan.FromSeconds(positionSeconds);

            var playbackController = new PlaybackController(MediaManager);

            await playbackController.PlayPreviousOrSeekToStart();

            if (afterTreshold)
            {
                _mediaManagerMock
                    .Verify(mediaManager => mediaManager.Seek(TimeSpan.Zero), Times.Once);
            }
            else
            {
                _mediaManagerMock
                    .Verify(mediaManager => mediaManager.PlayPrevious(), Times.Once);
            }
        }

        private IMediaQueue GetMediaQueue(bool hasPrevious)
        {
            var mediaQueueMock = new Mock<IMediaQueue>();
            mediaQueueMock
                .Setup(queue => queue.HasPrevious())
                .Returns(hasPrevious);

            var mediaQueue = mediaQueueMock.Object;

            return mediaQueue;
        }

        private TimeSpan Duration
        {
            set
            {
                var duration = value;

                _mediaManagerMock
                    .SetupGet(mediaManager => mediaManager.Duration)
                    .Returns(duration);
            }
        }

        private TimeSpan Position
        {
            set
            {
                var position = value;

                _mediaManagerMock
                    .SetupGet(mediaManager => mediaManager.Position)
                    .Returns(position);
            }
        }

        private IMediaQueue MediaQueue
        {
            set
            {
                var queue = value;

                _mediaManagerMock
                    .SetupGet(mediaManager => mediaManager.MediaQueue)
                    .Returns(queue);
            }
        }

        private MediaPlayerStatus MediaManagerStatus
        {
            set
            {
                var status = value;

                _mediaManagerMock
                    .SetupGet(mediaManager => mediaManager.Status)
                    .Returns(status);
            }
        }

        public static IEnumerable NotPlayingStatuses
        {
            get
            {
                var notPlayingStatuses = new List<MediaPlayerStatus>
                {
                    MediaPlayerStatus.Paused,
                    MediaPlayerStatus.Stopped
                };

                foreach (var status in notPlayingStatuses)
                {
                    yield return new TestCaseData(status);
                }
            }
        }
    }
}