// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EffectsProvider.cs" company="Itransition">
// Torchilov Pavel 07.09.2013  
// </copyright>
// <summary>
//   Defines the EffectsProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NAudioTest
{
    using NAudio.Wave;

    /// <summary>
    /// The effects provider.
    /// </summary>
    public class EffectsProvider : ISampleProvider
    {
        #region Fields
        
        /// <summary>
        /// The source
        /// </summary>
        private readonly ISampleProvider source;

        /// <summary>
        /// The fade sample position
        /// </summary>
        private long fadeSamplePosition;

        /// <summary>
        /// The fade sample count
        /// </summary>
        private long fadeSampleCount;

        /// <summary>
        /// The fade sample skip
        /// </summary>
        private long fadeSampleSkip;

        /// <summary>
        /// The fade state
        /// </summary>
        private FadeState fadeState;

        /// <summary>
        /// The maximum sample
        /// </summary>
        private float maxSample;

        #endregion

        #region Constructors
        
        /// <summary>
        /// Initializes a new instance of the <see cref="EffectsProvider"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        public EffectsProvider(ISampleProvider source)
        {
            this.source = source;
            fadeState = FadeState.FullVolume;
        }

        #endregion

        #region Enums

        /// <summary>
        /// Current effect
        /// </summary>
        private enum FadeState
        {
            /// <summary>
            /// The fading in.
            /// </summary>
            FadingIn,

            /// <summary>
            /// The full volume.
            /// </summary>
            FullVolume,

            /// <summary>
            /// The fading out.
            /// </summary>
            FadingOut,

            /// <summary>
            /// The normalize.
            /// </summary>
            Normalize
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the wave format.
        /// </summary>
        /// <value>
        /// The wave format.
        /// </value>
        public WaveFormat WaveFormat
        {
            get { return source.WaveFormat; }
        }

        #endregion

        #region Public Methods
        
        /// <summary>
        /// Begins the fade information.
        /// </summary>
        /// <param name="fadeDurationInMilliseconds">The fade duration information milliseconds.</param>
        public void BeginFadeIn(double fadeDurationInMilliseconds)
        {
            fadeSamplePosition = 0;
            fadeSampleCount = (long)((fadeDurationInMilliseconds * source.WaveFormat.SampleRate) / 1000);
            fadeSampleCount = (long)((fadeDurationInMilliseconds * source.WaveFormat.SampleRate) / 1000);
            fadeState = FadeState.FadingIn;
        }

        /// <summary>
        /// Ends the fade out.
        /// </summary>
        /// <param name="fadeDurationInMilliseconds">The fade duration information milliseconds.</param>
        /// <param name="fullTime">The full time.</param>
        public void EndFadeOut(double fadeDurationInMilliseconds, long fullTime)
        {
            fadeSamplePosition = 0;
            fadeSampleSkip = fullTime - (long)((fadeDurationInMilliseconds * source.WaveFormat.SampleRate) / 1000);
            fadeSampleCount = (long)((fadeDurationInMilliseconds * source.WaveFormat.SampleRate) / 1000);
            fadeState = FadeState.FadingOut;
        }

        /// <summary>
        /// Normalizes the specified maximum.
        /// </summary>
        /// <param name="max">The maximum.</param>
        public void Normalize(float max)
        {
            fadeState = FadeState.Normalize;
            maxSample = max;
        }

        /// <summary>
        /// Reads the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        /// <returns>Number of samples read</returns>
        public int Read(float[] buffer, int offset, int count)
        {
            var sourceSamplesRead = source.Read(buffer, offset, count);
            switch (fadeState)
            {
                case FadeState.FadingIn:
                    FadeIn(buffer, offset, sourceSamplesRead);
                    break;
                case FadeState.FadingOut:
                    FadeOut(buffer, offset, sourceSamplesRead);
                    break;
                case FadeState.Normalize:
                    NormalizeSample(buffer, offset, sourceSamplesRead);
                    break;
            }

            return sourceSamplesRead;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Fades the out.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="sourceSamplesRead">The source samples read.</param>
        private void FadeOut(float[] buffer, int offset, int sourceSamplesRead)
        {
            var sample = 0;
            while (sample < sourceSamplesRead)
            {
                if (fadeSamplePosition > fadeSampleSkip)
                {
                    fadeState = FadeState.FadingOut;
                    var multiplier = 1.0f - ((fadeSamplePosition - fadeSampleSkip) / (float)fadeSampleCount);
                    for (var ch = 0; ch < source.WaveFormat.Channels; ch++)
                    {
                        buffer[offset + sample] *= multiplier;
                        sample++;
                    }
                }
                else
                {
                    sample += source.WaveFormat.Channels;
                }

                fadeSamplePosition++;
            }
        }

        /// <summary>
        /// Fades the information.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="sourceSamplesRead">The source samples read.</param>
        private void FadeIn(float[] buffer, int offset, int sourceSamplesRead)
        {
            var sample = 0;
            while (sample < sourceSamplesRead)
            {
                var multiplier = fadeSamplePosition / (float)fadeSampleCount;
                for (var ch = 0; ch < source.WaveFormat.Channels; ch++)
                {
                    buffer[offset + sample] *= multiplier;
                    sample++;
                }

                fadeSamplePosition++;
                if (fadeSamplePosition > fadeSampleCount)
                {
                    fadeState = FadeState.FullVolume;
                    break;
                }
            }
        }

        /// <summary>
        /// Normalizes the sample.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="sourceSamplesRead">The source samples read.</param>
        private void NormalizeSample(float[] data, int offset, int sourceSamplesRead)
        {
            var sample = 0;

            while (sample < sourceSamplesRead)
            {
                for (var ch = 0; ch < source.WaveFormat.Channels; ch++)
                {
                    data[offset + sample] /= maxSample;
                    sample++;
                }
            }
        }

        #endregion
    }
}