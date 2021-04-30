using System;

using NodaTime; // Отсюда LocalTime
using NetTopologySuite.Geometries;
using NetTopologySuite.Mathematics;

using OSMLSGlobalLibrary.Modules; 

using ActorModule;

using OSMLSGlobalLibrary.Map;

using PathsFindingCoreModule;

using ActorHandlerModuleHome;

namespace ActorHandlerTestModuleHome
{
    public class ActorHandlerTestModule : OSMLSModule
    {
        [CustomStyle(@"new style.Style({
                stroke: new style.Stroke({
                    color: 'rgba(90, 0, 157, 1)',
                    width: 2
                })
            });
        ")]
        private class Highlighted : GeometryCollection
        {
            public Highlighted(Geometry[] geometries) : base(geometries)
            {
            }
        }

        private double x = 4173165;
        private double y = 7510997;

        // Зададим радиус, в котором будут ходить акторы
        private double radius = 100;

        //Генератор случайных чисел
        private Random random = new Random();

        // И случайное смещение от центра, которое будем использовать для создания точек интереса
        private double offset { get { return random.NextDouble() * 2 * radius - radius; } }

        // Этот метод будет вызван один раз при запуске, соответственно тут вся инициализация
        protected override void Initialize()
        {

            State state = new State()
            {
                Hunger = 100,

                // Интервал можно задать через объекты LocalTime
                HomeTime = new TimeInterval(new LocalTime(20, 0), new LocalTime(18, 23)),

            };

            // Создаём акторов
            for (int i = 0; i < 2; i++)
            {
                Console.WriteLine($"Creating actor {i + 1}");

                // Делаем для каждого точку дома и точку работы в квадрате заданного радиуса от точки спавна
                state.Home = new Point(x + offset, y + offset);
                state.Job = new Point(x + offset, y + offset);

                Console.WriteLine($"Home at {state.Home.X}, {state.Home.Y}; " +
                                  $"Job at {state.Job.X}, {state.Job.Y}");

                // Создаём актора с заданным состоянием
                // Так как в конструкторе актора состояние копируется, 
                // можно использовать один и тот же объект состояния для инициализации,
                // при этом каждый актор получит отдельный объект, не связанный с другими
                Actor actor = new Actor(x, y, state);

                // Добавляем актора в объекты карты
                MapObjects.Add(actor);

                var firstCoordinate = new Coordinate(x, y);
                var secondCoordinate = new Coordinate(state.Home.X, state.Home.Y);

                Console.WriteLine($"Coor {firstCoordinate} and {secondCoordinate}");


                Console.WriteLine("Building path...");
                MapObjects.Add(new Highlighted(new Geometry[]
                    {
                                PathsFinding.GetPath(firstCoordinate, secondCoordinate, "Walking").Result
                    }));
                Console.WriteLine("Path was builded");
            }

            // Получаем список акторов на карте и выводим их количество
            var actors = MapObjects.GetAll<Actor>();
            Console.WriteLine($"Added {actors.Count} actors");

            foreach (var actor in actors)
                Console.WriteLine($"Actor on ({actor.Coordinate.X}, {actor.Coordinate.Y})\n" +
                                  $"\tHome at {actor.State.Home.X}, {actor.State.Home.Y}\n");
        }
        // Этот метод вызывается регулярно, поэтому тут все действия, которые будут повторяться
        public override void Update(long elapsedMilliseconds)
        {
            var actors = MapObjects.GetAll<Actor>();
            Console.WriteLine($"Got {actors.Count} actors\n");

            // Для каждого актёра проверяем условия и назначаем новую активность если нужно
            foreach (var actor in actors)
            {
                // Есть ли активность
                bool isActivity = actor.Activity != null;

                // Если активность дороги домой
                bool goHome = isActivity ? actor.Activity is MovementActivityHome : false;

                // Если ли активность ожидания
                bool goWait = isActivity ? actor.Activity is WaitingActivityHome : false;


                Console.WriteLine($"Flags: {isActivity} {goHome} {goWait}");

                if (!isActivity)
                {
                    // Назначить актору путь до дома
                    Console.WriteLine("Said actor go home\n");
                    actor.Activity = new MovementActivityHome();
                }

            }
        }
    }
}
