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


            byte[] soundData = File.ReadAllBytes(soundFilePath);
            AL.BufferData(_buffer, ALFormat.Mono16, soundData, soundData.Length, 44100);
            AL.Source(_source, ALSourcei.Buffer, _buffer);

            AL.Source(_source, ALSourcef.Gain, 1.0f); //громкость
            AL.Source(_source, ALSourceb.Looping, false); //повторение
        }

        //воспроизводим звук
        public void Play()
        {
            AL.SourcePlay(_source);
        }

    }
}
