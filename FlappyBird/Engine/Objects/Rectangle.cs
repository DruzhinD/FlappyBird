namespace FlappyBird.Engine
{
    class Rectangle
    {
        public float[] Verticies { get; set; }

        //индексы
        public uint[] Indicies { get; set; } =
        {
            0, 1, 2,
            1, 2, 3
        };

        public float PosX { get; }
        public float PosY { get; }
        public float Width { get; }
        public float Height { get; }
        public float TextureIndex { get; }

        public enum RectMode
        {
            Center,
            Left
        }

        public Rectangle(float posX, float posY, float width, float height, float textureIndex, RectMode mode = RectMode.Center)
        {
            PosX = posX;
            PosY = posY;
            Width = width;
            Height = height;
            TextureIndex = textureIndex;

            //в конструктор были переданы координаты центра прямоугольника (объекта)
            if (mode == RectMode.Center)
            {
                float[] verticies =
                {
                    //первые три - xyz                        координаты тексты (4,5) и индекс текстуры (6)
                    posX - width/2, posY - height/2, 0.0f,    0.0f, 1.0f, textureIndex, //bottom left
                    posX - width/2, posY + height/2, 0.0f,    0.0f, 0.0f, textureIndex, //top left
                    posX + width/2, posY - height/2, 0.0f,    1.0f, 1.0f, textureIndex, //bottom right
                    posX + width/2, posY + height/2, 0.0f,    1.0f, 0.0f, textureIndex //top right
                };

                this.Verticies = verticies;
            }
            //отрисовка начинается БЕЗ модификации переданных координат
            //т.е. первая вершина = переданным координатам
            else if (mode == RectMode.Left)
            {
                float[] verticies =
                {   //позиция|координаты                      координаты текстуры и её индекс
                    posX, posY, 0.0f,                         0.0f, 1.0f,   textureIndex, //bottom left нижняя левая вершина
                    posX, posY + height, 0.0f,                0.0f, 0.0f,   textureIndex, //top left верхняя левая
                    posX + width, posY, 0.0f,                 1.0f, 1.0f,   textureIndex, //bottom right нижняя правая
                    posX + width, posY + height, 0.0f,        1.0f, 0.0f,   textureIndex //top right верхняя правая
                 };

                this.Verticies = verticies;
            }
        }
    }
}
