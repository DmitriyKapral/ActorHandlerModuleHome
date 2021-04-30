using System;
using System.Collections.Generic;
using System.Text;
using OSMLSGlobalLibrary.Modules;

namespace ActorHandlerModuleHome
{
    public class ActorHandlerModuleHome : OSMLSModule
    {

        /// <summary>
        /// Инициализация модуля. В отладочной конфигурации выводит сообщение
        /// </summary>
        protected override void Initialize()
        {
#if DEBUG
            Console.WriteLine("ActorHandlerModule: Initialize");
#endif
        }
        /// <summary>
        /// Вызывает Update на всех акторах
        /// </summary>
        public override void Update(long elapsedMilliseconds)
        {
        }
    }
}
