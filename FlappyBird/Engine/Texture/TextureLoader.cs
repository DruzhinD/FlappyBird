using OpenTK.Graphics.OpenGL4;
using System;
using System.IO;

namespace FlappyBird.Engine
{
    class TextureLoader
    {
        private string _directoryPath;
        private string[] _texturePaths;
        private Texture[] _textures;

        public TextureLoader(string configFilePath)
        {
            //получаем путь к директории
            _directoryPath = Path.GetDirectoryName(configFilePath);

            //загружаем всю информацию из текстового файла с именами текстур
            string configFile = LoadSource(configFilePath);
            _texturePaths = configFile.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < _texturePaths.Length; i++)
            {
                _texturePaths[i] = _directoryPath + @"\" + _texturePaths[i];
            }

            if (_texturePaths.Length > 32)
                throw new Exception("Слишком много текстур для загрузки");

            _textures = new Texture[_texturePaths.Length];

            //Создаем отдельные текстуры
            for (int i = 0; i < _textures.Length; i++)
                _textures[i] = new Texture(_texturePaths[i]);
        }

        /// <summary>Включаем использование текстур</summary>
        public void UseTextures()
        {
            var units = Enum.GetValues(typeof(TextureUnit));
            for (int i = 0; i < _textures.Length; i++)
            {
                _textures[i].Use((TextureUnit)units.GetValue(i));
            }
        }

        //загружаем индексы текстур
        public int[] GetTextureIndicies()
        {
            int[] indicies = new int[_textures.Length];
            for (int i = 0; i < indicies.Length; i++)
                indicies[i] = i;

            return indicies;
        }

        private string LoadSource(string path)
        {
            using (StreamReader reader = new StreamReader(path))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
