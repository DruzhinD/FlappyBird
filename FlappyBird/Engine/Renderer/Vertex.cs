namespace FlappyBird.Engine
{
    /// <summary>
    /// Структура, необходимая для указания на размер вершины
    /// </summary>
    //unsafe для создания буфера фиксированного размера
    unsafe struct Vertex
    {
        public fixed float Position[3]; //4*3 байт
        public fixed float TexCoords[2]; //4*2
        public float TextureIndex; //4
    }
}
