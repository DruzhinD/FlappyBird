namespace FlappyBird.Engine
{
    class Rectangle
    {
        /// <summary>координаты (0-2) <br/> координаты текстуры (3-4) <br/> индекс текстуры (5)</summary>
        public float[] Verticies { get; set; }

        //индексы
        public uint[] Indicies { get; set; } =
        {
            0, 1, 2,
            1, 2, 3
        };

        /// <summary>режим отрисовки</summary>
        public enum RectMode
        {
            Center, //переданные координаты - центр
            Left //переданные координаты - левый нижний угол
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="posX">координата по оси X</param>
        /// <param name="posY">координата по оси Y</param>
        /// <param name="width">ширина</param>
        /// <param name="height">высота</param>
        /// <param name="textureIndex">индекс текстуры</param>
        /// <param name="mode">режим отрисовки</param>
        public Rectangle(float posX, float posY, float width, float height, float textureIndex, RectMode mode = RectMode.Center)
        {
            if (mode == RectMode.Center)
            {
                float[] verticies =
                {
                    //первые три - xyz                        координаты текстуры и её индекс
                    posX - width/2, posY - height/2, 0.0f,    0.0f, 1.0f, textureIndex, //нижняя левая вершина
                    posX - width/2, posY + height/2, 0.0f,    0.0f, 0.0f, textureIndex, //верхняя левая
                    posX + width/2, posY - height/2, 0.0f,    1.0f, 1.0f, textureIndex, //нижняя правая
                    posX + width/2, posY + height/2, 0.0f,    1.0f, 0.0f, textureIndex //верхняя правая
                };

                this.Verticies = verticies;
            }
            else if (mode == RectMode.Left)
            {
                float[] verticies =
                {   //позиция|координаты                      координаты текстуры и её индекс
                    posX, posY, 0.0f,                         0.0f, 1.0f,   textureIndex, //нижняя левая вершина
                    posX, posY + height, 0.0f,                0.0f, 0.0f,   textureIndex, //верхняя левая
                    posX + width, posY, 0.0f,                 1.0f, 1.0f,   textureIndex, //нижняя правая
                    posX + width, posY + height, 0.0f,        1.0f, 0.0f,   textureIndex //верхняя правая
                 };

                this.Verticies = verticies;
            }
        }
    }
}
