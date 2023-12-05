namespace FlappyBird.Engine
{
    /// <summary>
    /// Структура, необходимая для указания на размер вершины
    /// </summary>
    unsafe struct Vertex
    {
        public fixed float Position[3];
        public fixed float TexCoords[2];
        public float TextureIndex;
    }
}
