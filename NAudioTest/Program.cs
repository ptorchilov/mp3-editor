using System;
using System.IO;
using WaveLib;
using Yeti.Lame;
using Yeti.MMedia;
using Yeti.MMedia.Mp3;

namespace NAudioTest
{
    public class Program
    {
        private static void Main()
        {
            const String Mp3Path =
                @"D:\test\01 Ef.mp3";
            try
            {
                //trim mp3
                var trimmedMp3 = EffectsUtility.TrimMp3(Mp3Path, 33.0, 24.0);

                //convert to wav
                var wavFile = EffectsUtility.ConvertToWav(trimmedMp3);

                //fade in in begin
                var fadein = EffectsUtility.BeginFadeIn(wavFile, 5.0);

                //fade out in end
                var fadeout = EffectsUtility.EndFadeOut(wavFile, 5.0);

                //change volume
                var volume = EffectsUtility.ChangeVolume(fadein, 2.0);

                //normalize
                var normalize = EffectsUtility.Normalize(volume);

                //convert to mp3
                EffectsUtility.ConvertWavToMp3(normalize);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}