namespace FlappyBird.Engine
{
    class Rectangle
    {
        public float[] Verticies { get; set; }

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

            if (mode == RectMode.Center)
            {
                float[] verticies =
                {
                    posX - width/2, posY - height/2, 0.0f,    0.0f, 1.0f, textureIndex, //bottom left
                    posX - width/2, posY + height/2, 0.0f,    0.0f, 0.0f, textureIndex, //top left
                    posX + width/2, posY - height/2, 0.0f,    1.0f, 1.0f, textureIndex, //bottom right
                    posX + width/2, posY + height/2, 0.0f,    1.0f, 0.0f, textureIndex //top right
                };

                this.Verticies = verticies;
            }
            else if (mode == RectMode.Left)
            {
                float[] verticies =
                {   //position                                texture coords
                    posX, posY, 0.0f,                         0.0f, 1.0f,   textureIndex, //bottom left
                    posX, posY + height, 0.0f,                0.0f, 0.0f,   textureIndex, //top left
                    posX + width, posY, 0.0f,                 1.0f, 1.0f,   textureIndex, //bottom right
                    posX + width, posY + height, 0.0f,        1.0f, 0.0f,   textureIndex //top right
                 };

                this.Verticies = verticies;
            }
        }
    }
}
