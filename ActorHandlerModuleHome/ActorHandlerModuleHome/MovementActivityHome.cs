using System;

using NodaTime; // Отсюда LocalTime
using NetTopologySuite.Geometries;  // Отсюда Point и другая геометрия
using NetTopologySuite.Mathematics; // Отсюда векторы

using OSMLSGlobalLibrary.Modules;  // Отсюда OSMLSModule

using ActorModule;

using OSMLSGlobalLibrary.Map;

using PathsFindingCoreModule;

namespace ActorHandlerModuleHome
{
    // Активность которая заставляет актора куда-то идти
    public class MovementActivityHome : IActivity
    {
        public Coordinate[] Path;
        public int i = 0;

        public bool IsPath = true;

        public int Priority { get; set; } = 1;

        public MovementActivityHome()
        {
        }

        public MovementActivityHome(int priority)
        {
            Priority = priority;
        }

        // Здесь происходит работа с актором
        public bool Update(Actor actor, double deltaTime)
        {
            double speed = 10;

            // Расстояние, которое может пройти актор с заданной скоростью за прошедшее время
            double distance = speed * deltaTime;

            if (IsPath)
            {
                var firstCoordinate = new Coordinate(actor.X, actor.Y);
                var secondCoordinate = new Coordinate(actor.State.Home.X, actor.State.Home.Y);
                Path = PathsFinding.GetPath(firstCoordinate, secondCoordinate, "Walking").Result.Coordinates;
                IsPath = false;
            }

            Vector2D direction = new Vector2D(actor.Coordinate, Path[i]);
            // Проверка на перешагивание
            
            if (direction.Length() <= distance)
            {
                // Шагаем в точку, если она ближе, чем расстояние которое можно пройти
                actor.X = Path[i].X;
                actor.Y = Path[i].Y;
            }
            else
            {
                // Вычисляем новый вектор, с направлением к точке назначения и длинной в distance
                direction = direction.Normalize().Multiply(distance);

                // Смещаемся по вектору
                actor.X += direction.X;
                actor.Y += direction.Y;
            }

            if (actor.X == Path[i].X && actor.Y == Path[i].Y && i < Path.Length - 1)
            {
                i++;
                Console.WriteLine(i);
                Console.WriteLine(Path.Length);
            }

            // Если в процессе шагания мы достигли точки назначения
            if (actor.X == Path[Path.Length-1].X && actor.Y == Path[Path.Length-1].Y)
            {
                Console.WriteLine("prikol");
                actor.Activity = new WaitingActivityHome(actor.State.HomeTime);
                i = 0;
                IsPath = true;
                //WaitingActivityHome wait = new WaitingActivityHome(actor.State.HomeTime);
                //while (true)
                //{
                //    if (wait.Update(actor, deltaTime))
                //    {
                //        return true;  // Выходим с флагом true - активность завершена
                //    }
                //}
            }
            return false;
        }
    }
}
