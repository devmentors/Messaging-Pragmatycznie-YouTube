![logo](./docs/logo.png)


# Messaging: Pragmatycznie (YouTube)
To repozytorium zawiera aplikację Filo, która stanowić będzie oś dyskusji na temat komunikacji asynchronicznej w systemach rozporszonych. Nowe odcinki będą publikowane na **[naszym kanale YouTube.](https://www.youtube.com/@DevMentorsPL)**


# Lista odcinków
1. [Messaging, czyli po co nam komunikacja asynchroniczna?](https://www.youtube.com/watch?v=cA1Cpqk1Zxo)
2. ???
3. ???
4. ???


## Jak uruchomić?

RabbitMQ uruchamiamy naszybciej poprzez `docker-compose`. Przechodzimy do katalogu `compose`, a następnie w konsoli uruchamiamy polecenie:

```bash
docker-compose up -d
```

Management UI powinien być dostępny pod adrsem `localhost:15672`.


## Tworzenie topologii Filo
Projekt zawiera aplikację konsolową `Filo.Tools.RabbitMqTopology`, która pozwala na szybkie utworzenie topologii w RabbitMQ. Uruchom aplikację, a następnie sprawdź czy została utworzona wymiana (`files-exchange`) wraz z kolejkami dla każdej z aplikacji konsumującej komunikaty.