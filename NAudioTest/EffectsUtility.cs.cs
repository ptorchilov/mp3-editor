// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EffectsUtility.cs.cs" company="Itransition">
//   Pavel Torchilov 07.09.2013 
// </copyright>
// <summary>
//   Class for make effects on audio files
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NAudioTest
{
    using System;
    using System.IO;

    using NAudio.Wave;
    using NAudio.Wave.SampleProviders;

    using Yeti.Lame;
    using Yeti.MMedia.Mp3;

    using WaveStream = WaveLib.WaveStream;

    /// <summary>
    /// Class for make effects on audio files
    /// </summary>
    public static class EffectsUtility
    {
        #region Fields

        /// <summary>
        /// The default bit rate
        /// </summary>
        private const uint DefaultBitRate = 128;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the bit rate.
        /// </summary>
        /// <value>
        /// The bit rate.
        /// </value>
        public static double BitRate { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Normalizes the specified input file.
        /// </summary>
        /// <param name="inputFile">The input file.</param>
        /// <returns>
        /// Path of changed file
        /// </returns>
        public static string Normalize(string inputFile)
        {
            return Normalize(inputFile, Path.ChangeExtension(inputFile, ".normalize.wav"));
        }

        /// <summary>
        /// Normalizes the specified input file.
        /// </summary>
        /// <param name="inputFile">The input file.</param>
        /// <param name="outputFile">The output file.</param>
        /// <returns>
        /// Path of changed file
        /// </returns>
        public static string Normalize(string inputFile, string outputFile)
        {
            using (var reader = new AudioFileReader(inputFile))
            {
                var provider = new EffectsProvider(reader);
                provider.Normalize(GetMax(inputFile));
                WaveFileWriter.CreateWaveFile16(outputFile, provider);
            }

            return outputFile;
        }

        /// <summary>
        /// Changes the volume.
        /// </summary>
        /// <param name="inputFile">The input file.</param>
        /// <param name="level">The level.</param>
        /// <returns>
        /// Path of changed file
        /// </returns>
        public static string ChangeVolume(string inputFile, double level)
        {
            return ChangeVolume(inputFile, Path.ChangeExtension(inputFile, ".volume.wav"), level);
        }

        /// <summary>
        /// Changes the volume.
        /// </summary>
        /// <param name="inputFile">The input file.</param>
        /// <param name="outputFile">The output file.</param>
        /// <param name="level">The level.</param>
        /// <returns>
        /// Path of changed file
        /// </returns>
        public static string ChangeVolume(string inputFile, string outputFile, double level)
        {
            using (var reader = new AudioFileReader(inputFile))
            {
                var provider = new VolumeSampleProvider(reader)
                {
                    Volume = Convert.ToSingle(level)
                };

                WaveFileWriter.CreateWaveFile16(outputFile, provider);
            }

            return outputFile;
        }

        /// <summary>
        /// Ends the fade out.
        /// </summary>
        /// <param name="inputFile">The input file.</param>
        /// <param name="duration">The duration.</param>
        /// <returns>
        /// Path of changed file
        /// </returns>
        public static string EndFadeOut(string inputFile, double duration)
        {
            return EndFadeOut(inputFile, Path.ChangeExtension(inputFile, ".fadeout.wav"), duration);
        }

        /// <summary>
        /// Ends the fade out.
        /// </summary>
        /// <param name="inputFile">The input file.</param>
        /// <param name="outputFile">The output file.</param>
        /// <param name="duration">The duration.</param>
        /// <returns>
        /// Path of changed file
        /// </returns>
        public static string EndFadeOut(string inputFile, string outputFile, double duration)
        {
            using (var reader = new AudioFileReader(inputFile))
            {
                var provider = new EffectsProvider(reader);
                provider.EndFadeOut(duration * 1000, reader.Length / reader.WaveFormat.BlockAlign);
                WaveFileWriter.CreateWaveFile16(outputFile, provider);
            }

            return outputFile;
        }

        /// <summary>
        /// Begins the fade information.
        /// </summary>
        /// <param name="inputFile">The input file.</param>
        /// <param name="duration">The duration.</param>
        /// <returns>
        /// Path of changed file
        /// </returns>
        public static string BeginFadeIn(string inputFile, double duration)
        {
            return BeginFadeIn(inputFile, Path.ChangeExtension(inputFile, ".fadein.wav"), duration);
        }

        /// <summary>
        /// Begins the fade information.
        /// </summary>
        /// <param name="inputFile">The input file.</param>
        /// <param name="outputFile">The output file.</param>
        /// <param name="duration">The duration.</param>
        /// <returns>
        /// Path of changed file
        /// </returns>
        public static string BeginFadeIn(string inputFile, string outputFile, double duration)
        {
            using (var reader = new AudioFileReader(inputFile))
            {
                var provider = new EffectsProvider(reader);
                provider.BeginFadeIn(duration * 1000);
                WaveFileWriter.CreateWaveFile16(outputFile, provider);
            }

            return outputFile;
        }

        /// <summary>
        /// Converts to WAV.
        /// </summary>
        /// <param name="inputFile">The input file.</param>
        /// <returns>
        /// Path of changed file
        /// </returns>
        public static string ConvertToWav(string inputFile)
        {
            return ConvertToWav(inputFile, Path.ChangeExtension(inputFile, ".wav"));
        }

        /// <summary>
        /// Converts to WAV.
        /// </summary>
        /// <param name="inputFile">The input file.</param>
        /// <param name="outputFile">The output file.</param>
        /// <returns>
        /// Path of changed file
        /// </returns>
        public static string ConvertToWav(string inputFile, string outputFile)
        {
            using (var reader = new Mp3FileReader(inputFile))
            {
                WaveFileWriter.CreateWaveFile(outputFile, reader);
            }

            return outputFile;
        }

        /// <summary>
        /// Trims the MP3.
        /// </summary>
        /// <param name="mp3Path">The MP3 path.</param>
        /// <param name="timeFromDouble">The time from double.</param>
        /// <param name="duration">The duration.</param>
        /// <returns>
        /// Path of changed file
        /// </returns>
        public static string TrimMp3(string mp3Path, double timeFromDouble, double duration)
        {
            return TrimMp3(mp3Path, Path.ChangeExtension(mp3Path, ".trimmed.mp3"), timeFromDouble, duration);
        }

        /// <summary>
        /// Trims the MP3.
        /// </summary>
        /// <param name="mp3Path">The MP3 path.</param>
        /// <param name="outputPath">The output path.</param>
        /// <param name="timeFromDouble">The time from double.</param>
        /// <param name="duration">The duration.</param>
        /// <returns>
        /// Path of changed file
        /// </returns>
        public static string TrimMp3(string mp3Path, string outputPath, double timeFromDouble, double duration)
        {
            var timeToDouble = timeFromDouble + duration;

            if (timeToDouble > timeFromDouble)
            {
                var timeFrom = TimeSpan.FromSeconds(timeFromDouble);
                var timeTo = TimeSpan.FromSeconds(timeToDouble);

                using (var reader = new Mp3FileReader(mp3Path))
                using (var writer = File.Create(outputPath))
                {
                    Mp3Frame frame;
                    while ((frame = reader.ReadNextFrame()) != null)
                    {
                        BitRate = frame.BitRate / 1000.0;
                        if (reader.CurrentTime >= timeFrom && reader.CurrentTime <= timeTo)
                        {
                            writer.Write(frame.RawData, 0, frame.RawData.Length);
                        }
                    }
                }
            }

            return outputPath;
        }

        /// <summary>
        /// Converts WAV to MP3.
        /// </summary>
        /// <param name="inputFile">The input file.</param>
        /// <returns>
        /// Path of changed file
        /// </returns>
        public static string ConvertWavToMp3(string inputFile)
        {
            return ConvertWavToMp3(inputFile, Path.ChangeExtension(inputFile, ".converted.mp3"));
        }

        /// <summary>
        /// Converts Wav to MP3.
        /// </summary>
        /// <param name="inputFile">The input file.</param>
        /// <param name="outputPath">The output path.</param>
        /// <returns>
        /// Path of changed file
        /// </returns>
        public static string ConvertWavToMp3(string inputFile, string outputPath)
        {
            var reader = new WaveStream(inputFile);

            try
            {
                var config = new BE_CONFIG(reader.Format, (uint)(Math.Abs(BitRate - 0.0) < 0.1 ? DefaultBitRate : BitRate));
                var writer = new Mp3Writer(new FileStream(outputPath, FileMode.Create), reader.Format, config);

                try
                {
                    var buffer = new byte[writer.OptimalBufferSize];
                    int read;
                    while ((read = reader.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        writer.Write(buffer, 0, read);
                    }
                }
                finally
                {
                    writer.Close();
                }
            }
            finally
            {
                reader.Close();
            }

            return outputPath;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the maximum.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>
        /// Max float value in file
        /// </returns>
        private static float GetMax(string file)
        {
            using (var reader = new WaveFileReader(file))
            {
                float[] buffer;
                float max = 0;

                while ((buffer = reader.ReadNextSampleFrame()) != null)
                {
                    for (var n = 0; n < buffer.Length; n++)
                    {
                        max = Math.Max(max, Math.Abs(buffer[n]));
                    }
                }

                return max;
            }
        }

        #endregion
    }
}