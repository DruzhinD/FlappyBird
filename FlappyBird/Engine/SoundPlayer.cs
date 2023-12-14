using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using System.IO;

namespace FlappyBird.Engine
{
    public class SoundPlayer
    {
        private int _buffer;
        private int _source;
        private AudioContext _context;

        /// <param name="soundFilePath">путь к аудиофайлу</param>
        /// <param name="soundLoud">громкость звука, не больше 1f</param>
        public SoundPlayer(string soundFilePath, float soundLoud = 0.4f) 
        {
            _context = new AudioContext();
            _buffer = AL.GenBuffer();
            _source = AL.GenSource();

            if (AL.GetError() != ALError.NoError)
            {
                throw new AudioException("Проблема со звуком");
            }

            //считываем поток байтов из аудиофайла
            byte[] soundData = File.ReadAllBytes(soundFilePath);
            //привязываемся к буферу
            AL.BufferData(_buffer, ALFormat.Mono16, soundData, soundData.Length, 44100);
            AL.Source(_source, ALSourcei.Buffer, _buffer);

            AL.Source(_source, ALSourcef.Gain, soundLoud); //громкость
            AL.Source(_source, ALSourceb.Looping, false); //повторение
        }

        /// <summary>
        /// Воспроизвести звук
        /// </summary>
        public void Play()
        {
            AL.SourcePlay(_source);
        }
    }
}
