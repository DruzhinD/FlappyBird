using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlappyBird.GameObjects
{
    /// <summary>
    /// Сущность, объединяющая птицу и стены
    /// </summary>
    internal abstract class Entity
    {
        /// <summary>
        /// координата, к которой привязана сущность
        /// </summary>
        public Vector2 position;

        /// <summary>
        /// цвет сущности
        /// </summary>
        public Vector3 Color { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position">координата сущности</param>
        /// <param name="color">цвет сущности</param>
        protected Entity(Vector2 position, Vector3 color)
        {
            this.position = position;
            Color = color;
        }
        
        protected Entity() { }

        /// <summary>
        /// отрисовка сущности
        /// </summary>
        public abstract void Draw();
    }
}
