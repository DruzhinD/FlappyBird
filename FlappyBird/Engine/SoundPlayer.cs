using GroupDocs.Metadata;
using GroupDocs.Metadata.Formats.Audio;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlappyBird.Engine
{
    public class SoundPlayer
    {
        private int _buffer;
        private int _source;
        private AudioContext _context;

        public SoundPlayer(string soundFilePath) 
        {
            _context = new AudioContext();
            _buffer = AL.GenBuffer();
            _source = AL.GenSource();

            if (AL.GetError() != ALError.NoError)
            {
                throw new AudioException("Проблема со звуком");
            }

            ALFormat format;
            int frequency;
            using (Metadata metadata = new Metadata("ветер.wav"))
            {
                var root = metadata.GetRootPackage<WavRootPackage>();
                format = GetSoundFormat(root.WavPackage.NumberOfChannels, root.WavPackage.BitsPerSample);
                frequency = root.WavPackage.SampleRate;
                Console.WriteLine("Bits per Sample: " + root.WavPackage.BitsPerSample); // Bits per Sample
                //Console.WriteLine("Block Align: " + root.WavPackage.BlockAlign); // Block Align
                //Console.WriteLine("Byte Rate: " + root.WavPackage.ByteRate); // Byte Rate
                Console.WriteLine("Number of Channels: " + root.WavPackage.NumberOfChannels); // Number of Channels
                //Console.WriteLine("Audio Format: " + root.WavPackage.AudioFormat); // Audio Format 
                Console.WriteLine("Sample Rate: " + root.WavPackage.SampleRate); // Sample Rate
            }

            byte[] soundData = File.ReadAllBytes(soundFilePath);
            AL.BufferData(_buffer, format, soundData, soundData.Length, frequency);
            AL.Source(_source, ALSourcei.Buffer, _buffer);

            AL.Source(_source, ALSourcef.Gain, 1.0f); //громкость
            AL.Source(_source, ALSourceb.Looping, false); //повторение
        }

        //воспроизводим звук
        public void Play()
        {
            AL.SourcePlay(_source);
        }

        static ALFormat GetSoundFormat(int channels, int bitsPerSample)
        {

            if (channels == 1 & bitsPerSample == 8)
                return ALFormat.Mono8;
            else if (channels == 2 & bitsPerSample == 8)
                return ALFormat.Stereo8;
            else if (channels == 2 & bitsPerSample == 16)
                return ALFormat.Stereo16;
            else
                return ALFormat.Mono16;
        }

    }
}
