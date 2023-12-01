# RabbitTestTask
Чтобы запустить приложение нужно:
Чтобы на запускамом усройстве работал докер демон
Чтобы были установлены sdk 8.0
# Как запустить:
В консоли переходим в папку проекта
Ищем там docker compose файл
В директории с файлом пишем docker compose up
Все.
# Параметры передаваемые в при запуске приложения
Параметры передаются в 2 сервиса
Они указываются в файле docker
В этой строчке 
ENTRYPOINT  [ "dotnet", "DataProcessorService.dll", "rabbit", "guest", "guest", "/app/log/customer.log", "/db/ModuleCategory.db" ]
это передаваемые параметры "rabbit", "guest", "guest", "/app/log/customer.log", "/db/ModuleCategory.db"
Где: первый параметр (rabbit) это имя хоста сервиса RabbitMq 
второй и третий (guest) это логин и пароль от сервиса RabbitMq
четвертый (*.log) это имя файла лога, может быть любым, тут надо указывать полный путь, надо убедиться чтобы существовала папка в которой будет создавать лог файл
пятый (*db) это полный путь до файла с базой,база сама не создается, если хотите поменять имя, либо переменуйте ее, либо создайте новую с такой структурой.
# Предупреждения
Запускаются 3 контейнера, 1 с RabbitMq 2 других сервисы из задания
В сервисах из задания стоит время ожидания подключения к RabbitMq 16 секунд
Я решил что это лучше чем ждать бесконечность
При первом запуске docker compose файла, RabbitMq скорее всего за это время подняться не успеет, и вы увидите ошибку из сервиса FileParserService
Но на второй запуск всегда успевало подниматься
Также
На системе Windows у меня получилось проверить файл базы данных, там данные обновляются раз в секунду, программы работает как ожидается
На системе Linux у меня не получилось проверить файл базы данных, но программа работает штатно.
